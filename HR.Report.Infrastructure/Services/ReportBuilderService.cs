using AutoMapper;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using HR.Report.Core.DTOs;
using HR.Report.Core.Entity;
using HR.Report.Infrastructure.Data;
using HR.Report.Infrastructure.GlobalDBContext;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Globalization;
using System.Text;
using DocumentFormat.OpenXml;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;

namespace HR.Report.Infrastructure.Services;

/// <summary>
/// سرویس گزارش ساز پویا - با استفاده از GlobalDbContext
/// </summary>
public class ReportBuilderService : IScopedServices
{
    private readonly IUnitOfWork<ReportContext> _unitOfWork;
    private readonly IUnitOfWork<BaseInfoContext> _baseInfoUnitOfWork;
    private readonly IUnitOfWork<IdentityContext> _identityUnitOfWork;
    private readonly GlobalDbContext _globalDbContext;
    private readonly IMapper _mapper;
    private readonly IDapper _dapper;
    private readonly string _connectionString;
    private readonly UserResolverService _userResolverService;
    private readonly ILogger<ReportBuilderService> _logger;

    public long _currentUserDefaultOrganId;

    public ReportBuilderService(IMapper mapper, IUnitOfWork<ReportContext> unitOfWork, IUnitOfWork<BaseInfoContext> baseInfoUnitOfWork, IUnitOfWork<IdentityContext> identityUnitOfWork, GlobalDbContext globalDbContext, IDapper dapper, UserResolverService userService, ILogger<ReportBuilderService> logger)
    {
        _unitOfWork = unitOfWork;
        _baseInfoUnitOfWork = baseInfoUnitOfWork;
        _identityUnitOfWork = identityUnitOfWork;
        _globalDbContext = globalDbContext;
        _mapper = mapper;
        _dapper = dapper;
        _userResolverService = userService;
        _logger = logger;

        // Get connection string
        var configuration = new Microsoft.Extensions.Configuration.ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location))
#if DEBUG
            .AddJsonFile("appsettings.Development.json")
#else
            .AddJsonFile("appsettings.json")
#endif
            .Build();

        var raw = configuration.GetConnectionString("HRMSConnection");
        var dec = HR.SharedKernel.Security.ConnectionStringProtector.TryUnprotect(raw);
        _connectionString = dec ?? raw;
    }

    /// <summary>
    /// دریافت محدودیت تعداد رکورد گزارش از تنظیمات
    /// </summary>
    private int GetReportRecordLimit()
    {
        try
        {
            var setting = _baseInfoUnitOfWork.Context.Set<Setting>()
                .FirstOrDefault(s => s.Id == 10020 && !s.IsDeleted);

            if (setting != null && !string.IsNullOrWhiteSpace(setting.Value))
            {
                if (int.TryParse(setting.Value, out int limit) && limit > 0)
                {
                    return limit;
                }
            }

            // Default limit if setting not found or invalid
            return 10000;
        }
        catch
        {
            // Return default limit in case of any error
            return 10000;
        }
    }

    /// <summary>
    /// دریافت فهرست ReportableEntityId های مجاز برای کاربر جاری
    /// اگر کاربر ادمین باشد، null برمی‌گرداند (یعنی همه موجودیت‌ها مجاز هستند)
    /// </summary>
    private HashSet<long>? GetAllowedReportableEntityIds()
    {
        // بررسی اینکه آیا کاربر ادمین است
        if (_userResolverService.IsAdmin())
        {
            return null; // null یعنی همه موجودیت‌ها مجاز هستند
        }

        var userId = _userResolverService.GetUserId();
        if (userId <= 0)
        {
            return new HashSet<long>(); // کاربر نامعتبر - هیچ موجودیتی مجاز نیست
        }

        var allowedIds = new HashSet<long>();

        // دریافت ReportableEntityId های مستقیم کاربر
        var userEntityIds = _identityUnitOfWork.Context.Set<UserReportableEntity>()
            .Where(ure => ure.UserId == userId && !ure.IsDeleted)
            .Select(ure => ure.ReportableEntityId)
            .ToList();

        foreach (var id in userEntityIds)
        {
            allowedIds.Add(id);
        }

        // دریافت Role های کاربر
        var userRoleIds = _identityUnitOfWork.Context.Set<UserRole>()
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.RoleId)
            .ToList();

        if (userRoleIds.Any())
        {
            // دریافت ReportableEntityId های نقش‌های کاربر
            var roleEntityIds = _identityUnitOfWork.Context.Set<RoleReportableEntity>()
                .Where(rre => userRoleIds.Contains(rre.RoleId) && !rre.IsDeleted)
                .Select(rre => rre.ReportableEntityId)
                .ToList();

            foreach (var id in roleEntityIds)
            {
                allowedIds.Add(id);
            }
        }

        return allowedIds;
    }

    /// <summary>
    /// بررسی دسترسی کاربر به یک ReportableEntity
    /// </summary>
    private bool HasAccessToReportableEntity(long entityId)
    {
        var allowedIds = GetAllowedReportableEntityIds();
        
        // اگر null باشد یعنی کاربر ادمین است و به همه دسترسی دارد
        if (allowedIds == null)
        {
            return true;
        }

        return allowedIds.Contains(entityId);
    }

    /// <summary>
    /// دریافت متادیتای گزارش ساز
    /// </summary>
    public OperationResult GetMetadata(long? entityId = null)
    {
        var allEntities = _unitOfWork.Context.Set<ReportableEntity>()
            .Where(e => e.IsActive)
            .OrderBy(e => e.FriendlyName)
            .ToList();

        // فیلتر کردن موجودیت‌ها بر اساس دسترسی کاربر
        var allowedIds = GetAllowedReportableEntityIds();
        var entities = allowedIds == null
            ? allEntities // اگر ادمین است، همه موجودیت‌ها
            : allEntities.Where(e => allowedIds.Contains(e.Id)).ToList(); // در غیر این صورت فقط موجودیت‌های مجاز

        var fields = entityId.HasValue
            ? _unitOfWork.Context.Set<ReportableField>()
                .Include(f => f.FieldDataType)
                .Where(f => f.ReportableEntityId == entityId.Value && f.IsActive)
                .OrderBy(f => f.Priority)                          // Sort by priority ascending
                .ThenBy(f => f.FriendlyName)                      // Then by friendly name
                .ToList()
            : new List<ReportableField>();

        // dataTypes و operators فقط هنگام انتخاب موجودیت لازم هستند
        var dataTypes = entityId.HasValue
            ? _unitOfWork.Context.Set<FieldDataType>()
                .Include(dt => dt.FieldOperators)
                .AsNoTracking()
                .ToList()
            : new List<FieldDataType>();

        var operators = entityId.HasValue
            ? _unitOfWork.Context.Set<FieldOperator>()
                .Include(o => o.FieldDataType)
                .AsNoTracking()
                .ToList()
            : new List<FieldOperator>();

        var recordLimit = GetReportRecordLimit();

        // Get entity for checking column types
        var entity = entityId.HasValue 
            ? _unitOfWork.Context.Set<ReportableEntity>().FirstOrDefault(e => e.Id == entityId.Value)
            : null;

        // Map fields and set IsLongColumn and TargetTableName properties
        var fieldDTOs = _mapper.Map<List<ReportableFieldDTO>>(fields);
        if (entity != null)
        {
            foreach (var fieldDto in fieldDTOs)
            {
                var field = fields.FirstOrDefault(f => f.Id == fieldDto.Id);
                if (field != null)
                {
                    fieldDto.IsLongColumn = IsLongField(field, entity);
                    // Get target table name for FK fields
                    fieldDto.TargetTableName = GetTargetTableName(field, entity);
                }
            }
        }

        var metadata = new ReportBuilderMetadataDTO
        {
            Entities = _mapper.Map<List<ReportableEntityDTO>>(entities),
            Fields = fieldDTOs,
            DataTypes = _mapper.Map<List<FieldDataTypeDTO>>(dataTypes),
            Operators = _mapper.Map<List<FieldOperatorDTO>>(operators),
            MaxRecordLimit = recordLimit
        };

        return OperationResult.Succeeded(payload: metadata);
    }

    /// <summary>
    /// دریافت محدودیت تعداد رکورد برای نمایش به کاربر
    /// </summary>
    public OperationResult GetReportLimit()
    {
        var limit = GetReportRecordLimit();
        return OperationResult.Succeeded(payload: new { maxRecords = limit });
    }

    /// <summary>
    /// اجرای گزارش و دریافت نتایج - با استفاده از GlobalDbContext
    /// </summary>
    public OperationResult ExecuteReport(ReportBuilderRequestDTO request)
    {
        try
        {
            // Validate request
            if (request.EntityId <= 0)
                return OperationResult.Failed("موجودیت انتخاب نشده است");

            if (request.SelectedFieldIds == null || !request.SelectedFieldIds.Any())
                return OperationResult.Failed("حداقل یک فیلد باید انتخاب شود");

            // اعتبارسنجی دسترسی: بررسی اینکه کاربر به ReportableEntity دسترسی دارد
            if (!HasAccessToReportableEntity(request.EntityId))
            {
                return OperationResult.Failed("شما به این موجودیت گزارش‌گیری دسترسی ندارید");
            }

            // اعتبارسنجی امنیتی: بررسی اینکه PayLocationIds ارسالی از کلاینت فقط شامل محل‌های پرداخت کاربر جاری باشد
            if (request.PayLocationIds != null && request.PayLocationIds.Any())
            {
                var validationResult = ValidatePayLocationIds(request.PayLocationIds);
                if (!validationResult.Success)
                {
                    return validationResult;
                }
            }

            // Get entity
            var entity = _unitOfWork.Context.Set<ReportableEntity>()
                .FirstOrDefault(e => e.Id == request.EntityId);

            if (entity == null)
                return OperationResult.NotFound();

            // Get fields (including filter fields)
            var allFieldIds = request.SelectedFieldIds.ToList();
            if (request.Filters != null && request.Filters.Any())
            {
                allFieldIds.AddRange(request.Filters.Select(f => f.FieldId).Where(id => !allFieldIds.Contains(id)));
            }

            // Add GroupBy field IDs to allFieldIds
            if (request.GroupByFieldIds != null && request.GroupByFieldIds.Any())
            {
                allFieldIds.AddRange(request.GroupByFieldIds.Where(id => !allFieldIds.Contains(id)));
            }

            var allFields = _unitOfWork.Context.Set<ReportableField>()
                .Include(f => f.FieldDataType)
                .Where(f => allFieldIds.Contains(f.Id))
                .ToList();

            if (!allFields.Any())
                return OperationResult.Failed("فیلدهای انتخابی یافت نشد");

            // Separate selected fields from all fields and maintain the order from request
            var selectedFields = request.SelectedFieldIds
                .Select(id => allFields.FirstOrDefault(f => f.Id == id))
                .Where(f => f != null)
                .ToList()!;

            // Build SQL query
            var sqlQuery = BuildSqlQueryForGlobalContext(entity, selectedFields, allFields, request);

            // Log SQL query and parameters for debugging
            System.Diagnostics.Debug.WriteLine($"=== SQL Query ===");
            System.Diagnostics.Debug.WriteLine(sqlQuery.Query);
            System.Diagnostics.Debug.WriteLine($"=== Parameters ===");
            foreach (var param in sqlQuery.Parameters)
            {
                System.Diagnostics.Debug.WriteLine($"{param.Key} = {param.Value} (Type: {param.Value?.GetType().Name ?? "null"})");
            }

            // Execute query using connection from GlobalDbContext
            var connectionString = _globalDbContext.Database.GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            using var command = new SqlCommand(sqlQuery.Query, connection);
            
            // Set command timeout to 60 seconds (1 minute)
            command.CommandTimeout = 60;
            
            // Add parameters
            foreach (var param in sqlQuery.Parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                System.Diagnostics.Debug.WriteLine($"Added parameter: {param.Key} = {param.Value} (Type: {param.Value?.GetType().Name ?? "null"})");
            }

            // Read data
            var data = new List<Dictionary<string, object?>>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var row = new Dictionary<string, object?>();
                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        var value = reader.IsDBNull(i) ? null : reader.GetValue(i);

                        // Convert DateTime to Persian date string
                        if (value is DateTime dateValue)
                        {
                            value = ConvertToPersianDate(dateValue);
                        }
                        else if (value is DateTimeOffset dateTimeOffsetValue)
                        {
                            value = ConvertToPersianDate(dateTimeOffsetValue.DateTime);
                        }

                        row[reader.GetName(i)] = value;
                    }
                    data.Add(row);
                }
            }

            // Get total count
            int totalCount = data.Count;
            if (request.PageSize > 0)
            {
                // For paging, we need to get total count separately
                var countQueryResult = BuildCountQuery(entity, allFields, request);
                using var countCommand = new SqlCommand(countQueryResult.Query, connection);
                countCommand.CommandTimeout = 60; // Set timeout to 60 seconds
                foreach (var param in countQueryResult.Parameters)
                {
                    countCommand.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
                }
                totalCount = (int)countCommand.ExecuteScalar();
            }

            // Check record limit after getting total count
            var recordLimit = GetReportRecordLimit();
            if (totalCount > recordLimit)
            {
                return OperationResult.Failed($"تعداد رکوردهای گزارش ({totalCount:N0}) بیشتر از حد مجاز ({recordLimit:N0}) است. لطفاً فیلترهای بیشتری اعمال کنید یا برای گزارش‌های سنگین‌تر با راهبر سامانه تماس حاصل بفرمایید.");
            }

            // Build response
            var response = new ReportBuilderResponseDTO
            {
                Columns = new List<ReportColumnDTO>(),
                Data = data,
                TotalCount = totalCount,
                CurrentPage = request.CurrentPage,
                PageSize = request.PageSize
            };

            // دریافت collection navigation info برای ساخت ستون‌های داینامیک
            var (singleNavigations, collectionNavigations) = GetNavigationInfoWithCollections(entity, allFields);
            
            // محاسبه MaxRows برای collection navigation ها
            foreach (var collectionNav in collectionNavigations)
            {
                var maxCount = GetMaxCollectionCount(collectionNav, entity, allFields, request);
                collectionNav.MaxRows = maxCount;
            }

            // اگر IncludeInterdictOrderWageItems فعال باشد و entity دارای InterdictOrderId باشد
            InterdictOrderWageItemInfo? interdictOrderWageItemInfo = null;
            if (request.IncludeInterdictOrderWageItems && HasInterdictOrderIdField(entity))
            {
                interdictOrderWageItemInfo = new InterdictOrderWageItemInfo
                {
                    ParentAlias = entity.TableName,
                    MaxRows = GetMaxInterdictOrderWageItemCount(entity, allFields, request)
                };
            }

            // اگر IncludeInterdictOrderCoefficientItems فعال باشد و entity دارای InterdictOrderId باشد
            InterdictOrderCoefficientItemInfo? interdictOrderCoefficientItemInfo = null;
            if (request.IncludeInterdictOrderCoefficientItems && HasInterdictOrderIdField(entity))
            {
                interdictOrderCoefficientItemInfo = new InterdictOrderCoefficientItemInfo
                {
                    ParentAlias = entity.TableName,
                    MaxRows = GetMaxInterdictOrderCoefficientItemCount(entity, allFields, request)
                };
            }

            // اگر IncludeFicheItems فعال باشد و entity دارای EmployeeId باشد
            FicheItemInfo? ficheItemInfo = null;
            if (request.IncludeFicheItems && HasEmployeeIdField(entity))
            {
                ficheItemInfo = new FicheItemInfo
                {
                    ParentAlias = entity.TableName,
                    MaxRows = GetMaxFicheItemCount(entity, allFields, request),
                    PaymentPeriodYear = request.FichePaymentPeriodYear,
                    PaymentPeriodMonth = request.FichePaymentPeriodMonth
                };
            }

            // اگر IncludePersonnelFunctionItems فعال باشد و entity دارای EmployeeId باشد
            PersonnelFunctionInfo? personnelFunctionInfo = null;
            if (request.IncludePersonnelFunctionItems && HasEmployeeIdField(entity))
            {
                personnelFunctionInfo = new PersonnelFunctionInfo
                {
                    ParentAlias = entity.TableName,
                    ParentKeyColumn = "EmployeeId",
                    PaymentPeriodYear = request.FichePaymentPeriodYear,
                    PaymentPeriodMonth = request.FichePaymentPeriodMonth
                };
            }

            // اگر IncludeFicheLeaveItems فعال باشد و entity دارای EmployeeId باشد
            FicheLeaveItemInfo? ficheLeaveItemInfo = null;
            if (request.IncludeFicheLeaveItems && HasEmployeeIdField(entity))
            {
                ficheLeaveItemInfo = new FicheLeaveItemInfo
                {
                    ParentAlias = entity.TableName,
                    MaxRows = GetMaxFicheLeaveItemCount(entity, allFields, request),
                    PaymentPeriodYear = request.FichePaymentPeriodYear,
                    PaymentPeriodMonth = request.FichePaymentPeriodMonth
                };
            }

            // Check if GroupBy is enabled
            var hasGroupBy = request.GroupByFieldIds != null && request.GroupByFieldIds.Any();
            var groupByFields = hasGroupBy 
                ? allFields.Where(f => request.GroupByFieldIds.Contains(f.Id)).ToList()
                : new List<ReportableField>();

            if (hasGroupBy && groupByFields.Any())
            {
                // Add GroupBy fields to columns
                foreach (var groupByField in groupByFields)
                {
                    string technicalName;
                    if (string.IsNullOrEmpty(groupByField.NavigationPath))
                    {
                        technicalName = groupByField.TechnicalName;
                    }
                    else
                    {
                        var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == groupByField.NavigationPath);
                        if (navInfo != null)
                        {
                            technicalName = $"{groupByField.NavigationPath!.Replace(".", "_")}_{groupByField.TechnicalName}";
                        }
                        else
                        {
                            technicalName = groupByField.TechnicalName;
                        }
                    }

                    response.Columns.Add(new ReportColumnDTO
                    {
                        FieldId = groupByField.Id,
                        TechnicalName = technicalName,
                        FriendlyName = groupByField.FriendlyName,
                        DataType = groupByField.FieldDataType?.TypeName
                    });

                    // Only add Title column if:
                    // 1. Field has navigation path AND TechnicalName is NOT "Title" (to avoid duplicate)
                    // 2. OR Field is FK (ends with Id) - then add Title from related table
                    bool shouldAddTitleColumn = false;
                    string? titleTechnicalName = null;

                    if (!string.IsNullOrEmpty(groupByField.NavigationPath))
                    {
                        // If TechnicalName is already "Title", don't add it again
                        if (groupByField.TechnicalName?.ToLower() != "title")
                        {
                            var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == groupByField.NavigationPath);
                            if (navInfo != null)
                            {
                                titleTechnicalName = $"{groupByField.NavigationPath!.Replace(".", "_")}_Title";
                                shouldAddTitleColumn = true;
                            }
                        }
                    }
                    // If field is FK (ends with Id) and doesn't have navigation path, add Title column
                    else if (!string.IsNullOrEmpty(groupByField.TechnicalName) && 
                             groupByField.TechnicalName.EndsWith("Id") && 
                             groupByField.TechnicalName.Length > 2)
                    {
                        // Try to find navigation property for this FK
                        var navPropertyName = groupByField.TechnicalName.Substring(0, groupByField.TechnicalName.Length - 2);
                        var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == navPropertyName);
                        if (navInfo != null)
                        {
                            titleTechnicalName = $"{navPropertyName}_Title";
                            shouldAddTitleColumn = true;
                        }
                    }

                    if (shouldAddTitleColumn && !string.IsNullOrEmpty(titleTechnicalName))
                    {
                        response.Columns.Add(new ReportColumnDTO
                        {
                            FieldId = 0, // Special field ID for Title
                            TechnicalName = titleTechnicalName,
                            FriendlyName = $"{groupByField.FriendlyName} (عنوان)",
                            DataType = "Text"
                        });
                    }
                }

                // Add Count column
                response.Columns.Add(new ReportColumnDTO
                {
                    FieldId = 0, // Special field ID for Count
                    TechnicalName = "Count",
                    FriendlyName = "تعداد",
                    DataType = "Int32"
                });
            }
            else
            {
                // اضافه کردن ستون‌های عادی (غیر collection)
                foreach (var field in selectedFields.Where(f => 
                    string.IsNullOrEmpty(f.NavigationPath) || 
                    !collectionNavigations.Any(cn => f.NavigationPath?.StartsWith(cn.NavigationProperty) == true)))
                {
                    response.Columns.Add(new ReportColumnDTO
                    {
                        FieldId = field.Id,
                        TechnicalName = string.IsNullOrEmpty(field.NavigationPath) 
                            ? field.TechnicalName 
                            : $"{field.NavigationPath!.Replace(".", "_")}_{field.TechnicalName}",
                        FriendlyName = field.FriendlyName,
                        DataType = field.FieldDataType?.TypeName
                    });
                }
            }

            // اضافه کردن ستون‌های collection navigation (داینامیک)
            foreach (var collectionNav in collectionNavigations)
            {
                var collectionFields = selectedFields
                    .Where(f => f.NavigationPath?.StartsWith(collectionNav.NavigationProperty) == true)
                    .ToList();

                foreach (var field in collectionFields)
                {
                    for (int rowNum = 1; rowNum <= collectionNav.MaxRows; rowNum++)
                    {
                        var columnAlias = $"{collectionNav.NavigationProperty.Replace(".", "_")}_{field.TechnicalName}_{rowNum}";
                        response.Columns.Add(new ReportColumnDTO
                        {
                            FieldId = field.Id,
                            TechnicalName = columnAlias,
                            FriendlyName = $"{field.FriendlyName} ({rowNum})",
                            DataType = field.FieldDataType?.TypeName
                        });
                    }
                }
            }

            // اضافه کردن ستون‌های InterdictOrderWageItem (اگر فعال باشد)
            if (interdictOrderWageItemInfo != null)
            {
                for (int rowNum = 1; rowNum <= interdictOrderWageItemInfo.MaxRows; rowNum++)
                {
                    var valueColumnName = $"InterdictOrderWageItem_Value_{rowNum}";
                    var titleColumnName = $"InterdictOrderWageItem_WageItem_Title_{rowNum}";
                    
                    // پیدا کردن عنوان WageItem از اولین رکورد داده
                    string wageItemTitle = "آیتم حکمی";
                    
                    if (data.Any())
                    {
                        // بررسی اینکه آیا این ستون در داده‌ها وجود دارد
                        var firstRowWithTitle = data.FirstOrDefault(row => 
                            row.ContainsKey(titleColumnName) && 
                            row[titleColumnName] != null && 
                            row[titleColumnName] != DBNull.Value &&
                            !string.IsNullOrWhiteSpace(row[titleColumnName]?.ToString()));
                        
                        if (firstRowWithTitle != null)
                        {
                            wageItemTitle = firstRowWithTitle[titleColumnName]!.ToString()!.Trim();
                        }
                        else
                        {
                            // اگر title وجود نداشت، بررسی می‌کنیم که آیا value column وجود دارد
                            var firstRowWithValue = data.FirstOrDefault(row => 
                                row.ContainsKey(valueColumnName));
                            
                            // اگر هیچکدام وجود نداشت، یعنی این ستون در query اضافه نشده است
                            if (firstRowWithValue == null)
                            {
                                continue; // این ستون را رد می‌کنیم
                            }
                        }
                    }
                    else
                    {
                        // اگر داده‌ای وجود نداشت، ستون را اضافه نمی‌کنیم
                        continue;
                    }
                    
                    // ستون را اضافه می‌کنیم
                    response.Columns.Add(new ReportColumnDTO
                    {
                        FieldId = 0,
                        TechnicalName = valueColumnName,
                        FriendlyName = wageItemTitle, // عنوان WageItem به عنوان هدر
                        DataType = "Int32"
                    });
                }
            }

            // اضافه کردن ستون‌های InterdictOrderCoefficientItem (اگر فعال باشد)
            if (interdictOrderCoefficientItemInfo != null)
            {
                for (int rowNum = 1; rowNum <= interdictOrderCoefficientItemInfo.MaxRows; rowNum++)
                {
                    var valueColumnName = $"InterdictOrderCoefficientItem_OutPutFactValue_{rowNum}";
                    var titleColumnName = $"InterdictOrderCoefficientItem_Coefficient_Title_{rowNum}";
                    
                    // پیدا کردن عنوان Coefficient از اولین رکورد داده
                    string coefficientTitle = "ضریب";
                    
                    if (data.Any())
                    {
                        // بررسی اینکه آیا این ستون در داده‌ها وجود دارد
                        var firstRowWithTitle = data.FirstOrDefault(row => 
                            row.ContainsKey(titleColumnName) && 
                            row[titleColumnName] != null && 
                            row[titleColumnName] != DBNull.Value &&
                            !string.IsNullOrWhiteSpace(row[titleColumnName]?.ToString()));
                        
                        if (firstRowWithTitle != null)
                        {
                            coefficientTitle = firstRowWithTitle[titleColumnName]!.ToString()!.Trim();
                        }
                        else
                        {
                            // اگر title وجود نداشت، بررسی می‌کنیم که آیا value column وجود دارد
                            var firstRowWithValue = data.FirstOrDefault(row => 
                                row.ContainsKey(valueColumnName));
                            
                            // اگر هیچکدام وجود نداشت، یعنی این ستون در query اضافه نشده است
                            if (firstRowWithValue == null)
                            {
                                continue; // این ستون را رد می‌کنیم
                            }
                        }
                    }
                    else
                    {
                        // اگر داده‌ای وجود نداشت، ستون را اضافه نمی‌کنیم
                        continue;
                    }
                    
                    // ستون را اضافه می‌کنیم
                    response.Columns.Add(new ReportColumnDTO
                    {
                        FieldId = 0,
                        TechnicalName = valueColumnName,
                        FriendlyName = coefficientTitle, // عنوان Coefficient به عنوان هدر
                        DataType = "Double"
                    });
                }
            }

            // اضافه کردن ستون‌های FicheItem (اگر فعال باشد)
            if (ficheItemInfo != null)
            {
                for (int rowNum = 1; rowNum <= ficheItemInfo.MaxRows; rowNum++)
                {
                    var valueColumnName = $"FicheItem_Value_{rowNum}";
                    var titleColumnName = $"FicheItem_WageItem_Title_{rowNum}";
                    
                    // بررسی اینکه آیا این ستون در داده‌ها مقداری دارد یا نه
                    // یک ستون خالی است اگر هم value و هم title در همه رکوردها null یا خالی باشند
                    bool hasData = false;
                    string wageItemTitle = "آیتم فیش";
                    
                    if (data.Any())
                    {
                        // بررسی اینکه آیا حداقل یک رکورد با title غیر خالی وجود دارد
                        var rowWithTitle = data.FirstOrDefault(row => 
                            row.ContainsKey(titleColumnName) && 
                            row[titleColumnName] != null && 
                            row[titleColumnName] != DBNull.Value &&
                            !string.IsNullOrWhiteSpace(row[titleColumnName]?.ToString()));
                        
                        if (rowWithTitle != null)
                        {
                            hasData = true;
                            wageItemTitle = rowWithTitle[titleColumnName]!.ToString()!.Trim();
                        }
                        else
                        {
                            // اگر title وجود نداشت یا خالی بود، بررسی می‌کنیم که آیا value غیر null وجود دارد
                            var rowWithValue = data.FirstOrDefault(row => 
                                row.ContainsKey(valueColumnName) && 
                                row[valueColumnName] != null && 
                                row[valueColumnName] != DBNull.Value);
                            
                            if (rowWithValue != null)
                            {
                                hasData = true;
                            }
                        }
                    }
                    
                    // فقط اگر داده‌ای وجود داشته باشد، ستون را اضافه می‌کنیم
                    if (hasData)
                    {
                        response.Columns.Add(new ReportColumnDTO
                        {
                            FieldId = 0,
                            TechnicalName = valueColumnName,
                            FriendlyName = wageItemTitle, // عنوان WageItem به عنوان هدر
                            DataType = "Double"
                        });
                    }
                }
            }

            // اضافه کردن ستون‌های PersonnelFunction (اگر فعال باشد)
            if (personnelFunctionInfo != null)
            {
                var personnelFunctionFields = new[]
                {
                    ("FunctionDay", "روزهای ماه", "Int32"),
                    ("PersonnelFunctionDay", "روزهای کارکرد", "Int32"),
                    ("PersonnelHourPresent", "ساعت حضور", "Int32"),
                    ("PersonnelNoEnter", "غیبت", "Decimal"),
                    ("PersonnelAbsenceDay", "روز غیبت", "Decimal"),
                    ("PersonnelIllnessDay", "روزهای استعلاجی", "Decimal"),
                    ("PersonnelMissionHours", "تعداد ساعت ماموریت", "Decimal"),
                    ("PersonnelOverTime", "ساعت اضافه کاری", "Decimal"),
                    ("PersonnelNightWork", "کار شبانه", "Decimal"),
                    ("PersonnelWorkingHolidayHours", "ساعت کار در تعطیل", "Decimal"),
                    ("RealFunctionDay", "روز کارکرد واقعی", "Decimal"),
                    ("HolidayFunctionDay", "روز کارکرد تعطیل", "Decimal"),
                    ("PersonnelMissionDay", "روز ماموریت", "Int32"),
                    ("PaylessDay", "روز بدون حقوق", "Int32"),
                    ("RewardsDay", "روز پاداش", "Int32"),
                    ("PersonnelNightWorkDay", "روز کار شبانه", "Decimal"),
                    ("PersonnelWorkingHolidaysDay", "روز کار در تعطیل", "Double"),
                    ("LinearFunctionDay", "روز کارکرد خطی", "Int64"),
                    ("PersonnelCeillingOvertime", "سقف اضافه کاری", "Decimal"),
                    ("PersonnelOverTimeFixed", "اضافه کاری ثابت", "Decimal"),
                    ("PersonnelHourlyWork", "کار ساعتی", "Decimal"),
                    ("PersonnelHourlyWorkMinutes", "دقیقه کار ساعتی", "Decimal"),
                    ("PaylessMinutes", "دقیقه بدون حقوق", "Decimal"),
                    ("Karaneh", "کارانه", "Decimal"),
                    ("PersonnelNightWorkMinutes", "دقیقه کار شبانه", "Decimal"),
                    ("BasijOverTime", "اضافه کاری بسیج", "Decimal"),
                    ("PersonnelWorkingHolidayMinutes", "دقیقه کار در تعطیل", "Decimal"),
                    ("FridayWorkHours", "جمعه کاری", "Decimal"),
                    ("FridayWorkAllowance", "فوق العاده جمعه کاری", "Decimal"),
                    ("NightWorkAllowance", "فوق العاده شبکاری", "Decimal")
                };
                
                foreach (var (fieldName, friendlyName, dataType) in personnelFunctionFields)
                {
                    var columnName = $"PersonnelFunction_{fieldName}";
                    
                    // بررسی اینکه آیا این ستون در داده‌ها مقداری دارد یا نه
                    bool hasData = false;
                    
                    if (data.Any())
                    {
                        var rowWithData = data.FirstOrDefault(row => 
                            row.ContainsKey(columnName) && 
                            row[columnName] != null && 
                            row[columnName] != DBNull.Value);
                        
                        if (rowWithData != null)
                        {
                            hasData = true;
                        }
                    }
                    
                    // فقط اگر داده‌ای وجود داشته باشد، ستون را اضافه می‌کنیم
                    if (hasData)
                    {
                        response.Columns.Add(new ReportColumnDTO
                        {
                            FieldId = 0,
                            TechnicalName = columnName,
                            FriendlyName = friendlyName,
                            DataType = dataType
                        });
                    }
                }
            }

            // اضافه کردن ستون‌های FicheLeaveItem (اگر فعال باشد)
            if (ficheLeaveItemInfo != null)
            {
                for (int rowNum = 1; rowNum <= ficheLeaveItemInfo.MaxRows; rowNum++)
                {
                    var leaveBalanceColumnName = $"FicheLeaveItem_LeaveBalanceTicks_{rowNum}";
                    var leaveAmountColumnName = $"FicheLeaveItem_LeaveAmountTicks_{rowNum}";
                    var leaveBalanceFormattedColumnName = $"FicheLeaveItem_LeaveBalanceFormatted_{rowNum}";
                    var leaveAmountFormattedColumnName = $"FicheLeaveItem_LeaveAmountFormatted_{rowNum}";
                    var titleColumnName = $"FicheLeaveItem_LeaveType_Title_{rowNum}";
                    
                    // بررسی اینکه آیا این ستون در داده‌ها مقداری دارد یا نه
                    bool hasData = false;
                    string leaveTypeTitle = "مرخصی";
                    
                    if (data.Any())
                    {
                        // بررسی اینکه آیا حداقل یک رکورد با title غیر خالی وجود دارد
                        var rowWithTitle = data.FirstOrDefault(row => 
                            row.ContainsKey(titleColumnName) && 
                            row[titleColumnName] != null && 
                            row[titleColumnName] != DBNull.Value &&
                            !string.IsNullOrWhiteSpace(row[titleColumnName]?.ToString()));
                        
                        if (rowWithTitle != null)
                        {
                            hasData = true;
                            leaveTypeTitle = rowWithTitle[titleColumnName]!.ToString()!.Trim();
                        }
                        else
                        {
                            // اگر title وجود نداشت یا خالی بود، بررسی می‌کنیم که آیا value غیر null وجود دارد
                            var rowWithValue = data.FirstOrDefault(row => 
                                (row.ContainsKey(leaveBalanceColumnName) && 
                                 row[leaveBalanceColumnName] != null && 
                                 row[leaveBalanceColumnName] != DBNull.Value) ||
                                (row.ContainsKey(leaveAmountColumnName) && 
                                 row[leaveAmountColumnName] != null && 
                                 row[leaveAmountColumnName] != DBNull.Value));
                            
                            if (rowWithValue != null)
                            {
                                hasData = true;
                            }
                        }
                    }
                    
                    // فقط اگر داده‌ای وجود داشته باشد، ستون را اضافه می‌کنیم
                    if (hasData)
                    {
                        // ستون مانده مرخصی (فرمت شده)
                        response.Columns.Add(new ReportColumnDTO
                        {
                            FieldId = 0,
                            TechnicalName = leaveBalanceFormattedColumnName,
                            FriendlyName = $"{leaveTypeTitle} - مانده",
                            DataType = "String"
                        });
                        
                        // ستون میزان مرخصی استفاده شده (فرمت شده)
                        response.Columns.Add(new ReportColumnDTO
                        {
                            FieldId = 0,
                            TechnicalName = leaveAmountFormattedColumnName,
                            FriendlyName = $"{leaveTypeTitle} - استفاده شده",
                            DataType = "String"
                        });
                    }
                }
            }

            return OperationResult.Succeeded(payload: response);
        }
        catch (SqlException sqlEx)
        {
            // Handle SQL timeout specifically
            if (sqlEx.Number == -2 || sqlEx.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) || sqlEx.Message.Contains("Timeout", StringComparison.OrdinalIgnoreCase))
            {
                return OperationResult.Failed("زمان اجرای گزارش به پایان رسید. لطفاً فیلترهای بیشتری اعمال کنید یا بعداً تلاش کنید.");
            }
            return OperationResult.Failed($"خطا در اجرای گزارش: {sqlEx.Message}");
        }
        catch (Exception ex)
        {
            // Handle general timeout errors
            if (ex.Message.Contains("timeout", StringComparison.OrdinalIgnoreCase) || ex.Message.Contains("Timeout", StringComparison.OrdinalIgnoreCase))
            {
                return OperationResult.Failed("زمان اجرای گزارش به پایان رسید. لطفاً فیلترهای بیشتری اعمال کنید یا بعداً تلاش کنید.");
            }
            return OperationResult.Failed($"خطا در اجرای گزارش: {ex.Message}");
        }
    }

    /// <summary>
    /// ساخت کوئری SQL برای GlobalDbContext
    /// </summary>
    private (string Query, Dictionary<string, object?> Parameters) BuildSqlQueryForGlobalContext(
        ReportableEntity entity,
        List<ReportableField> selectedFields,
        List<ReportableField> allFields,
        ReportBuilderRequestDTO request)
    {
        var parameters = new Dictionary<string, object?>();
        var sb = new StringBuilder();

        // Get all unique navigation paths and their foreign key info (from all fields including filters)
        var (singleNavigations, collectionNavigations) = GetNavigationInfoWithCollections(entity, allFields);

        // برای هر collection navigation، حداکثر تعداد رکورد را پیدا می‌کنیم
        foreach (var collectionNav in collectionNavigations)
        {
            var maxCount = GetMaxCollectionCount(collectionNav, entity, allFields, request);
            collectionNav.MaxRows = maxCount;
        }

        // اگر IncludeInterdictOrderWageItems فعال باشد و entity دارای InterdictOrderId باشد
        InterdictOrderWageItemInfo? interdictOrderWageItemInfo = null;
        if (request.IncludeInterdictOrderWageItems && HasInterdictOrderIdField(entity))
        {
            interdictOrderWageItemInfo = new InterdictOrderWageItemInfo
            {
                ParentAlias = entity.TableName,
                MaxRows = GetMaxInterdictOrderWageItemCount(entity, allFields, request)
            };
        }

        // اگر IncludeInterdictOrderCoefficientItems فعال باشد و entity دارای InterdictOrderId باشد
        InterdictOrderCoefficientItemInfo? interdictOrderCoefficientItemInfo = null;
        if (request.IncludeInterdictOrderCoefficientItems && HasInterdictOrderIdField(entity))
        {
            interdictOrderCoefficientItemInfo = new InterdictOrderCoefficientItemInfo
            {
                ParentAlias = entity.TableName,
                MaxRows = GetMaxInterdictOrderCoefficientItemCount(entity, allFields, request)
            };
        }

        // اگر IncludeFicheItems فعال باشد و entity دارای EmployeeId باشد
        FicheItemInfo? ficheItemInfo = null;
        if (request.IncludeFicheItems && HasEmployeeIdField(entity))
        {
            ficheItemInfo = new FicheItemInfo
            {
                ParentAlias = entity.TableName,
                MaxRows = GetMaxFicheItemCount(entity, allFields, request),
                PaymentPeriodYear = request.FichePaymentPeriodYear,
                PaymentPeriodMonth = request.FichePaymentPeriodMonth
            };
        }

        // اگر IncludePersonnelFunctionItems فعال باشد و entity دارای EmployeeId باشد
        PersonnelFunctionInfo? personnelFunctionInfo = null;
        if (request.IncludePersonnelFunctionItems && HasEmployeeIdField(entity))
        {
            personnelFunctionInfo = new PersonnelFunctionInfo
            {
                ParentAlias = entity.TableName,
                ParentKeyColumn = "EmployeeId",
                PaymentPeriodYear = request.FichePaymentPeriodYear,
                PaymentPeriodMonth = request.FichePaymentPeriodMonth
            };
        }

        // اگر IncludeFicheLeaveItems فعال باشد و entity دارای EmployeeId باشد
        FicheLeaveItemInfo? ficheLeaveItemInfo = null;
        if (request.IncludeFicheLeaveItems && HasEmployeeIdField(entity))
        {
            ficheLeaveItemInfo = new FicheLeaveItemInfo
            {
                ParentAlias = entity.TableName,
                MaxRows = GetMaxFicheLeaveItemCount(entity, allFields, request),
                PaymentPeriodYear = request.FichePaymentPeriodYear,
                PaymentPeriodMonth = request.FichePaymentPeriodMonth
            };
        }

        // Check if GroupBy is requested
        var hasGroupBy = request.GroupByFieldIds != null && request.GroupByFieldIds.Any();
        var groupByFields = hasGroupBy 
            ? allFields.Where(f => request.GroupByFieldIds.Contains(f.Id)).ToList()
            : new List<ReportableField>();

        // Debug logging for GroupBy
        if (hasGroupBy)
        {
            System.Diagnostics.Debug.WriteLine($"=== GroupBy Debug ===");
            System.Diagnostics.Debug.WriteLine($"GroupByFieldIds from request: {string.Join(", ", request.GroupByFieldIds)}");
            System.Diagnostics.Debug.WriteLine($"Total allFields count: {allFields.Count}");
            System.Diagnostics.Debug.WriteLine($"GroupBy fields found: {groupByFields.Count}");
            foreach (var gbf in groupByFields)
            {
                System.Diagnostics.Debug.WriteLine($"  - FieldId: {gbf.Id}, TechnicalName: {gbf.TechnicalName}, NavigationPath: {gbf.NavigationPath}");
            }
        }

        // SELECT clause
        sb.Append("SELECT ");
        
        // Helper method to get field expression and alias
        string GetFieldExpression(ReportableField f, out string columnAlias)
        {
            if (string.IsNullOrEmpty(f.NavigationPath))
            {
                columnAlias = f.TechnicalName;
                return $"[{entity.TableName}].[{f.TechnicalName}]";
            }
            else
            {
                var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == f.NavigationPath);
                if (navInfo != null)
                {
                    columnAlias = $"{f.NavigationPath!.Replace(".", "_")}_{f.TechnicalName}";
                    return $"[{navInfo.Alias}].[{f.TechnicalName}]";
                }
                else
                {
                    columnAlias = f.TechnicalName;
                    return $"[{entity.TableName}].[{f.TechnicalName}]";
                }
            }
        }

        var selectList = new List<string>();

        // If GroupBy is enabled, only include GroupBy fields in SELECT (without aggregation)
        // and add COUNT(*) for counting records in each group
        var groupByTitleColumns = new List<string>(); // Store Title columns for GROUP BY clause
        if (hasGroupBy)
        {
            System.Diagnostics.Debug.WriteLine($"=== Building SELECT for GroupBy ===");
            System.Diagnostics.Debug.WriteLine($"GroupBy fields count: {groupByFields.Count}");
            
            // Add GroupBy fields to SELECT
            foreach (var groupByField in groupByFields)
            {
                var fieldExpr = GetFieldExpression(groupByField, out string alias);
                var selectItem = $"{fieldExpr} AS [{alias}]";
                selectList.Add(selectItem);
                System.Diagnostics.Debug.WriteLine($"  Added to SELECT: {selectItem}");

                // Only add Title column if:
                // 1. Field has navigation path AND TechnicalName is NOT "Title" (to avoid duplicate)
                // 2. OR Field is FK (ends with Id) - then add Title from related table
                bool shouldAddTitle = false;
                string? titleAlias = null;
                string? titleSelectItem = null;
                string? titleGroupByExpression = null;

                if (!string.IsNullOrEmpty(groupByField.NavigationPath))
                {
                    // If TechnicalName is already "Title", don't add it again
                    if (groupByField.TechnicalName?.ToLower() != "title")
                    {
                        var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == groupByField.NavigationPath);
                        if (navInfo != null)
                        {
                            titleAlias = $"{groupByField.NavigationPath!.Replace(".", "_")}_Title";
                            titleSelectItem = $"[{navInfo.Alias}].[Title] AS [{titleAlias}]";
                            titleGroupByExpression = $"[{navInfo.Alias}].[Title]";
                            shouldAddTitle = true;
                        }
                    }
                }
                // If field is FK (ends with Id) and doesn't have navigation path, try to add Title from related table
                else if (!string.IsNullOrEmpty(groupByField.TechnicalName) && 
                         groupByField.TechnicalName.EndsWith("Id") && 
                         groupByField.TechnicalName.Length > 2)
                {
                    // Try to find navigation property for this FK
                    var navPropertyName = groupByField.TechnicalName.Substring(0, groupByField.TechnicalName.Length - 2);
                    var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == navPropertyName);
                    if (navInfo != null)
                    {
                        titleAlias = $"{navPropertyName}_Title";
                        titleSelectItem = $"[{navInfo.Alias}].[Title] AS [{titleAlias}]";
                        titleGroupByExpression = $"[{navInfo.Alias}].[Title]";
                        shouldAddTitle = true;
                    }
                }

                if (shouldAddTitle && !string.IsNullOrEmpty(titleSelectItem))
                {
                    selectList.Add(titleSelectItem);
                    if (!string.IsNullOrEmpty(titleGroupByExpression))
                    {
                        groupByTitleColumns.Add(titleGroupByExpression);
                    }
                    System.Diagnostics.Debug.WriteLine($"  Added Title to SELECT: {titleSelectItem}");
                }
            }

            // Add COUNT(DISTINCT Id) for counting unique records in each group
            // Using DISTINCT to avoid duplicate counting when JOINs are present
            selectList.Add($"COUNT(DISTINCT [{entity.TableName}].[Id]) AS [Count]");
            System.Diagnostics.Debug.WriteLine($"  Added COUNT(DISTINCT [{entity.TableName}].[Id]) AS [Count]");
        }
        else
        {
            // Normal SELECT: فیلدهای اصلی و single navigation
            var selectFields = selectedFields
                .Where(f => string.IsNullOrEmpty(f.NavigationPath) || 
                            !collectionNavigations.Any(cn => f.NavigationPath?.StartsWith(cn.NavigationProperty) == true))
                .Select(f =>
                {
                    var fieldExpr = GetFieldExpression(f, out string alias);
                    return $"{fieldExpr} AS [{alias}]";
                });
            
            selectList.AddRange(selectFields);
        }

        // اضافه کردن ستون‌های pivot برای collection navigation ها
        foreach (var collectionNav in collectionNavigations)
        {
            var collectionFields = selectedFields
                .Where(f => f.NavigationPath?.StartsWith(collectionNav.NavigationProperty) == true)
                .ToList();

            if (!collectionFields.Any()) continue;

            // ساخت ستون‌های pivot - تمام ستون‌ها را می‌سازیم (حتی اگر خالی باشند)
            var pivotColumns = BuildPivotColumns(collectionNav, collectionFields, entity.TableName);
            selectList.AddRange(pivotColumns);
        }

        // اضافه کردن ستون‌های InterdictOrderWageItem (اگر فعال باشد)
        if (interdictOrderWageItemInfo != null)
        {
            var interdictPivotColumns = BuildInterdictOrderWageItemPivotColumns(interdictOrderWageItemInfo);
            selectList.AddRange(interdictPivotColumns);
        }

        // اضافه کردن ستون‌های InterdictOrderCoefficientItem (اگر فعال باشد)
        if (interdictOrderCoefficientItemInfo != null)
        {
            var coefficientPivotColumns = BuildInterdictOrderCoefficientItemPivotColumns(interdictOrderCoefficientItemInfo);
            selectList.AddRange(coefficientPivotColumns);
        }

        // اضافه کردن ستون‌های FicheItem (اگر فعال باشد)
        if (ficheItemInfo != null)
        {
            var fichePivotColumns = BuildFicheItemPivotColumns(ficheItemInfo);
            selectList.AddRange(fichePivotColumns);
        }

        // اضافه کردن ستون‌های PersonnelFunction (اگر فعال باشد)
        if (personnelFunctionInfo != null)
        {
            var personnelFunctionColumns = BuildPersonnelFunctionColumns(personnelFunctionInfo);
            selectList.AddRange(personnelFunctionColumns);
        }

        // اضافه کردن ستون‌های FicheLeaveItem (اگر فعال باشد)
        if (ficheLeaveItemInfo != null)
        {
            var ficheLeavePivotColumns = BuildFicheLeaveItemPivotColumns(ficheLeaveItemInfo);
            selectList.AddRange(ficheLeavePivotColumns);
        }

        sb.Append(string.Join(", ", selectList));

        // FROM clause with NOLOCK hint for better read performance
        sb.Append($" FROM [{entity.Schema}].[{entity.TableName}] WITH (NOLOCK)");

        // JOIN clauses for single navigation properties (supporting multi-level)
        foreach (var nav in singleNavigations)
        {
            sb.Append($" LEFT JOIN [{nav.TargetSchema}].[{nav.TargetTable}] AS [{nav.Alias}] ON [{nav.ParentAlias}].[{nav.ForeignKeyColumn}] = [{nav.Alias}].[{nav.PrincipalKeyColumn}]");
        }

        // LEFT JOIN برای collection navigation ها (با استفاده از ROW_NUMBER)
        // برای هر collection، باید چندین JOIN ایجاد کنیم (یکی برای هر رکورد)
        foreach (var collectionNav in collectionNavigations)
        {
            var collectionFields = selectedFields
                .Where(f => f.NavigationPath?.StartsWith(collectionNav.NavigationProperty) == true)
                .Select(f => f.TechnicalName)
                .Distinct()
                .ToList();

            if (!collectionFields.Any()) continue;

            // برای هر رکورد (تا حداکثر MaxRows)، یک JOIN جداگانه ایجاد می‌کنیم
            for (int rowNum = 1; rowNum <= collectionNav.MaxRows; rowNum++)
            {
                var collectionAlias = $"{collectionNav.NavigationProperty.Replace(".", "_")}_Collection_{rowNum}";
                sb.Append($" LEFT JOIN (");
                sb.Append($"   SELECT ");
                sb.Append($"     [{collectionNav.ForeignKeyColumn}], ");
                sb.Append(string.Join(", ", collectionFields.Select(f => $"[{f}]")));
                sb.Append($"   FROM (");
                sb.Append($"     SELECT ");
                sb.Append($"       [{collectionNav.ForeignKeyColumn}], ");
                sb.Append($"       ROW_NUMBER() OVER (PARTITION BY [{collectionNav.ForeignKeyColumn}] ORDER BY Id) AS RowNum, ");
                sb.Append(string.Join(", ", collectionFields.Select(f => $"[{f}]")));
                sb.Append($"     FROM [{collectionNav.CollectionSchema}].[{collectionNav.CollectionTable}] WITH (NOLOCK)");
                sb.Append($"   ) AS Ranked");
                sb.Append($"   WHERE Ranked.RowNum = {rowNum}");
                sb.Append($") AS [{collectionAlias}] ON [{collectionNav.ParentAlias}].[{collectionNav.ParentKeyColumn}] = [{collectionAlias}].[{collectionNav.ForeignKeyColumn}]");
            }
        }

        // LEFT JOIN برای InterdictOrderWageItem (اگر فعال باشد)
        if (interdictOrderWageItemInfo != null)
        {
            for (int rowNum = 1; rowNum <= interdictOrderWageItemInfo.MaxRows; rowNum++)
            {
                var collectionAlias = $"InterdictOrderWageItem_{rowNum}";
                sb.Append($" LEFT JOIN (");
                sb.Append($"   SELECT ");
                sb.Append($"     IOWI.[{interdictOrderWageItemInfo.ForeignKeyColumn}], ");
                sb.Append($"     WI.[title] AS WageItem_Title, ");
                sb.Append($"     IOWI.[Value]");
                sb.Append($"   FROM (");
                sb.Append($"     SELECT ");
                sb.Append($"       [{interdictOrderWageItemInfo.ForeignKeyColumn}], ");
                sb.Append($"       [WageItemId], ");
                sb.Append($"       [Value], ");
                sb.Append($"       ROW_NUMBER() OVER (PARTITION BY [{interdictOrderWageItemInfo.ForeignKeyColumn}] ORDER BY Id) AS RowNum");
                sb.Append($"     FROM [{interdictOrderWageItemInfo.CollectionSchema}].[{interdictOrderWageItemInfo.CollectionTable}] WITH (NOLOCK)");
                sb.Append($"     WHERE [IsDeleted] = 0");
                sb.Append($"   ) AS IOWI");
                sb.Append($"   LEFT JOIN [bas].[Wage_Item] AS WI WITH (NOLOCK) ON IOWI.[WageItemId] = WI.[Id]");
                sb.Append($"   WHERE IOWI.RowNum = {rowNum}");
                sb.Append($") AS [{collectionAlias}] ON [{interdictOrderWageItemInfo.ParentAlias}].[{interdictOrderWageItemInfo.ParentKeyColumn}] = [{collectionAlias}].[{interdictOrderWageItemInfo.ForeignKeyColumn}]");
            }
        }

        // LEFT JOIN برای InterdictOrderCoefficientItem (اگر فعال باشد)
        if (interdictOrderCoefficientItemInfo != null)
        {
            for (int rowNum = 1; rowNum <= interdictOrderCoefficientItemInfo.MaxRows; rowNum++)
            {
                var collectionAlias = $"InterdictOrderCoefficientItem_{rowNum}";
                sb.Append($" LEFT JOIN (");
                sb.Append($"   SELECT ");
                sb.Append($"     IOCI.[{interdictOrderCoefficientItemInfo.ForeignKeyColumn}], ");
                sb.Append($"     C.[title] AS Coefficient_Title, ");
                sb.Append($"     IOCI.[OutPutFactValue]");
                sb.Append($"   FROM (");
                sb.Append($"     SELECT ");
                sb.Append($"       [{interdictOrderCoefficientItemInfo.ForeignKeyColumn}], ");
                sb.Append($"       [CoefficientId], ");
                sb.Append($"       [OutPutFactValue], ");
                sb.Append($"       ROW_NUMBER() OVER (PARTITION BY [{interdictOrderCoefficientItemInfo.ForeignKeyColumn}] ORDER BY Id) AS RowNum");
                sb.Append($"     FROM [{interdictOrderCoefficientItemInfo.CollectionSchema}].[{interdictOrderCoefficientItemInfo.CollectionTable}] WITH (NOLOCK)");
                sb.Append($"     WHERE [IsDeleted] = 0");
                sb.Append($"   ) AS IOCI");
                sb.Append($"   LEFT JOIN [bas].[Coefficient] AS C WITH (NOLOCK) ON IOCI.[CoefficientId] = C.[Id]");
                sb.Append($"   WHERE IOCI.RowNum = {rowNum}");
                sb.Append($") AS [{collectionAlias}] ON [{interdictOrderCoefficientItemInfo.ParentAlias}].[{interdictOrderCoefficientItemInfo.ParentKeyColumn}] = [{collectionAlias}].[{interdictOrderCoefficientItemInfo.ForeignKeyColumn}]");
            }
        }

        // LEFT JOIN برای FicheItem (اگر فعال باشد)
        if (ficheItemInfo != null)
        {
            for (int rowNum = 1; rowNum <= ficheItemInfo.MaxRows; rowNum++)
            {
                var collectionAlias = $"FicheItem_{rowNum}";
                sb.Append($" LEFT JOIN (");
                sb.Append($"   SELECT ");
                sb.Append($"     Ranked.[EmployeeId], ");
                sb.Append($"     WI.[title] AS WageItem_Title, ");
                sb.Append($"     Ranked.[Value]");
                sb.Append($"   FROM (");
                sb.Append($"     SELECT ");
                sb.Append($"       F.[EmployeeId], ");
                sb.Append($"       FI.[WageItemId], ");
                sb.Append($"       FI.[Value], ");
                sb.Append($"       ROW_NUMBER() OVER (PARTITION BY F.[EmployeeId] ORDER BY FI.Id) AS RowNum");
                sb.Append($"     FROM [Payroll].[Fiche] AS F");
                sb.Append($"     INNER JOIN [Payroll].[Fiche_Item] AS FI ON F.[Id] = FI.[FicheId]");
                sb.Append($"     INNER JOIN [Payroll].[Payment_Period] AS PP ON F.[PaymentPeriodId] = PP.[Id]");
                sb.Append($"     WHERE F.[IsDeleted] = 0 AND FI.[IsDeleted] = 0 AND PP.[IsDeleted] = 0");
                
                // اضافه کردن فیلتر سال و ماه
                if (ficheItemInfo.PaymentPeriodYear.HasValue && ficheItemInfo.PaymentPeriodMonth.HasValue)
                {
                    sb.Append($"       AND PP.[ShamsiYear] = {ficheItemInfo.PaymentPeriodYear.Value}");
                    sb.Append($"       AND PP.[ShamsiMonth] = {ficheItemInfo.PaymentPeriodMonth.Value}");
                }
                
                sb.Append($"   ) AS Ranked");
                sb.Append($"   LEFT JOIN [bas].[Wage_Item] AS WI WITH (NOLOCK) ON Ranked.[WageItemId] = WI.[Id]");
                sb.Append($"   WHERE Ranked.RowNum = {rowNum}");
                sb.Append($") AS [{collectionAlias}] ON [{ficheItemInfo.ParentAlias}].[{ficheItemInfo.ParentKeyColumn}] = [{collectionAlias}].[EmployeeId]");
            }
        }

        // LEFT JOIN برای FicheLeaveItem (اگر فعال باشد)
        if (ficheLeaveItemInfo != null)
        {
            for (int rowNum = 1; rowNum <= ficheLeaveItemInfo.MaxRows; rowNum++)
            {
                var collectionAlias = $"FicheLeaveItem_{rowNum}";
                sb.Append($" LEFT JOIN (");
                sb.Append($"   SELECT ");
                sb.Append($"     Ranked.[EmployeeId], ");
                sb.Append($"     LT.[title] AS LeaveType_Title, ");
                sb.Append($"     Ranked.[LeaveBalanceTicks], ");
                sb.Append($"     Ranked.[LeaveAmountTicks], ");
                sb.Append($"     CASE ");
                sb.Append($"       WHEN Ranked.[LeaveBalanceTicks] IS NULL OR Ranked.[LeaveBalanceTicks] = 0 THEN N'' ");
                sb.Append($"       ELSE CONCAT(");
                sb.Append($"         CAST(Ranked.[LeaveBalanceTicks] / CAST(864000000000 AS BIGINT) AS NVARCHAR), N' روز ', ");
                sb.Append($"         CAST((Ranked.[LeaveBalanceTicks] % CAST(864000000000 AS BIGINT)) / CAST(36000000000 AS BIGINT) AS NVARCHAR), N' ساعت ', ");
                sb.Append($"         CAST(((Ranked.[LeaveBalanceTicks] % CAST(864000000000 AS BIGINT)) % CAST(36000000000 AS BIGINT)) / CAST(600000000 AS BIGINT) AS NVARCHAR), N' دقیقه'");
                sb.Append($"       ) ");
                sb.Append($"     END AS LeaveBalanceFormatted, ");
                sb.Append($"     CASE ");
                sb.Append($"       WHEN Ranked.[LeaveAmountTicks] IS NULL OR Ranked.[LeaveAmountTicks] = 0 THEN N'' ");
                sb.Append($"       ELSE CONCAT(");
                sb.Append($"         CAST(Ranked.[LeaveAmountTicks] / CAST(864000000000 AS BIGINT) AS NVARCHAR), N' روز ', ");
                sb.Append($"         CAST((Ranked.[LeaveAmountTicks] % CAST(864000000000 AS BIGINT)) / CAST(36000000000 AS BIGINT) AS NVARCHAR), N' ساعت ', ");
                sb.Append($"         CAST(((Ranked.[LeaveAmountTicks] % CAST(864000000000 AS BIGINT)) % CAST(36000000000 AS BIGINT)) / CAST(600000000 AS BIGINT) AS NVARCHAR), N' دقیقه'");
                sb.Append($"       ) ");
                sb.Append($"     END AS LeaveAmountFormatted");
                sb.Append($"   FROM (");
                sb.Append($"     SELECT ");
                sb.Append($"       F.[EmployeeId], ");
                sb.Append($"       FLI.[LeaveTypeId], ");
                sb.Append($"       FLI.[LeaveBalanceTicks], ");
                sb.Append($"       FLI.[LeaveAmountTicks], ");
                sb.Append($"       ROW_NUMBER() OVER (PARTITION BY F.[EmployeeId] ORDER BY FLI.Id) AS RowNum");
                sb.Append($"     FROM [Payroll].[Fiche] AS F WITH (NOLOCK)");
                sb.Append($"     INNER JOIN [Payroll].[Fiche_Leave_Item] AS FLI WITH (NOLOCK) ON F.[Id] = FLI.[FicheId]");
                sb.Append($"     INNER JOIN [Payroll].[Payment_Period] AS PP WITH (NOLOCK) ON F.[PaymentPeriodId] = PP.[Id]");
                sb.Append($"     WHERE F.[IsDeleted] = 0 AND FLI.[IsDeleted] = 0 AND PP.[IsDeleted] = 0");
                
                // اضافه کردن فیلتر سال و ماه
                if (ficheLeaveItemInfo.PaymentPeriodYear.HasValue && ficheLeaveItemInfo.PaymentPeriodMonth.HasValue)
                {
                    sb.Append($"       AND PP.[ShamsiYear] = {ficheLeaveItemInfo.PaymentPeriodYear.Value}");
                    sb.Append($"       AND PP.[ShamsiMonth] = {ficheLeaveItemInfo.PaymentPeriodMonth.Value}");
                }
                
                sb.Append($"   ) AS Ranked");
                sb.Append($"   LEFT JOIN [bas].[LeaveType] AS LT WITH (NOLOCK) ON Ranked.[LeaveTypeId] = LT.[Id]");
                sb.Append($"   WHERE Ranked.RowNum = {rowNum}");
                sb.Append($") AS [{collectionAlias}] ON [{ficheLeaveItemInfo.ParentAlias}].[{ficheLeaveItemInfo.ParentKeyColumn}] = [{collectionAlias}].[EmployeeId]");
            }
        }

        // LEFT JOIN برای PersonnelFunction (اگر فعال باشد)
        if (personnelFunctionInfo != null)
        {
            var personnelFunctionAlias = "PersonnelFunction";
            sb.Append($" LEFT JOIN (");
            sb.Append($"   SELECT ");
            sb.Append($"     F.[EmployeeId], ");
            sb.Append($"     PF.[FunctionDay], ");
            sb.Append($"     PF.[PersonnelFunctionDay], ");
            sb.Append($"     PF.[PersonnelHourPresent], ");
            sb.Append($"     PF.[PersonnelNoEnter], ");
            sb.Append($"     PF.[PersonnelAbsenceDay], ");
            sb.Append($"     PF.[PersonnelIllnessDay], ");
            sb.Append($"     PF.[PersonnelMissionHours], ");
            sb.Append($"     PF.[PersonnelOverTime], ");
            sb.Append($"     PF.[PersonnelNightWork], ");
            sb.Append($"     PF.[PersonnelWorkingHolidayHours], ");
            sb.Append($"     PF.[RealFunctionDay], ");
            sb.Append($"     PF.[HolidayFunctionDay], ");
            sb.Append($"     PF.[PersonnelMissionDay], ");
            sb.Append($"     PF.[PaylessDay], ");
            sb.Append($"     PF.[RewardsDay], ");
            sb.Append($"     PF.[PersonnelNightWorkDay], ");
            sb.Append($"     PF.[PersonnelWorkingHolidaysDay], ");
            sb.Append($"     PF.[LinearFunctionDay], ");
            sb.Append($"     PF.[PersonnelCeillingOvertime], ");
            sb.Append($"     PF.[PersonnelOverTimeFixed], ");
            sb.Append($"     PF.[PersonnelHourlyWork], ");
            sb.Append($"     PF.[PersonnelHourlyWorkMinutes], ");
            sb.Append($"     PF.[PaylessMinutes], ");
            sb.Append($"     PF.[Karaneh], ");
            sb.Append($"     PF.[PersonnelNightWorkMinutes], ");
            sb.Append($"     PF.[BasijOverTime], ");
            sb.Append($"     PF.[PersonnelWorkingHolidayMinutes], ");
            sb.Append($"     PF.[FridayWorkHours], ");
            sb.Append($"     PF.[FridayWorkAllowance], ");
            sb.Append($"     PF.[NightWorkAllowance]");
            sb.Append($"   FROM [Payroll].[Fiche] AS F WITH (NOLOCK)");
            sb.Append($"   INNER JOIN [Payroll].[Personnel_Function] AS PF WITH (NOLOCK) ON F.[PersonnelFunctionId] = PF.[Id]");
            sb.Append($"   INNER JOIN [Payroll].[Payment_Period] AS PP WITH (NOLOCK) ON F.[PaymentPeriodId] = PP.[Id]");
            sb.Append($"   WHERE F.[IsDeleted] = 0 AND PF.[IsDeleted] = 0 AND PP.[IsDeleted] = 0");
            
            // اضافه کردن فیلتر سال و ماه
            if (personnelFunctionInfo.PaymentPeriodYear.HasValue && personnelFunctionInfo.PaymentPeriodMonth.HasValue)
            {
                sb.Append($"     AND PP.[ShamsiYear] = {personnelFunctionInfo.PaymentPeriodYear.Value}");
                sb.Append($"     AND PP.[ShamsiMonth] = {personnelFunctionInfo.PaymentPeriodMonth.Value}");
            }
            
            sb.Append($") AS [{personnelFunctionAlias}] ON [{personnelFunctionInfo.ParentAlias}].[{personnelFunctionInfo.ParentKeyColumn}] = [{personnelFunctionAlias}].[EmployeeId]");
        }

        // WHERE clause
        var whereClauses = new List<string>();

        // Add user access control clauses
        var userAccessClauses = BuildUserAccessWhereClause(entity);
        whereClauses.AddRange(userAccessClauses);

        // Add PayLocationIds filter if provided
        if (request.PayLocationIds != null && request.PayLocationIds.Any())
        {
            var payLocationFilter = BuildPayLocationFilterWhereClause(entity, request.PayLocationIds, hasGroupBy);
            if (!string.IsNullOrEmpty(payLocationFilter))
            {
                whereClauses.Add(payLocationFilter);
            }
        }

        // Add filters
        if (request.Filters != null && request.Filters.Any())
        {
            int paramIndex = 0;
            foreach (var filter in request.Filters)
            {
                var field = allFields.FirstOrDefault(f => f.Id == filter.FieldId);
                if (field == null)
                {
                    // Log warning if field not found
                    System.Diagnostics.Debug.WriteLine($"Filter field not found: FieldId={filter.FieldId}");
                    continue;
                }

                string fieldPath;
                // Check if field has BaseTableId (FK to BaseTableValue)
                if (field.BaseTableId.HasValue)
                {
                    // For BaseTableValue FK, use the TechnicalName directly (it's the FK column)
                    // The filter value is the BaseTableValue.Id that should match the FK column
                    fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                    System.Diagnostics.Debug.WriteLine($"BaseTableId filter: FieldId={filter.FieldId}, TechnicalName={field.TechnicalName}, BaseTableId={field.BaseTableId}, EntityTable={entity.TableName}, EntitySchema={entity.Schema}, FieldPath={fieldPath}, FilterValue={filter.Value}");
                }
                // Check if field is Long type and TechnicalName ends with "Id" (FK column like BloodGroupId)
                // Note: IsLongField checks the actual column type in the database table, not FieldDataType
                else if (IsLongField(field, entity) && !string.IsNullOrEmpty(field.TechnicalName) && field.TechnicalName.EndsWith("Id") && field.TechnicalName.Length > 2)
                {
                    // For Long FK columns, use the TechnicalName directly (it's the FK column in the main table)
                    fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                }
                else if (string.IsNullOrEmpty(field.NavigationPath))
                {
                    fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                }
                else
                {
                    var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == field.NavigationPath);
                    if (navInfo != null)
                    {
                        fieldPath = $"[{navInfo.Alias}].[{field.TechnicalName}]";
                    }
                    else
                    {
                        fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                    }
                }

                var operator_ = _unitOfWork.Context.Set<FieldOperator>()
                    .FirstOrDefault(o => o.Id == filter.OperatorId);

                if (operator_ == null)
                {
                    // Log warning if operator not found
                    System.Diagnostics.Debug.WriteLine($"Filter operator not found: OperatorId={filter.OperatorId}, FieldId={filter.FieldId}");
                    continue;
                }

                string whereClause = BuildWhereClause(fieldPath, operator_.Operator, filter, ref paramIndex, parameters);
                if (!string.IsNullOrEmpty(whereClause))
                {
                    whereClauses.Add(whereClause);
                    System.Diagnostics.Debug.WriteLine($"Filter applied: FieldId={filter.FieldId}, TechnicalName={field.TechnicalName}, FieldPath={fieldPath}, Operator={operator_.Operator}, Value={filter.Value}, WhereClause={whereClause}");
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine($"Filter whereClause is empty: FieldId={filter.FieldId}, Operator={operator_.Operator}, Value={filter.Value}");
                }
            }
        }

        if (whereClauses.Any())
        {
            sb.Append(" WHERE ");
            sb.Append(string.Join(" AND ", whereClauses));
        }

        // GROUP BY clause
        if (hasGroupBy && groupByFields.Any())
        {
            sb.Append(" GROUP BY ");
            var groupByClauses = groupByFields.Select(f =>
            {
                if (string.IsNullOrEmpty(f.NavigationPath))
                {
                    return $"[{entity.TableName}].[{f.TechnicalName}]";
                }
                else
                {
                    var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == f.NavigationPath);
                    if (navInfo != null)
                    {
                        return $"[{navInfo.Alias}].[{f.TechnicalName}]";
                    }
                    return $"[{entity.TableName}].[{f.TechnicalName}]";
                }
            }).ToList();
            
            // Add Title columns to GROUP BY clause (required for valid SQL when Title columns are in SELECT)
            if (groupByTitleColumns.Any())
            {
                groupByClauses.AddRange(groupByTitleColumns);
            }
            
            sb.Append(string.Join(", ", groupByClauses));
        }

        // ORDER BY clause
        if (request.Sorts != null && request.Sorts.Any())
        {
            sb.Append(" ORDER BY ");
            var orderClauses = request.Sorts.Select(s =>
            {
                var field = allFields.FirstOrDefault(f => f.Id == s.FieldId);
                if (field == null) return null;

                // When GroupBy is enabled, only allow sorting by GroupBy fields or aggregate functions
                if (hasGroupBy)
                {
                    // Check if this field is in the GroupBy fields
                    var isGroupByField = groupByFields.Any(gbf => gbf.Id == field.Id);
                    
                    // Allow COUNT(*) aggregate function
                    var isCountAggregate = field.TechnicalName?.ToLower() == "count" || 
                                          (field.TechnicalName == null && s.FieldId == 0);
                    
                    if (!isGroupByField && !isCountAggregate)
                    {
                        // Skip this sort - it's not a GroupBy field or aggregate
                        System.Diagnostics.Debug.WriteLine($"Skipping sort for non-GroupBy field: FieldId={s.FieldId}, TechnicalName={field.TechnicalName}");
                        return null;
                    }
                }

                string fieldPath;
                if (hasGroupBy && field.TechnicalName?.ToLower() == "count")
                {
                    // For COUNT aggregate, use the alias from SELECT
                    fieldPath = "[Count]";
                }
                else if (string.IsNullOrEmpty(field.NavigationPath))
                {
                    fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                }
                else
                {
                    var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == field.NavigationPath);
                    if (navInfo != null)
                    {
                        fieldPath = $"[{navInfo.Alias}].[{field.TechnicalName}]";
                    }
                    else
                    {
                        fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                    }
                }

                return $"{fieldPath} {(s.Direction.ToLower() == "desc" ? "DESC" : "ASC")}";
            }).Where(c => c != null).ToList();

            // If no valid order clauses remain, add default order by COUNT or first GroupBy field
            if (!orderClauses.Any() && hasGroupBy)
            {
                if (groupByFields.Any())
                {
                    var firstGroupByField = groupByFields.First();
                    string firstFieldPath;
                    if (string.IsNullOrEmpty(firstGroupByField.NavigationPath))
                    {
                        firstFieldPath = $"[{entity.TableName}].[{firstGroupByField.TechnicalName}]";
                    }
                    else
                    {
                        var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == firstGroupByField.NavigationPath);
                        if (navInfo != null)
                        {
                            firstFieldPath = $"[{navInfo.Alias}].[{firstGroupByField.TechnicalName}]";
                        }
                        else
                        {
                            firstFieldPath = $"[{entity.TableName}].[{firstGroupByField.TechnicalName}]";
                        }
                    }
                    orderClauses.Add($"{firstFieldPath} ASC");
                }
                else
                {
                    // Fallback to COUNT if no GroupBy fields
                    orderClauses.Add("[Count] DESC");
                }
            }

            sb.Append(string.Join(", ", orderClauses));
        }

        // PAGINATION
        if (request.PageSize > 0)
        {
            if (request.Sorts == null || !request.Sorts.Any())
            {
                // Add default order by for pagination
                sb.Append(" ORDER BY (SELECT NULL)");
            }
            sb.Append($" OFFSET {request.CurrentPage * request.PageSize} ROWS FETCH NEXT {request.PageSize} ROWS ONLY");
        }

        return (sb.ToString(), parameters);
    }

    /// <summary>
    /// دریافت اطلاعات navigation properties برای ساخت JOIN (با پشتیبانی از چند سطح و collection navigation)
    /// </summary>
    private (List<NavigationInfo> SingleNavigations, List<CollectionNavigationInfo> CollectionNavigations) 
        GetNavigationInfoWithCollections(ReportableEntity entity, List<ReportableField> fields)
    {
        var singleNavigations = new List<NavigationInfo>();
        var collectionNavigations = new List<CollectionNavigationInfo>();
        
        var uniqueNavigations = fields
            .Where(f => !string.IsNullOrEmpty(f.NavigationPath))
            .Select(f => f.NavigationPath)
            .Distinct()
            .ToList();

        if (!uniqueNavigations.Any())
            return (singleNavigations, collectionNavigations);

        // Get entity type from GlobalDbContext
        var entityType = _globalDbContext.Model.GetEntityTypes()
            .FirstOrDefault(e => e.GetTableName() == entity.TableName && e.GetSchema() == entity.Schema);

        if (entityType == null)
            return (singleNavigations, collectionNavigations);

        // Process each navigation path (including nested paths like "BirthPlace.PlaceType")
        foreach (var navPath in uniqueNavigations)
        {
            // Split the path by dots to handle nested navigations
            var pathParts = navPath!.Split('.');
            
            // Process each level of the navigation path
            var currentEntityType = entityType;
            var previousAlias = entity.TableName;
            var cumulativePath = "";

            for (int i = 0; i < pathParts.Length; i++)
            {
                var currentNavName = pathParts[i];
                cumulativePath = string.IsNullOrEmpty(cumulativePath) 
                    ? currentNavName 
                    : $"{cumulativePath}.{currentNavName}";

                // Check if we already added this level as single navigation
                var existingSingleNav = singleNavigations.FirstOrDefault(n => n.NavigationProperty == cumulativePath);
                if (existingSingleNav != null)
                {
                    previousAlias = existingSingleNav.Alias;
                    currentEntityType = _globalDbContext.Model.GetEntityTypes()
                        .FirstOrDefault(e => e.GetTableName() == existingSingleNav.TargetTable && 
                                           e.GetSchema() == existingSingleNav.TargetSchema);
                    continue;
                }

                // Check if we already added this level as collection navigation
                if (collectionNavigations.Any(cn => cn.NavigationProperty == cumulativePath))
                {
                    break; // Collection navigation already processed
                }

                var navigation = currentEntityType?.FindNavigation(currentNavName);
                
                if (navigation == null)
                {
                    break;
                }

                if (navigation.IsCollection)
                {
                    // این یک collection navigation است
                    var collectionEntityType = navigation.TargetEntityType;
                    var foreignKey = navigation.ForeignKey;
                    
                    var collectionFields = fields
                        .Where(f => f.NavigationPath == cumulativePath)
                        .Select(f => f.TechnicalName)
                        .Distinct()
                        .ToList();

                    var collectionNavInfo = new CollectionNavigationInfo
                    {
                        NavigationProperty = cumulativePath,
                        CollectionTable = collectionEntityType.GetTableName() ?? "",
                        CollectionSchema = collectionEntityType.GetSchema() ?? "dbo",
                        ForeignKeyColumn = foreignKey.Properties.First().Name,
                        ParentKeyColumn = foreignKey.PrincipalKey.Properties.First().Name,
                        ParentAlias = previousAlias,
                        SelectedFields = collectionFields
                    };
                    
                    collectionNavigations.Add(collectionNavInfo);
                    break; // بعد از collection navigation، ادامه نمی‌دهیم
                }
                else
                {
                    // navigation یک-به-یک
                    var foreignKey = navigation.ForeignKey;
                    var principalEntityType = foreignKey.PrincipalEntityType;

                    var navInfo = new NavigationInfo
                    {
                        NavigationProperty = cumulativePath,
                        ForeignKeyColumn = foreignKey.Properties.First().Name,
                        TargetTable = principalEntityType.GetTableName() ?? "",
                        TargetSchema = principalEntityType.GetSchema() ?? "dbo",
                        PrincipalKeyColumn = foreignKey.PrincipalKey.Properties.First().Name,
                        Alias = $"{cumulativePath.Replace(".", "_")}_Nav",
                        ParentAlias = previousAlias,
                        Level = i
                    };

                    singleNavigations.Add(navInfo);

                    // Update for next iteration
                    previousAlias = navInfo.Alias;
                    currentEntityType = principalEntityType;
                }
            }
        }

        // Sort single navigations by level to ensure correct JOIN order
        return (singleNavigations.OrderBy(n => n.Level).ToList(), collectionNavigations);
    }

    /// <summary>
    /// دریافت اطلاعات navigation properties برای ساخت JOIN (با پشتیبانی از چند سطح)
    /// </summary>
    private List<NavigationInfo> GetNavigationInfo(ReportableEntity entity, List<ReportableField> fields)
    {
        var navigationInfos = new List<NavigationInfo>();
        var uniqueNavigations = fields
            .Where(f => !string.IsNullOrEmpty(f.NavigationPath))
            .Select(f => f.NavigationPath)
            .Distinct()
            .ToList();

        if (!uniqueNavigations.Any())
            return navigationInfos;

        // Get entity type from GlobalDbContext
        var entityType = _globalDbContext.Model.GetEntityTypes()
            .FirstOrDefault(e => e.GetTableName() == entity.TableName && e.GetSchema() == entity.Schema);

        if (entityType == null)
            return navigationInfos;

        // Process each navigation path (including nested paths like "BirthPlace.PlaceType")
        foreach (var navPath in uniqueNavigations)
        {
            // Split the path by dots to handle nested navigations
            var pathParts = navPath!.Split('.');
            
            // Process each level of the navigation path
            var currentEntityType = entityType;
            var previousAlias = entity.TableName;
            var cumulativePath = "";

            for (int i = 0; i < pathParts.Length; i++)
            {
                var currentNavName = pathParts[i];
                cumulativePath = string.IsNullOrEmpty(cumulativePath) 
                    ? currentNavName 
                    : $"{cumulativePath}.{currentNavName}";

                // Check if we already added this level
                if (navigationInfos.Any(n => n.NavigationProperty == cumulativePath))
                {
                    // Update previous alias for next iteration
                    var existingNav = navigationInfos.First(n => n.NavigationProperty == cumulativePath);
                    previousAlias = existingNav.Alias;
                    currentEntityType = _globalDbContext.Model.GetEntityTypes()
                        .FirstOrDefault(e => e.GetTableName() == existingNav.TargetTable && 
                                           e.GetSchema() == existingNav.TargetSchema);
                    continue;
                }

                var navigation = currentEntityType?.FindNavigation(currentNavName);
                if (navigation != null && !navigation.IsCollection)
                {
                    var foreignKey = navigation.ForeignKey;
                    var principalEntityType = foreignKey.PrincipalEntityType;

                    var navInfo = new NavigationInfo
                    {
                        NavigationProperty = cumulativePath,
                        ForeignKeyColumn = foreignKey.Properties.First().Name,
                        TargetTable = principalEntityType.GetTableName() ?? "",
                        TargetSchema = principalEntityType.GetSchema() ?? "dbo",
                        PrincipalKeyColumn = foreignKey.PrincipalKey.Properties.First().Name,
                        Alias = $"{cumulativePath.Replace(".", "_")}_Nav",
                        ParentAlias = previousAlias,
                        Level = i
                    };

                    navigationInfos.Add(navInfo);

                    // Update for next iteration
                    previousAlias = navInfo.Alias;
                    currentEntityType = principalEntityType;
                }
                else
                {
                    // Navigation not found, skip this path
                    break;
                }
            }
        }

        // Sort by level to ensure correct JOIN order
        return navigationInfos.OrderBy(n => n.Level).ToList();
    }

    /// <summary>
    /// کلاس کمکی برای نگهداری اطلاعات navigation (با پشتیبانی از چند سطح)
    /// </summary>
    private class NavigationInfo
    {
        public string NavigationProperty { get; set; } = "";
        public string ForeignKeyColumn { get; set; } = "";
        public string TargetTable { get; set; } = "";
        public string TargetSchema { get; set; } = "";
        public string PrincipalKeyColumn { get; set; } = "";
        public string Alias { get; set; } = "";
        public string ParentAlias { get; set; } = ""; // For multi-level joins
        public int Level { get; set; } = 0; // Depth level of navigation
    }

    /// <summary>
    /// کلاس کمکی برای نگهداری اطلاعات collection navigation (یک به چند)
    /// </summary>
    private class CollectionNavigationInfo
    {
        public string NavigationProperty { get; set; } = "";
        public string CollectionTable { get; set; } = "";
        public string CollectionSchema { get; set; } = "";
        public string ForeignKeyColumn { get; set; } = ""; // در جدول collection
        public string ParentKeyColumn { get; set; } = ""; // در جدول parent
        public string ParentAlias { get; set; } = "";
        public List<string> SelectedFields { get; set; } = new(); // فیلدهایی که باید pivot شوند
        public int MaxRows { get; set; } = 100; // حداکثر تعداد ستون‌ها
    }

    /// <summary>
    /// کلاس کمکی برای نگهداری اطلاعات InterdictOrderWageItem
    /// </summary>
    private class InterdictOrderWageItemInfo
    {
        public string NavigationProperty { get; set; } = "InterdictOrderWageItems";
        public string CollectionTable { get; set; } = "Interdict_Order_WageItem";
        public string CollectionSchema { get; set; } = "Order";
        public string ForeignKeyColumn { get; set; } = "InterdictOrderId";
        public string ParentKeyColumn { get; set; } = "InterdictOrderId";
        public string ParentAlias { get; set; } = "";
        public int MaxRows { get; set; } = 100;
    }

    /// <summary>
    /// کلاس کمکی برای نگهداری اطلاعات InterdictOrderCoefficientItem
    /// </summary>
    private class InterdictOrderCoefficientItemInfo
    {
        public string NavigationProperty { get; set; } = "InterdictOrderCoefficientItems";
        public string CollectionTable { get; set; } = "Interdict_Order_CoefficientItem";
        public string CollectionSchema { get; set; } = "Order";
        public string ForeignKeyColumn { get; set; } = "InterdictOrderId";
        public string ParentKeyColumn { get; set; } = "InterdictOrderId";
        public string ParentAlias { get; set; } = "";
        public int MaxRows { get; set; } = 100;
    }

    /// <summary>
    /// کلاس کمکی برای نگهداری اطلاعات FicheItem
    /// </summary>
    private class FicheItemInfo
    {
        public string NavigationProperty { get; set; } = "FicheItems";
        public string CollectionTable { get; set; } = "Fiche_Item";
        public string CollectionSchema { get; set; } = "Payroll";
        public string ForeignKeyColumn { get; set; } = "EmployeeId";
        public string ParentKeyColumn { get; set; } = "EmployeeId";
        public string ParentAlias { get; set; } = "";
        public int MaxRows { get; set; } = 100;
        public int? PaymentPeriodYear { get; set; }
        public int? PaymentPeriodMonth { get; set; }
    }

    /// <summary>
    /// کلاس کمکی برای نگهداری اطلاعات PersonnelFunction
    /// </summary>
    private class PersonnelFunctionInfo
    {
        public string ParentAlias { get; set; } = "";
        public string ParentKeyColumn { get; set; } = "EmployeeId";
        public int? PaymentPeriodYear { get; set; }
        public int? PaymentPeriodMonth { get; set; }
    }

    /// <summary>
    /// کلاس کمکی برای نگهداری اطلاعات FicheLeaveItem
    /// </summary>
    private class FicheLeaveItemInfo
    {
        public string NavigationProperty { get; set; } = "FicheLeaveItems";
        public string CollectionTable { get; set; } = "Fiche_Leave_Item";
        public string CollectionSchema { get; set; } = "Payroll";
        public string ForeignKeyColumn { get; set; } = "EmployeeId";
        public string ParentKeyColumn { get; set; } = "EmployeeId";
        public string ParentAlias { get; set; } = "";
        public int MaxRows { get; set; } = 100;
        public int? PaymentPeriodYear { get; set; }
        public int? PaymentPeriodMonth { get; set; }
    }

    /// <summary>
    /// ساخت ستون‌های pivot برای collection navigation
    /// </summary>
    private List<string> BuildPivotColumns(
        CollectionNavigationInfo collectionNav, 
        List<ReportableField> fields, 
        string parentTableName)
    {
        var columns = new List<string>();
        var baseAlias = $"{collectionNav.NavigationProperty.Replace(".", "_")}_Collection";

        foreach (var field in fields)
        {
            for (int rowNum = 1; rowNum <= collectionNav.MaxRows; rowNum++)
            {
                var collectionAlias = $"{baseAlias}_{rowNum}";
                var columnAlias = $"{collectionNav.NavigationProperty.Replace(".", "_")}_{field.TechnicalName}_{rowNum}";
                var columnExpression = $"[{collectionAlias}].[{field.TechnicalName}] AS [{columnAlias}]";
                columns.Add(columnExpression);
            }
        }

        return columns;
    }

    /// <summary>
    /// پیدا کردن حداکثر تعداد رکورد در collection navigation برای تمام parent records
    /// </summary>
    private int GetMaxCollectionCount(
        CollectionNavigationInfo collectionNav,
        ReportableEntity entity,
        List<ReportableField> allFields,
        ReportBuilderRequestDTO request)
    {
        try
        {
            var connectionString = _globalDbContext.Database.GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // ساخت WHERE clause برای فیلترها
            var whereClauses = new List<string>();
            var parameters = new Dictionary<string, object?>();
            
            // Get single navigation info for filters
            var (singleNavigations, _) = GetNavigationInfoWithCollections(entity, allFields);
            
            // Add user access control clauses
            var userAccessClauses = BuildUserAccessWhereClause(entity);
            whereClauses.AddRange(userAccessClauses);

            // Add PayLocationIds filter if provided
            if (request.PayLocationIds != null && request.PayLocationIds.Any())
            {
                var hasGroupBy = request.GroupByFieldIds != null && request.GroupByFieldIds.Any();
                var payLocationFilter = BuildPayLocationFilterWhereClause(entity, request.PayLocationIds, hasGroupBy);
                if (!string.IsNullOrEmpty(payLocationFilter))
                {
                    whereClauses.Add(payLocationFilter);
                }
            }
            
            if (request.Filters != null && request.Filters.Any())
            {
                int paramIndex = 0;
                foreach (var filter in request.Filters)
                {
                    var field = allFields.FirstOrDefault(f => f.Id == filter.FieldId);
                    if (field == null) continue;

                    string fieldPath;
                    if (string.IsNullOrEmpty(field.NavigationPath))
                    {
                        fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                    }
                    else
                    {
                        var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == field.NavigationPath);
                        if (navInfo != null)
                        {
                            fieldPath = $"[{navInfo.Alias}].[{field.TechnicalName}]";
                        }
                        else
                        {
                            fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                        }
                    }

                    var operator_ = _unitOfWork.Context.Set<FieldOperator>()
                        .FirstOrDefault(o => o.Id == filter.OperatorId);

                    if (operator_ == null) continue;

                    string filterWhereClause = BuildWhereClause(fieldPath, operator_.Operator, filter, ref paramIndex, parameters);
                    if (!string.IsNullOrEmpty(filterWhereClause))
                    {
                        whereClauses.Add(filterWhereClause);
                    }
                }
            }

            var whereClause = whereClauses.Any() 
                ? "WHERE " + string.Join(" AND ", whereClauses) 
                : "";

            // JOIN clauses for single navigation properties with NOLOCK hint
            var joinClauses = new StringBuilder();
            foreach (var nav in singleNavigations)
            {
                joinClauses.Append($" LEFT JOIN [{nav.TargetSchema}].[{nav.TargetTable}] AS [{nav.Alias}] WITH (NOLOCK) ON [{nav.ParentAlias}].[{nav.ForeignKeyColumn}] = [{nav.Alias}].[{nav.PrincipalKeyColumn}]");
            }

            // کوئری برای پیدا کردن حداکثر تعداد رکورد در collection برای هر parent
            var query = $@"
                SELECT MAX(ItemCount) as MaxCount
                FROM (
                    SELECT 
                        [{entity.TableName}].Id,
                        COUNT([{collectionNav.CollectionTable}].Id) as ItemCount
                    FROM [{entity.Schema}].[{entity.TableName}] WITH (NOLOCK)
                    {joinClauses}
                    LEFT JOIN [{collectionNav.CollectionSchema}].[{collectionNav.CollectionTable}] WITH (NOLOCK)
                        ON [{entity.TableName}].Id = [{collectionNav.CollectionTable}].[{collectionNav.ForeignKeyColumn}]
                    {whereClause}
                    GROUP BY [{entity.TableName}].Id
                ) AS Counts";

            using var command = new SqlCommand(query, connection);
            command.CommandTimeout = 60; // Set timeout to 60 seconds
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }

            var result = command.ExecuteScalar();
            var maxCount = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            
            // محدود کردن به حداکثر 100
            return Math.Min(maxCount, 100);
        }
        catch
        {
            // در صورت خطا، مقدار پیش‌فرض 100 را برمی‌گردانیم
            return 100;
        }
    }

    /// <summary>
    /// تبدیل تاریخ میلادی به شمسی
    /// </summary>
    private string ConvertToPersianDate(DateTime dateTime)
    {
        try
        {
            var persianCalendar = new PersianCalendar();
            return $"{persianCalendar.GetYear(dateTime):0000}/{persianCalendar.GetMonth(dateTime):00}/{persianCalendar.GetDayOfMonth(dateTime):00}";
        }
        catch
        {
            return dateTime.ToString("yyyy/MM/dd");
        }
    }

    private sealed class UserAccessScope
    {
        public List<long> PayLocationIds { get; init; } = new();
        public List<long> CostCenterIds { get; init; } = new();
        public List<long> OrganizationUnitIds { get; init; } = new();
        public List<long> WorkPlaceIds { get; init; } = new();

        public bool HasAnyAccess =>
            PayLocationIds.Count > 0 ||
            CostCenterIds.Count > 0 ||
            OrganizationUnitIds.Count > 0 ||
            WorkPlaceIds.Count > 0;
    }

    private UserAccessScope GetUserAccessScope(long userId)
    {
        return new UserAccessScope
        {
            PayLocationIds = _globalDbContext.Set<GlobalDBContext.Models.UserPayLocation>()
                .Where(u => u.UserId == userId && !u.IsDeleted)
                .Select(u => u.PayLocationId)
                .Distinct()
                .ToList(),
            CostCenterIds = _globalDbContext.Set<GlobalDBContext.Models.UserCostCenter>()
                .Where(u => u.UserId == userId && !u.IsDeleted)
                .Select(u => u.CostCenterId)
                .Distinct()
                .ToList(),
            OrganizationUnitIds = _globalDbContext.Set<GlobalDBContext.Models.UserOrganizationUnit>()
                .Where(u => u.UserId == userId && !u.IsDeleted)
                .Select(u => u.OrganizationUnitId)
                .Distinct()
                .ToList(),
            WorkPlaceIds = _globalDbContext.Set<GlobalDBContext.Models.UserWorkPlace>()
                .Where(u => u.UserId == userId && !u.IsDeleted)
                .Select(u => u.WorkPlaceId)
                .Distinct()
                .ToList()
        };
    }

    private static string JoinIds(IEnumerable<long> ids) => string.Join(", ", ids);

    /// <summary>
    /// شرط دسترسی به پرسنل — هم‌تراز با EmployeeService.GetAccessibleEmployeesQueryable
    /// </summary>
    private string? BuildEmployeeAccessSqlCondition(string employeeAlias)
    {
        if (_userResolverService.IsAdmin())
            return null;

        var userId = _userResolverService.GetUserId();
        if (userId <= 0)
            return "1=0";

        var scope = GetUserAccessScope(userId);
        if (!scope.HasAnyAccess)
            return "1=0";

        var parts = new List<string>();
        var orderParts = new List<string>();

        if (scope.CostCenterIds.Count > 0)
            orderParts.Add($"ro.[CostCenterId] IN ({JoinIds(scope.CostCenterIds)})");
        if (scope.OrganizationUnitIds.Count > 0)
            orderParts.Add($"ro.[OrganizationUnitId] IN ({JoinIds(scope.OrganizationUnitIds)})");
        if (scope.PayLocationIds.Count > 0)
            orderParts.Add($"ro.[PayLocationId] IN ({JoinIds(scope.PayLocationIds)})");
        if (scope.WorkPlaceIds.Count > 0)
            orderParts.Add($"ro.[WorkPlaceId] IN ({JoinIds(scope.WorkPlaceIds)})");

        if (orderParts.Count > 0)
        {
            parts.Add($@"EXISTS (
                SELECT 1
                FROM [Order].[Recruit_Order] ro
                WHERE ro.[EmployeeId] = [{employeeAlias}].[Id]
                  AND ({string.Join(" OR ", orderParts)})
            )");
        }

        if (scope.PayLocationIds.Count > 0)
            parts.Add($"[{employeeAlias}].[BaseOrganisationId] IN ({JoinIds(scope.PayLocationIds)})");

        return parts.Count > 0 ? $"({string.Join(" OR ", parts)})" : "1=0";
    }

    /// <summary>
    /// ساخت عبارت WHERE برای چک دسترسی کاربر جاری
    /// </summary>
    private List<string> BuildUserAccessWhereClause(ReportableEntity entity)
    {
        var accessClauses = new List<string>();

        if (_userResolverService.IsAdmin())
            return accessClauses;

        var currentUserId = _userResolverService.GetUserId();
        if (currentUserId <= 0)
        {
            accessClauses.Add("1=0");
            return accessClauses;
        }

        // Employee_Property: محدودیت محل پرداخت و مرکز هزینه
        if (entity.TableName == "Employee_Property" && entity.Schema == "rpt")
        {
            var payLocationClause = $@"
                (NOT EXISTS (SELECT 1 FROM [Identity].[User_PayLocation] WHERE [UserId] = {currentUserId} AND [IsDeleted] = 0)
                 OR EXISTS (
                    SELECT 1
                    FROM [Identity].[User_PayLocation]
                    WHERE [UserId] = {currentUserId}
                      AND [IsDeleted] = 0
                      AND [PayLocationId] = [{entity.TableName}].[PayLocationId]
                 ))";
            accessClauses.Add(payLocationClause);

            var costCenterClause = $@"
                (NOT EXISTS (SELECT 1 FROM [Identity].[User_CostCenter] WHERE [UserId] = {currentUserId} AND [IsDeleted] = 0)
                 OR EXISTS (
                    SELECT 1
                    FROM [Identity].[User_CostCenter]
                    WHERE [UserId] = {currentUserId}
                      AND [IsDeleted] = 0
                      AND [CostCenterId] = [{entity.TableName}].[CostCenterId]
                 ))";
            accessClauses.Add(costCenterClause);

            return accessClauses;
        }

        // emp.Employee
        if (entity.TableName == "Employee" && entity.Schema == "emp")
        {
            var employeeAccess = BuildEmployeeAccessSqlCondition(entity.TableName);
            if (!string.IsNullOrEmpty(employeeAccess))
                accessClauses.Add(employeeAccess);

            return accessClauses;
        }

        // سایر جداول دارای EmployeeId
        if (HasEmployeeIdField(entity))
        {
            var employeeAccess = BuildEmployeeAccessSqlCondition("e");
            if (!string.IsNullOrEmpty(employeeAccess))
            {
                accessClauses.Add($@"EXISTS (
                    SELECT 1
                    FROM [emp].[Employee] e
                    WHERE e.[Id] = [{entity.TableName}].[EmployeeId]
                      AND e.[IsDeleted] = 0
                      AND {employeeAccess}
                )");
            }
        }

        return accessClauses;
    }

    /// <summary>
    /// اعتبارسنجی امنیتی: بررسی اینکه PayLocationIds ارسالی از کلاینت فقط شامل محل‌های پرداخت کاربر جاری باشد
    /// </summary>
    private OperationResult ValidatePayLocationIds(List<long> payLocationIds)
    {
        if (payLocationIds == null || !payLocationIds.Any())
            return OperationResult.Succeeded();

        var currentUserId = _userResolverService.GetUserId();
        if (currentUserId <= 0)
            return OperationResult.Failed("کاربر احراز هویت نشده است");

        try
        {
            // دریافت تمام PayLocationId های مجاز برای کاربر جاری از جدول UserPayLocation
            var allowedPayLocationIds = _globalDbContext.Set<HR.Report.Infrastructure.GlobalDBContext.Models.UserPayLocation>()
                .Where(upl => upl.UserId == currentUserId && !upl.IsDeleted)
                .Select(upl => upl.PayLocationId)
                .Distinct()
                .ToList();

            // بررسی اینکه آیا تمام PayLocationIds ارسالی در فهرست مجاز وجود دارد
            var invalidPayLocationIds = payLocationIds
                .Where(id => !allowedPayLocationIds.Contains(id))
                .ToList();

            if (invalidPayLocationIds.Any())
            {
                _logger.LogWarning(
                    "Security validation failed: User {UserId} attempted to use unauthorized PayLocationIds: {InvalidIds}. Allowed PayLocationIds: {AllowedIds}",
                    currentUserId,
                    string.Join(", ", invalidPayLocationIds),
                    string.Join(", ", allowedPayLocationIds)
                );
                return OperationResult.Failed($"دسترسی غیرمجاز: برخی از محل‌های پرداخت انتخاب شده مجاز نیستند");
            }

            return OperationResult.Succeeded();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating PayLocationIds for user {UserId}", currentUserId);
            return OperationResult.Failed("خطا در اعتبارسنجی محل‌های پرداخت");
        }
    }

    /// <summary>
    /// ساخت فیلتر WHERE برای PayLocationIds
    /// </summary>
    private string? BuildPayLocationFilterWhereClause(ReportableEntity entity, List<long> payLocationIds, bool hasGroupBy = false)
    {
        if (payLocationIds == null || !payLocationIds.Any())
            return null;

        var payLocationIdsString = string.Join(", ", payLocationIds);

        // 1- اگر جدول EmployeeProperty بود
        if (entity.TableName == "Employee_Property" && entity.Schema == "rpt")
        {
            return $"[{entity.TableName}].[PayLocationId] IN ({payLocationIdsString})";
        }

        // 2- اگر جدول Employee بود
        if (entity.TableName == "Employee" && entity.Schema == "emp")
        {
            // همیشه از BaseOrganisationId برای بررسی دسترسی استفاده می‌کنیم
            // منطق InterdictOrder نباید برای محدود کردن دسترسی در گزارش‌ساز استفاده شود
            return $"[{entity.TableName}].[BaseOrganisationId] IN ({payLocationIdsString})";
        }

        // 3- حالتی که جدول انتخاب شده ستونی به اسم OrganisationChartId دارد
        // بررسی اینکه آیا جدول ستون OrganisationChartId دارد (نه Employee و نه Employee_Property)
        // این بررسی فقط برای جداول دیگر انجام می‌شود
        // بررسی دقیق: اگر Employee یا Employee_Property باشد، این بخش اجرا نشود
        bool isEmployee = entity.TableName == "Employee" && entity.Schema == "emp";
        bool isEmployeeProperty = entity.TableName == "Employee_Property" && entity.Schema == "rpt";
        
        if (!isEmployee && !isEmployeeProperty)
        {
            try
            {
                var entityType = _globalDbContext.Model.GetEntityTypes()
                    .FirstOrDefault(e => e.GetTableName() == entity.TableName && e.GetSchema() == entity.Schema);

                if (entityType != null)
                {
                    // ابتدا بررسی می‌کنیم که آیا navigation property با این نام وجود دارد
                    // اگر navigation property وجود داشته باشد، نباید از آن به عنوان ستون استفاده کنیم
                    var navigation = entityType.FindNavigation("OrganisationChartId");
                    if (navigation != null)
                    {
                        // این یک navigation property است، نه یک ستون - پس نباید استفاده کنیم
                        return null;
                    }
                    
                    // حالا بررسی می‌کنیم که آیا property عادی با این نام وجود دارد
                    var organisationChartIdProperty = entityType.FindProperty("OrganisationChartId");
                    
                    if (organisationChartIdProperty != null)
                    {
                        // بررسی اینکه این property واقعاً به یک ستون در جدول map شده است
                        var columnName = organisationChartIdProperty.GetColumnName();
                        
                        // بررسی اینکه نام ستون با نام property یکسان است
                        // و اینکه این property در جدول اصلی map شده است (نه در یک جدول join شده)
                        if (!string.IsNullOrEmpty(columnName) && 
                            columnName == "OrganisationChartId")
                        {
                            // بررسی نهایی: اطمینان از اینکه این property در جدول اصلی است
                            // با بررسی اینکه GetTableName() با entity.TableName یکسان است
                            var propertyTableName = organisationChartIdProperty.DeclaringEntityType.GetTableName();
                            var propertySchema = organisationChartIdProperty.DeclaringEntityType.GetSchema();
                            
                            if (propertyTableName == entity.TableName && propertySchema == entity.Schema)
                            {
                                return $"[{entity.TableName}].[OrganisationChartId] IN ({payLocationIdsString})";
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // در صورت خطا، null برمی‌گردانیم (بدون اضافه کردن فیلتر)
                System.Diagnostics.Debug.WriteLine($"Error checking OrganisationChartId for {entity.TableName}: {ex.Message}");
            }
        }

        return null;
    }

    /// <summary>
    /// بررسی اینکه آیا فیلد در جدول اصلی از نوع Long است
    /// این متد نوع ستون را از جدول اصلی (مثل Employee) بررسی می‌کند، نه از FieldDataType
    /// </summary>
    private bool IsLongField(ReportableField field, ReportableEntity entity)
    {
        if (field == null || string.IsNullOrEmpty(field.TechnicalName) || entity == null)
            return false;

        try
        {
            // Find the entity type in GlobalDbContext
            var entityType = _globalDbContext.Model.GetEntityTypes()
                .FirstOrDefault(e => e.GetTableName() == entity.TableName && e.GetSchema() == entity.Schema);

            if (entityType == null)
                return false;

            // Find the property in the entity
            var property = entityType.FindProperty(field.TechnicalName);
            if (property == null)
                return false;

            // Check if the property type is long or nullable long
            var clrType = property.ClrType;
            var underlyingType = Nullable.GetUnderlyingType(clrType) ?? clrType;

            // Check if it's long, int64, or bigint
            return underlyingType == typeof(long) || 
                   underlyingType == typeof(Int64) ||
                   (underlyingType.IsPrimitive && underlyingType.Name.ToLower().Contains("int64"));
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// دریافت نام جدول مقصد برای FK fields
    /// برای مثال: BirthPlaceId -> Places
    /// </summary>
    private string? GetTargetTableName(ReportableField field, ReportableEntity entity)
    {
        if (field == null || string.IsNullOrEmpty(field.TechnicalName) || entity == null)
            return null;

        // اگر navigationPath وجود دارد، از آن استفاده می‌کنیم
        if (!string.IsNullOrEmpty(field.NavigationPath))
        {
            try
            {
                var entityType = _globalDbContext.Model.GetEntityTypes()
                    .FirstOrDefault(e => e.GetTableName() == entity.TableName && e.GetSchema() == entity.Schema);

                if (entityType == null)
                    return null;

                // Split navigation path (مثل "BirthPlace" یا "BirthPlace.PlaceType")
                var navPathParts = field.NavigationPath.Split('.');
                var currentEntityType = entityType;

                foreach (var navName in navPathParts)
                {
                    var navigation = currentEntityType?.FindNavigation(navName);
                    if (navigation != null && !navigation.IsCollection)
                    {
                        var foreignKey = navigation.ForeignKey;
                        var principalEntityType = foreignKey.PrincipalEntityType;
                        currentEntityType = principalEntityType;
                    }
                    else
                    {
                        return null;
                    }
                }

                // Return the final target table name
                return currentEntityType?.GetTableName();
            }
            catch
            {
                return null;
            }
        }

        // اگر navigationPath وجود ندارد، از TechnicalName استفاده می‌کنیم
        // برای مثال: BirthPlaceId -> BirthPlace -> Places
        if (IsLongField(field, entity) && 
            !string.IsNullOrEmpty(field.TechnicalName) && 
            field.TechnicalName.EndsWith("Id") && 
            field.TechnicalName.Length > 2)
        {
            try
            {
                var entityType = _globalDbContext.Model.GetEntityTypes()
                    .FirstOrDefault(e => e.GetTableName() == entity.TableName && e.GetSchema() == entity.Schema);

                if (entityType == null)
                    return null;

                // Extract navigation property name from TechnicalName
                // مثال: BirthPlaceId -> BirthPlace
                var navPropertyName = field.TechnicalName.Substring(0, field.TechnicalName.Length - 2);

                // Find navigation property
                var navigation = entityType.FindNavigation(navPropertyName);
                if (navigation != null && !navigation.IsCollection)
                {
                    var foreignKey = navigation.ForeignKey;
                    var principalEntityType = foreignKey.PrincipalEntityType;
                    return principalEntityType.GetTableName();
                }
            }
            catch
            {
                return null;
            }
        }

        return null;
    }

    /// <summary>
    /// ساخت عبارت WHERE
    /// </summary>
    private string BuildWhereClause(string fieldPath, string? operator_, ReportFilterDTO filter, ref int paramIndex, Dictionary<string, object?> parameters)
    {
        if (string.IsNullOrEmpty(operator_)) return string.Empty;

        var paramName = $"@p{paramIndex++}";
        
        // Try to convert value to appropriate type for numeric comparisons
        object? paramValue = filter.Value;
        if (!string.IsNullOrEmpty(filter.Value))
        {
            // Try to parse as long (for FK fields that are typically bigint)
            if (long.TryParse(filter.Value, out long longValue))
            {
                paramValue = longValue;
            }
            // Try to parse as int
            else if (int.TryParse(filter.Value, out int intValue))
            {
                paramValue = intValue;
            }
            // Try to parse as decimal
            else if (decimal.TryParse(filter.Value, out decimal decimalValue))
            {
                paramValue = decimalValue;
            }
            // Keep as string for other cases (dates, strings, etc.)
        }

        switch (operator_.ToLower())
        {
            case "=":
            case "equals":
                parameters[paramName] = paramValue;
                return $"{fieldPath} = {paramName}";

            case "!=":
            case "notequals":
                parameters[paramName] = paramValue;
                return $"{fieldPath} != {paramName}";

            case ">":
            case "greaterthan":
                parameters[paramName] = paramValue;
                return $"{fieldPath} > {paramName}";

            case ">=":
            case "greaterthanorequal":
                parameters[paramName] = paramValue;
                return $"{fieldPath} >= {paramName}";

            case "<":
            case "lessthan":
                parameters[paramName] = paramValue;
                return $"{fieldPath} < {paramName}";

            case "<=":
            case "lessthanorequal":
                parameters[paramName] = paramValue;
                return $"{fieldPath} <= {paramName}";

            case "contains":
            case "like":
                parameters[paramName] = $"%{filter.Value}%";
                return $"{fieldPath} LIKE {paramName}";

            case "startswith":
                parameters[paramName] = $"{filter.Value}%";
                return $"{fieldPath} LIKE {paramName}";

            case "endswith":
                parameters[paramName] = $"%{filter.Value}";
                return $"{fieldPath} LIKE {paramName}";

            case "between":
                var paramName2 = $"@p{paramIndex++}";
                // Try to convert value2 as well
                object? paramValue2 = filter.Value2;
                if (!string.IsNullOrEmpty(filter.Value2))
                {
                    if (long.TryParse(filter.Value2, out long longValue2))
                    {
                        paramValue2 = longValue2;
                    }
                    else if (int.TryParse(filter.Value2, out int intValue2))
                    {
                        paramValue2 = intValue2;
                    }
                    else if (decimal.TryParse(filter.Value2, out decimal decimalValue2))
                    {
                        paramValue2 = decimalValue2;
                    }
                }
                parameters[paramName] = paramValue;
                parameters[paramName2] = paramValue2;
                return $"{fieldPath} BETWEEN {paramName} AND {paramName2}";

            case "isnull":
                return $"{fieldPath} IS NULL";

            case "isnotnull":
                return $"{fieldPath} IS NOT NULL";

            case "in":
            case "containsany":
                // For IN operator, value should be a comma-separated list or array
                // Parse the value and create multiple parameters
                if (string.IsNullOrEmpty(filter.Value))
                    return string.Empty;
                
                var inValues = filter.Value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => v.Trim())
                    .Where(v => !string.IsNullOrEmpty(v))
                    .ToList();
                
                if (!inValues.Any())
                    return string.Empty;
                
                var inParams = new List<string>();
                foreach (var inValue in inValues)
                {
                    var inParamName = $"@p{paramIndex++}";
                    object? inParamValue = inValue;
                    
                    // Try to parse as long
                    if (long.TryParse(inValue, out long inLongValue))
                    {
                        inParamValue = inLongValue;
                    }
                    // Try to parse as int
                    else if (int.TryParse(inValue, out int inIntValue))
                    {
                        inParamValue = inIntValue;
                    }
                    // Try to parse as decimal
                    else if (decimal.TryParse(inValue, out decimal inDecimalValue))
                    {
                        inParamValue = inDecimalValue;
                    }
                    
                    parameters[inParamName] = inParamValue;
                    inParams.Add(inParamName);
                }
                
                return $"{fieldPath} IN ({string.Join(", ", inParams)})";

            case "notin":
            case "notcontainsany":
                // For NOT IN operator
                if (string.IsNullOrEmpty(filter.Value))
                    return string.Empty;
                
                var notInValues = filter.Value.Split(new[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(v => v.Trim())
                    .Where(v => !string.IsNullOrEmpty(v))
                    .ToList();
                
                if (!notInValues.Any())
                    return string.Empty;
                
                var notInParams = new List<string>();
                foreach (var notInValue in notInValues)
                {
                    var notInParamName = $"@p{paramIndex++}";
                    object? notInParamValue = notInValue;
                    
                    // Try to parse as long
                    if (long.TryParse(notInValue, out long notInLongValue))
                    {
                        notInParamValue = notInLongValue;
                    }
                    // Try to parse as int
                    else if (int.TryParse(notInValue, out int notInIntValue))
                    {
                        notInParamValue = notInIntValue;
                    }
                    // Try to parse as decimal
                    else if (decimal.TryParse(notInValue, out decimal notInDecimalValue))
                    {
                        notInParamValue = notInDecimalValue;
                    }
                    
                    parameters[notInParamName] = notInParamValue;
                    notInParams.Add(notInParamName);
                }
                
                return $"{fieldPath} NOT IN ({string.Join(", ", notInParams)})";

            default:
                System.Diagnostics.Debug.WriteLine($"Unknown operator: {operator_}");
                return string.Empty;
        }
    }

    /// <summary>
    /// ساخت کوئری شمارش
    /// </summary>
    private (string Query, Dictionary<string, object?> Parameters) BuildCountQuery(
        ReportableEntity entity,
        List<ReportableField> allFields,
        ReportBuilderRequestDTO request)
    {
        var parameters = new Dictionary<string, object?>();
        var sb = new StringBuilder();

        // Get navigation info (only single navigations for count query)
        var (singleNavigations, _) = GetNavigationInfoWithCollections(entity, allFields);

        sb.Append($"SELECT COUNT(DISTINCT [{entity.TableName}].Id) FROM [{entity.Schema}].[{entity.TableName}]");

        // JOIN clauses for single navigation properties (supporting multi-level)
        foreach (var nav in singleNavigations)
        {
            sb.Append($" LEFT JOIN [{nav.TargetSchema}].[{nav.TargetTable}] AS [{nav.Alias}] ON [{nav.ParentAlias}].[{nav.ForeignKeyColumn}] = [{nav.Alias}].[{nav.PrincipalKeyColumn}]");
        }

        // WHERE clause (same as main query)
        var whereClauses = new List<string>();

        // Add user access control clauses
        var userAccessClauses = BuildUserAccessWhereClause(entity);
        whereClauses.AddRange(userAccessClauses);

        // Add PayLocationIds filter if provided
        if (request.PayLocationIds != null && request.PayLocationIds.Any())
        {
            var hasGroupBy = request.GroupByFieldIds != null && request.GroupByFieldIds.Any();
            var payLocationFilter = BuildPayLocationFilterWhereClause(entity, request.PayLocationIds, hasGroupBy);
            if (!string.IsNullOrEmpty(payLocationFilter))
            {
                whereClauses.Add(payLocationFilter);
            }
        }

        if (request.Filters != null && request.Filters.Any())
        {
            int paramIndex = 0;
            foreach (var filter in request.Filters)
            {
                var field = allFields.FirstOrDefault(f => f.Id == filter.FieldId);
                if (field == null) continue;

                string fieldPath;
                // Check if field has BaseTableId (FK to BaseTableValue)
                if (field.BaseTableId.HasValue)
                {
                    // For BaseTableValue FK, use the TechnicalName directly (it's the FK column)
                    // The filter value is the BaseTableValue.Id that should match the FK column
                    fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                }
                // Check if field is Long type and TechnicalName ends with "Id" (FK column like BloodGroupId)
                // Note: IsLongField checks the actual column type in the database table, not FieldDataType
                else if (IsLongField(field, entity) && !string.IsNullOrEmpty(field.TechnicalName) && field.TechnicalName.EndsWith("Id") && field.TechnicalName.Length > 2)
                {
                    // For Long FK columns, use the TechnicalName directly (it's the FK column in the main table)
                    fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                }
                else if (string.IsNullOrEmpty(field.NavigationPath))
                {
                    fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                }
                else
                {
                    var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == field.NavigationPath);
                    if (navInfo != null)
                    {
                        fieldPath = $"[{navInfo.Alias}].[{field.TechnicalName}]";
                    }
                    else
                    {
                        fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                    }
                }

                var operator_ = _unitOfWork.Context.Set<FieldOperator>()
                    .FirstOrDefault(o => o.Id == filter.OperatorId);

                if (operator_ == null) continue;

                string whereClause = BuildWhereClause(fieldPath, operator_.Operator, filter, ref paramIndex, parameters);
                if (!string.IsNullOrEmpty(whereClause))
                {
                    whereClauses.Add(whereClause);
                }
            }
        }

        if (whereClauses.Any())
        {
            sb.Append(" WHERE ");
            sb.Append(string.Join(" AND ", whereClauses));
        }

        return (sb.ToString(), parameters);
    }

    /// <summary>
    /// دریافت داده‌های گزارش برای خروجی اکسل (بدون صفحه‌بندی)
    /// </summary>
    public OperationResult ExecuteReportForExport(ReportBuilderRequestDTO request)
    {
        // Remove pagination for export
        var exportRequest = new ReportBuilderRequestDTO
        {
            EntityId = request.EntityId,
            SelectedFieldIds = request.SelectedFieldIds,
            GroupByFieldIds = request.GroupByFieldIds,
            Filters = request.Filters,
            Sorts = request.Sorts,
            PageSize = 0, // No pagination
            CurrentPage = 0,
            IncludeInterdictOrderWageItems = request.IncludeInterdictOrderWageItems,
            IncludeInterdictOrderCoefficientItems = request.IncludeInterdictOrderCoefficientItems,
            IncludeFicheItems = request.IncludeFicheItems,
            FichePaymentPeriodYear = request.FichePaymentPeriodYear,
            FichePaymentPeriodMonth = request.FichePaymentPeriodMonth,
            IncludePersonnelFunctionItems = request.IncludePersonnelFunctionItems,
            IncludeFicheLeaveItems = request.IncludeFicheLeaveItems,
            PayLocationIds = request.PayLocationIds
        };

        return ExecuteReport(exportRequest);
    }

    /// <summary>
    /// ساخت فایل اکسل از داده‌های گزارش و ذخیره در TempGlobalFile
    /// </summary>
    public OperationResult ExportToExcel(ReportBuilderRequestDTO request, string entityName = "گزارش", string? encryptionPassword = null)
    {
        try
        {
            // اجرای گزارش برای دریافت داده‌ها
            var reportResult = ExecuteReportForExport(request);
            if (!reportResult.Success || reportResult.Payload == null)
            {
                return OperationResult.Failed("خطا در دریافت داده‌های گزارش");
            }

            var response = reportResult.Payload as ReportBuilderResponseDTO;
            if (response == null || response.Columns == null || response.Data == null)
            {
                return OperationResult.Failed("داده‌های گزارش یافت نشد");
            }

            // ساخت اکسل از داده‌ها
            var excelBytes = ExportReportDataToExcel(response);

            // رمزنگاری فایل در صورت وجود رمز عبور
            byte[] finalFileBytes = excelBytes;
            string fileExtension = ".xlsx";
            string mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            if (!string.IsNullOrWhiteSpace(encryptionPassword))
            {
                try
                {
                    finalFileBytes = HR.SharedKernel.Security.FileEncryptionService.EncryptFile(excelBytes, encryptionPassword);
                    fileExtension = ".hr";
                    mimeType = "application/octet-stream";
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در رمزنگاری فایل اکسل");
                    return OperationResult.Failed($"خطا در رمزنگاری فایل: {ex.Message}");
                }
            }

            // ذخیره در TempGlobalFile
            var tempFile = new TempGlobalFile
            {
                Content = finalFileBytes,
                CreateDate = DateTime.Now,
                IPAddress = _userResolverService.GetIP() ?? "Report",
                MimeType = mimeType,
                Extension = fileExtension,
                Size = finalFileBytes.LongLength,
                UniqueId = Guid.NewGuid(),
                title = $"{entityName}_{DateTime.Now:yyyyMMdd_HHmmss}",
                IsDeleted = false
            };

            _baseInfoUnitOfWork.Context.Set<TempGlobalFile>().Add(tempFile);
            _baseInfoUnitOfWork.Context.SaveChanges();

            return OperationResult.Succeeded(payload: tempFile.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ساخت فایل اکسل");
            return OperationResult.Failed($"خطا در ساخت فایل اکسل: {ex.Message}");
        }
    }

    /// <summary>
    /// تبدیل داده‌های گزارش به فایل اکسل
    /// </summary>
    private byte[] ExportReportDataToExcel(ReportBuilderResponseDTO response)
    {
        using var stream = new MemoryStream();
        using var workbook = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

        var workbookPart = workbook.AddWorkbookPart();
        workbook.WorkbookPart.Workbook = new Workbook();
        workbook.WorkbookPart.Workbook.Sheets = new Sheets();

        // ایجاد StyleSheet برای استایل‌دهی
        var workbookStylesPart = workbookPart.AddNewPart<WorkbookStylesPart>();
        workbookStylesPart.Stylesheet = CreateStylesheet();

        var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
        var sheetData = new SheetData();
        sheetPart.Worksheet = new Worksheet(sheetData);

        var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
        string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

        var sheet = new Sheet() { Id = relationshipId, SheetId = 1, Name = "گزارش" };
        sheets.Append(sheet);

        var columnCount = response.Columns.Count;
        
        // اضافه کردن ردیف برچسب اطلاعات فوق محرمانه با استایل
        var confidentialRow = new Row 
        { 
            RowIndex = 1,
            Height = 30, // ارتفاع ردیف
            CustomHeight = true
        };
        var confidentialCell = new Cell
        {
            DataType = CellValues.String,
            CellValue = new CellValue("اطلاعات فوق محرمانه"),
            StyleIndex = 1 // استفاده از استایل واترمارک
        };
        confidentialRow.AppendChild(confidentialCell);
        sheetData.AppendChild(confidentialRow);

        // ساخت هدر (ردیف دوم)
        var headerRow = new Row { RowIndex = 2 };
        foreach (var column in response.Columns)
        {
            var cell = new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(column.FriendlyName ?? column.TechnicalName ?? "")
            };
            headerRow.AppendChild(cell);
        }
        sheetData.AppendChild(headerRow);

        // merge سلول‌های ردیف اول برای نمایش واترمارک در تمام عرض جدول
        var mergeCells = new MergeCells();
        var lastColumnName = GetColumnName((uint)columnCount);
        var mergeCell = new MergeCell { Reference = $"A1:{lastColumnName}1" };
        mergeCells.Append(mergeCell);
        sheetPart.Worksheet.InsertAfter(mergeCells, sheetData);

        // ساخت ردیف‌های داده
        uint rowIndex = 3;
        foreach (var rowData in response.Data)
        {
            var row = new Row { RowIndex = rowIndex };
            foreach (var column in response.Columns)
            {
                var technicalName = column.TechnicalName ?? "";
                var value = rowData.ContainsKey(technicalName) ? rowData[technicalName] : null;
                
                var cell = new Cell
                {
                    DataType = CellValues.String,
                    CellValue = new CellValue(value?.ToString() ?? "")
                };
                row.AppendChild(cell);
            }
            sheetData.AppendChild(row);
            rowIndex++;
        }

        workbook.Save();
        return stream.ToArray();
    }

    /// <summary>
    /// ایجاد StyleSheet برای استایل‌دهی واترمارک
    /// </summary>
    private Stylesheet CreateStylesheet()
    {
        var stylesheet = new Stylesheet();

        // Fonts
        var fonts = new Fonts();
        fonts.Append(new Font()); // Default font (index 0)
        
        // Font برای واترمارک: Bold, بزرگ، قرمز
        var watermarkFont = new Font(
            new Bold(),
            new FontSize() { Val = 18 },
            new Color() { Rgb = "FFFF0000" } // قرمز
        );
        fonts.Append(watermarkFont);
        fonts.Count = (uint)fonts.ChildElements.Count;

        // Fills
        var fills = new Fills();
        fills.Append(new Fill(new PatternFill() { PatternType = PatternValues.None })); // Default fill (index 0)
        fills.Append(new Fill(new PatternFill() { PatternType = PatternValues.Gray125 })); // Default fill (index 1)
        
        // Fill برای واترمارک: پس‌زمینه زرد
        var watermarkPatternFill = new PatternFill
        {
            PatternType = PatternValues.Solid
        };
        watermarkPatternFill.Append(new ForegroundColor() { Rgb = "FFFFFF00" }); // زرد
        watermarkPatternFill.Append(new BackgroundColor() { Indexed = 64 });
        var watermarkFill = new Fill(watermarkPatternFill);
        fills.Append(watermarkFill);
        fills.Count = (uint)fills.ChildElements.Count;

        // Borders
        var borders = new Borders();
        borders.Append(new Border()); // Default border (index 0)
        borders.Count = (uint)borders.ChildElements.Count;

        // Cell Formats
        var cellFormats = new CellFormats();
        cellFormats.Append(new CellFormat()); // Default format (index 0)
        
        // Cell Format برای واترمارک
        var watermarkFormat = new CellFormat
        {
            FontId = 1,
            FillId = 2,
            BorderId = 0,
            Alignment = new Alignment
            {
                Horizontal = HorizontalAlignmentValues.Center,
                Vertical = VerticalAlignmentValues.Center
            },
            ApplyFont = true,
            ApplyFill = true,
            ApplyAlignment = true
        };
        cellFormats.Append(watermarkFormat);
        cellFormats.Count = (uint)cellFormats.ChildElements.Count;

        stylesheet.Fonts = fonts;
        stylesheet.Fills = fills;
        stylesheet.Borders = borders;
        stylesheet.CellFormats = cellFormats;

        return stylesheet;
    }

    /// <summary>
    /// تبدیل شماره ستون به نام ستون (1 -> A, 2 -> B, ...)
    /// </summary>
    private string GetColumnName(uint columnNumber)
    {
        uint dividend = columnNumber;
        string columnName = string.Empty;
        uint modulo;

        while (dividend > 0)
        {
            modulo = (dividend - 1) % 26;
            columnName = Convert.ToChar(65 + modulo).ToString() + columnName;
            dividend = (uint)((dividend - modulo) / 26);
        }

        return columnName;
    }

    /// <summary>
    /// بررسی اینکه آیا entity دارای فیلد InterdictOrderId است
    /// </summary>
    private bool HasInterdictOrderIdField(ReportableEntity entity)
    {
        try
        {
            var entityType = _globalDbContext.Model.GetEntityTypes()
                .FirstOrDefault(e => e.GetTableName() == entity.TableName && e.GetSchema() == entity.Schema);

            if (entityType == null)
                return false;

            var property = entityType.FindProperty("InterdictOrderId");
            return property != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// بررسی اینکه آیا entity دارای فیلد EmployeeId است
    /// </summary>
    private bool HasEmployeeIdField(ReportableEntity entity)
    {
        try
        {
            var entityType = _globalDbContext.Model.GetEntityTypes()
                .FirstOrDefault(e => e.GetTableName() == entity.TableName && e.GetSchema() == entity.Schema);

            if (entityType == null)
                return false;

            var property = entityType.FindProperty("EmployeeId");
            return property != null;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// پیدا کردن حداکثر تعداد InterdictOrderWageItem برای تمام parent records
    /// </summary>
    private int GetMaxInterdictOrderWageItemCount(
        ReportableEntity entity,
        List<ReportableField> allFields,
        ReportBuilderRequestDTO request)
    {
        try
        {
            var connectionString = _globalDbContext.Database.GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // ساخت WHERE clause برای فیلترها
            var whereClauses = new List<string>();
            var parameters = new Dictionary<string, object?>();
            
            // Get single navigation info for filters
            var (singleNavigations, _) = GetNavigationInfoWithCollections(entity, allFields);
            
            // Add user access control clauses
            var userAccessClauses = BuildUserAccessWhereClause(entity);
            whereClauses.AddRange(userAccessClauses);

            // Add PayLocationIds filter if provided
            if (request.PayLocationIds != null && request.PayLocationIds.Any())
            {
                var hasGroupBy = request.GroupByFieldIds != null && request.GroupByFieldIds.Any();
                var payLocationFilter = BuildPayLocationFilterWhereClause(entity, request.PayLocationIds, hasGroupBy);
                if (!string.IsNullOrEmpty(payLocationFilter))
                {
                    whereClauses.Add(payLocationFilter);
                }
            }
            
            if (request.Filters != null && request.Filters.Any())
            {
                int paramIndex = 0;
                foreach (var filter in request.Filters)
                {
                    var field = allFields.FirstOrDefault(f => f.Id == filter.FieldId);
                    if (field == null) continue;

                    string fieldPath;
                    if (string.IsNullOrEmpty(field.NavigationPath))
                    {
                        fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                    }
                    else
                    {
                        var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == field.NavigationPath);
                        if (navInfo != null)
                        {
                            fieldPath = $"[{navInfo.Alias}].[{field.TechnicalName}]";
                        }
                        else
                        {
                            fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                        }
                    }

                    var operator_ = _unitOfWork.Context.Set<FieldOperator>()
                        .FirstOrDefault(o => o.Id == filter.OperatorId);

                    if (operator_ == null) continue;

                    string filterWhereClause = BuildWhereClause(fieldPath, operator_.Operator, filter, ref paramIndex, parameters);
                    if (!string.IsNullOrEmpty(filterWhereClause))
                    {
                        whereClauses.Add(filterWhereClause);
                    }
                }
            }

            var whereClause = whereClauses.Any() 
                ? "WHERE " + string.Join(" AND ", whereClauses) 
                : "";

            // JOIN clauses for single navigation properties with NOLOCK hint
            var joinClauses = new StringBuilder();
            foreach (var nav in singleNavigations)
            {
                joinClauses.Append($" LEFT JOIN [{nav.TargetSchema}].[{nav.TargetTable}] AS [{nav.Alias}] WITH (NOLOCK) ON [{nav.ParentAlias}].[{nav.ForeignKeyColumn}] = [{nav.Alias}].[{nav.PrincipalKeyColumn}]");
            }

            // کوئری برای پیدا کردن حداکثر تعداد رکورد در InterdictOrderWageItem برای هر parent
            var query = $@"
                SELECT MAX(ItemCount) as MaxCount
                FROM (
                    SELECT 
                        [{entity.TableName}].[InterdictOrderId],
                        COUNT(IOWI.Id) as ItemCount
                    FROM [{entity.Schema}].[{entity.TableName}] WITH (NOLOCK)
                    {joinClauses}
                    LEFT JOIN [Order].[Interdict_Order_WageItem] AS IOWI WITH (NOLOCK)
                        ON [{entity.TableName}].[InterdictOrderId] = IOWI.[InterdictOrderId]
                        AND IOWI.[IsDeleted] = 0
                    {whereClause}
                    GROUP BY [{entity.TableName}].[InterdictOrderId]
                ) AS Counts";

            using var command = new SqlCommand(query, connection);
            command.CommandTimeout = 60; // Set timeout to 60 seconds
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }

            var result = command.ExecuteScalar();
            var maxCount = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            
            // محدود کردن به حداکثر 50
            return Math.Min(maxCount, 50);
        }
        catch
        {
            // در صورت خطا، مقدار پیش‌فرض 0 را برمی‌گردانیم
            return 0;
        }
    }

    /// <summary>
    /// ساخت ستون‌های pivot برای InterdictOrderWageItem
    /// </summary>
    private List<string> BuildInterdictOrderWageItemPivotColumns(InterdictOrderWageItemInfo info)
    {
        var columns = new List<string>();

        for (int rowNum = 1; rowNum <= info.MaxRows; rowNum++)
        {
            var collectionAlias = $"InterdictOrderWageItem_{rowNum}";
            
            // ستون Value با alias که شامل عنوان WageItem است
            columns.Add($"[{collectionAlias}].[Value] AS [InterdictOrderWageItem_Value_{rowNum}]");
            
            // ستون WageItem.Title برای استفاده در هدر
            columns.Add($"[{collectionAlias}].[WageItem_Title] AS [InterdictOrderWageItem_WageItem_Title_{rowNum}]");
        }

        return columns;
    }

    /// <summary>
    /// پیدا کردن حداکثر تعداد InterdictOrderCoefficientItem برای تمام parent records
    /// </summary>
    private int GetMaxInterdictOrderCoefficientItemCount(
        ReportableEntity entity,
        List<ReportableField> allFields,
        ReportBuilderRequestDTO request)
    {
        try
        {
            var connectionString = _globalDbContext.Database.GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // ساخت WHERE clause برای فیلترها
            var whereClauses = new List<string>();
            var parameters = new Dictionary<string, object?>();
            
            // Get single navigation info for filters
            var (singleNavigations, _) = GetNavigationInfoWithCollections(entity, allFields);
            
            // Add user access control clauses
            var userAccessClauses = BuildUserAccessWhereClause(entity);
            whereClauses.AddRange(userAccessClauses);

            // Add PayLocationIds filter if provided
            if (request.PayLocationIds != null && request.PayLocationIds.Any())
            {
                var hasGroupBy = request.GroupByFieldIds != null && request.GroupByFieldIds.Any();
                var payLocationFilter = BuildPayLocationFilterWhereClause(entity, request.PayLocationIds, hasGroupBy);
                if (!string.IsNullOrEmpty(payLocationFilter))
                {
                    whereClauses.Add(payLocationFilter);
                }
            }
            
            if (request.Filters != null && request.Filters.Any())
            {
                int paramIndex = 0;
                foreach (var filter in request.Filters)
                {
                    var field = allFields.FirstOrDefault(f => f.Id == filter.FieldId);
                    if (field == null) continue;

                    string fieldPath;
                    if (string.IsNullOrEmpty(field.NavigationPath))
                    {
                        fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                    }
                    else
                    {
                        var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == field.NavigationPath);
                        if (navInfo != null)
                        {
                            fieldPath = $"[{navInfo.Alias}].[{field.TechnicalName}]";
                        }
                        else
                        {
                            fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                        }
                    }

                    var operator_ = _unitOfWork.Context.Set<FieldOperator>()
                        .FirstOrDefault(o => o.Id == filter.OperatorId);

                    if (operator_ == null) continue;

                    string filterWhereClause = BuildWhereClause(fieldPath, operator_.Operator, filter, ref paramIndex, parameters);
                    if (!string.IsNullOrEmpty(filterWhereClause))
                    {
                        whereClauses.Add(filterWhereClause);
                    }
                }
            }

            var whereClause = whereClauses.Any() 
                ? "WHERE " + string.Join(" AND ", whereClauses) 
                : "";

            // JOIN clauses for single navigation properties with NOLOCK hint
            var joinClauses = new StringBuilder();
            foreach (var nav in singleNavigations)
            {
                joinClauses.Append($" LEFT JOIN [{nav.TargetSchema}].[{nav.TargetTable}] AS [{nav.Alias}] WITH (NOLOCK) ON [{nav.ParentAlias}].[{nav.ForeignKeyColumn}] = [{nav.Alias}].[{nav.PrincipalKeyColumn}]");
            }

            // کوئری برای پیدا کردن حداکثر تعداد رکورد در InterdictOrderCoefficientItem برای هر parent
            var query = $@"
                SELECT MAX(ItemCount) as MaxCount
                FROM (
                    SELECT 
                        [{entity.TableName}].[InterdictOrderId],
                        COUNT(IOCI.Id) as ItemCount
                    FROM [{entity.Schema}].[{entity.TableName}] WITH (NOLOCK)
                    {joinClauses}
                    LEFT JOIN [Order].[Interdict_Order_CoefficientItem] AS IOCI WITH (NOLOCK)
                        ON [{entity.TableName}].[InterdictOrderId] = IOCI.[InterdictOrderId]
                        AND IOCI.[IsDeleted] = 0
                    {whereClause}
                    GROUP BY [{entity.TableName}].[InterdictOrderId]
                ) AS Counts";

            using var command = new SqlCommand(query, connection);
            command.CommandTimeout = 60; // Set timeout to 60 seconds
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }

            var result = command.ExecuteScalar();
            var maxCount = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            
            // محدود کردن به حداکثر 50
            return Math.Min(maxCount, 50);
        }
        catch
        {
            // در صورت خطا، مقدار پیش‌فرض 0 را برمی‌گردانیم
            return 0;
        }
    }

    /// <summary>
    /// ساخت ستون‌های pivot برای InterdictOrderCoefficientItem
    /// </summary>
    private List<string> BuildInterdictOrderCoefficientItemPivotColumns(InterdictOrderCoefficientItemInfo info)
    {
        var columns = new List<string>();

        for (int rowNum = 1; rowNum <= info.MaxRows; rowNum++)
        {
            var collectionAlias = $"InterdictOrderCoefficientItem_{rowNum}";
            
            // ستون OutPutFactValue با alias که شامل عنوان Coefficient است
            columns.Add($"[{collectionAlias}].[OutPutFactValue] AS [InterdictOrderCoefficientItem_OutPutFactValue_{rowNum}]");
            
            // ستون Coefficient.Title برای استفاده در هدر
            columns.Add($"[{collectionAlias}].[Coefficient_Title] AS [InterdictOrderCoefficientItem_Coefficient_Title_{rowNum}]");
        }

        return columns;
    }

    /// <summary>
    /// پیدا کردن حداکثر تعداد FicheItem برای تمام parent records
    /// </summary>
    private int GetMaxFicheItemCount(
        ReportableEntity entity,
        List<ReportableField> allFields,
        ReportBuilderRequestDTO request)
    {
        try
        {
            var connectionString = _globalDbContext.Database.GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // ساخت WHERE clause برای فیلترها
            var whereClauses = new List<string>();
            var parameters = new Dictionary<string, object?>();
            
            // Get single navigation info for filters
            var (singleNavigations, _) = GetNavigationInfoWithCollections(entity, allFields);
            
            // Add user access control clauses
            var userAccessClauses = BuildUserAccessWhereClause(entity);
            whereClauses.AddRange(userAccessClauses);

            // Add PayLocationIds filter if provided
            if (request.PayLocationIds != null && request.PayLocationIds.Any())
            {
                var hasGroupBy = request.GroupByFieldIds != null && request.GroupByFieldIds.Any();
                var payLocationFilter = BuildPayLocationFilterWhereClause(entity, request.PayLocationIds, hasGroupBy);
                if (!string.IsNullOrEmpty(payLocationFilter))
                {
                    whereClauses.Add(payLocationFilter);
                }
            }
            
            if (request.Filters != null && request.Filters.Any())
            {
                int paramIndex = 0;
                foreach (var filter in request.Filters)
                {
                    var field = allFields.FirstOrDefault(f => f.Id == filter.FieldId);
                    if (field == null) continue;

                    string fieldPath;
                    if (string.IsNullOrEmpty(field.NavigationPath))
                    {
                        fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                    }
                    else
                    {
                        var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == field.NavigationPath);
                        if (navInfo != null)
                        {
                            fieldPath = $"[{navInfo.Alias}].[{field.TechnicalName}]";
                        }
                        else
                        {
                            fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                        }
                    }

                    var operator_ = _unitOfWork.Context.Set<FieldOperator>()
                        .FirstOrDefault(o => o.Id == filter.OperatorId);

                    if (operator_ == null) continue;

                    string filterWhereClause = BuildWhereClause(fieldPath, operator_.Operator, filter, ref paramIndex, parameters);
                    if (!string.IsNullOrEmpty(filterWhereClause))
                    {
                        whereClauses.Add(filterWhereClause);
                    }
                }
            }

            var whereClause = whereClauses.Any() 
                ? "WHERE " + string.Join(" AND ", whereClauses) 
                : "";

            // JOIN clauses for single navigation properties with NOLOCK hint
            var joinClauses = new StringBuilder();
            foreach (var nav in singleNavigations)
            {
                joinClauses.Append($" LEFT JOIN [{nav.TargetSchema}].[{nav.TargetTable}] AS [{nav.Alias}] WITH (NOLOCK) ON [{nav.ParentAlias}].[{nav.ForeignKeyColumn}] = [{nav.Alias}].[{nav.PrincipalKeyColumn}]");
            }

            // کوئری برای پیدا کردن حداکثر تعداد رکورد در FicheItem برای هر parent
            var ficheWhereClause = "";
            if (request.FichePaymentPeriodYear.HasValue && request.FichePaymentPeriodMonth.HasValue)
            {
                ficheWhereClause = $@"
                    AND EXISTS (
                        SELECT 1 FROM [Payroll].[Fiche] AS F
                        INNER JOIN [Payroll].[Payment_Period] AS PP ON F.[PaymentPeriodId] = PP.[Id]
                        WHERE F.[Id] = FI.[FicheId]
                        AND PP.[ShamsiYear] = {request.FichePaymentPeriodYear.Value}
                        AND PP.[ShamsiMonth] = {request.FichePaymentPeriodMonth.Value}
                        AND F.[IsDeleted] = 0
                        AND PP.[IsDeleted] = 0
                    )";
            }

            var query = $@"
                SELECT MAX(ItemCount) as MaxCount
                FROM (
                    SELECT 
                        [{entity.TableName}].[EmployeeId],
                        COUNT(FI.Id) as ItemCount
                    FROM [{entity.Schema}].[{entity.TableName}] WITH (NOLOCK)
                    {joinClauses}
                    LEFT JOIN [Payroll].[Fiche] AS F_Main WITH (NOLOCK)
                        ON [{entity.TableName}].[EmployeeId] = F_Main.[EmployeeId]
                        AND F_Main.[IsDeleted] = 0
                    LEFT JOIN [Payroll].[Payment_Period] AS PP_Main WITH (NOLOCK)
                        ON F_Main.[PaymentPeriodId] = PP_Main.[Id]
                        AND PP_Main.[IsDeleted] = 0
                        {(request.FichePaymentPeriodYear.HasValue && request.FichePaymentPeriodMonth.HasValue 
                            ? $"AND PP_Main.[ShamsiYear] = {request.FichePaymentPeriodYear.Value} AND PP_Main.[ShamsiMonth] = {request.FichePaymentPeriodMonth.Value}" 
                            : "")}
                    LEFT JOIN [Payroll].[Fiche_Item] AS FI WITH (NOLOCK)
                        ON F_Main.[Id] = FI.[FicheId]
                        AND FI.[IsDeleted] = 0
                    {whereClause}
                    GROUP BY [{entity.TableName}].[EmployeeId]
                ) AS Counts";

            using var command = new SqlCommand(query, connection);
            command.CommandTimeout = 60; // Set timeout to 60 seconds
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }

            var result = command.ExecuteScalar();
            var maxCount = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            
            // محدود کردن به حداکثر 50
            return Math.Min(maxCount, 50);
        }
        catch
        {
            // در صورت خطا، مقدار پیش‌فرض 0 را برمی‌گردانیم
            return 0;
        }
    }

    /// <summary>
    /// ساخت ستون‌های pivot برای FicheItem
    /// </summary>
    private List<string> BuildFicheItemPivotColumns(FicheItemInfo info)
    {
        var columns = new List<string>();

        for (int rowNum = 1; rowNum <= info.MaxRows; rowNum++)
        {
            var collectionAlias = $"FicheItem_{rowNum}";
            
            // ستون Value با alias که شامل عنوان WageItem است
            columns.Add($"[{collectionAlias}].[Value] AS [FicheItem_Value_{rowNum}]");
            
            // ستون WageItem.Title برای استفاده در هدر
            columns.Add($"[{collectionAlias}].[WageItem_Title] AS [FicheItem_WageItem_Title_{rowNum}]");
        }

        return columns;
    }

    /// <summary>
    /// ساخت ستون‌های PersonnelFunction
    /// </summary>
    private List<string> BuildPersonnelFunctionColumns(PersonnelFunctionInfo info)
    {
        var columns = new List<string>();
        var alias = "PersonnelFunction";
        
        // فهرست ستون‌های PersonnelFunction که باید نمایش داده شوند
        var personnelFunctionFields = new[]
        {
            ("FunctionDay", "روزهای ماه"),
            ("PersonnelFunctionDay", "روزهای کارکرد"),
            ("PersonnelHourPresent", "ساعت حضور"),
            ("PersonnelNoEnter", "غیبت"),
            ("PersonnelAbsenceDay", "روز غیبت"),
            ("PersonnelIllnessDay", "روزهای استعلاجی"),
            ("PersonnelMissionHours", "تعداد ساعت ماموریت"),
            ("PersonnelOverTime", "ساعت اضافه کاری"),
            ("PersonnelNightWork", "کار شبانه"),
            ("PersonnelWorkingHolidayHours", "ساعت کار در تعطیل"),
            ("RealFunctionDay", "روز کارکرد واقعی"),
            ("HolidayFunctionDay", "روز کارکرد تعطیل"),
            ("PersonnelMissionDay", "روز ماموریت"),
            ("PaylessDay", "روز بدون حقوق"),
            ("RewardsDay", "روز پاداش"),
            ("PersonnelNightWorkDay", "روز کار شبانه"),
            ("PersonnelWorkingHolidaysDay", "روز کار در تعطیل"),
            ("LinearFunctionDay", "روز کارکرد خطی"),
            ("PersonnelCeillingOvertime", "سقف اضافه کاری"),
            ("PersonnelOverTimeFixed", "اضافه کاری ثابت"),
            ("PersonnelHourlyWork", "کار ساعتی"),
            ("PersonnelHourlyWorkMinutes", "دقیقه کار ساعتی"),
            ("PaylessMinutes", "دقیقه بدون حقوق"),
            ("Karaneh", "کارانه"),
            ("PersonnelNightWorkMinutes", "دقیقه کار شبانه"),
            ("BasijOverTime", "اضافه کاری بسیج"),
            ("PersonnelWorkingHolidayMinutes", "دقیقه کار در تعطیل"),
            ("FridayWorkHours", "جمعه کاری"),
            ("FridayWorkAllowance", "فوق العاده جمعه کاری"),
            ("NightWorkAllowance", "فوق العاده شبکاری")
        };
        
        foreach (var (fieldName, friendlyName) in personnelFunctionFields)
        {
            columns.Add($"[{alias}].[{fieldName}] AS [PersonnelFunction_{fieldName}]");
        }
        
        return columns;
    }

    /// <summary>
    /// پیدا کردن حداکثر تعداد FicheLeaveItem برای تمام parent records
    /// </summary>
    private int GetMaxFicheLeaveItemCount(
        ReportableEntity entity,
        List<ReportableField> allFields,
        ReportBuilderRequestDTO request)
    {
        try
        {
            var connectionString = _globalDbContext.Database.GetConnectionString();
            using var connection = new SqlConnection(connectionString);
            connection.Open();

            // ساخت WHERE clause برای فیلترها
            var whereClauses = new List<string>();
            var parameters = new Dictionary<string, object?>();
            
            // Get single navigation info for filters
            var (singleNavigations, _) = GetNavigationInfoWithCollections(entity, allFields);
            
            // Add user access control clauses
            var userAccessClauses = BuildUserAccessWhereClause(entity);
            whereClauses.AddRange(userAccessClauses);

            // Add PayLocationIds filter if provided
            if (request.PayLocationIds != null && request.PayLocationIds.Any())
            {
                var hasGroupBy = request.GroupByFieldIds != null && request.GroupByFieldIds.Any();
                var payLocationFilter = BuildPayLocationFilterWhereClause(entity, request.PayLocationIds, hasGroupBy);
                if (!string.IsNullOrEmpty(payLocationFilter))
                {
                    whereClauses.Add(payLocationFilter);
                }
            }
            
            if (request.Filters != null && request.Filters.Any())
            {
                int paramIndex = 0;
                foreach (var filter in request.Filters)
                {
                    var field = allFields.FirstOrDefault(f => f.Id == filter.FieldId);
                    if (field == null) continue;

                    string fieldPath;
                    if (string.IsNullOrEmpty(field.NavigationPath))
                    {
                        fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                    }
                    else
                    {
                        var navInfo = singleNavigations.FirstOrDefault(n => n.NavigationProperty == field.NavigationPath);
                        if (navInfo != null)
                        {
                            fieldPath = $"[{navInfo.Alias}].[{field.TechnicalName}]";
                        }
                        else
                        {
                            fieldPath = $"[{entity.TableName}].[{field.TechnicalName}]";
                        }
                    }

                    var operator_ = _unitOfWork.Context.Set<FieldOperator>()
                        .FirstOrDefault(o => o.Id == filter.OperatorId);

                    if (operator_ == null) continue;

                    string filterWhereClause = BuildWhereClause(fieldPath, operator_.Operator, filter, ref paramIndex, parameters);
                    if (!string.IsNullOrEmpty(filterWhereClause))
                    {
                        whereClauses.Add(filterWhereClause);
                    }
                }
            }

            var whereClause = whereClauses.Any() 
                ? "WHERE " + string.Join(" AND ", whereClauses) 
                : "";

            // JOIN clauses for single navigation properties with NOLOCK hint
            var joinClauses = new StringBuilder();
            foreach (var nav in singleNavigations)
            {
                joinClauses.Append($" LEFT JOIN [{nav.TargetSchema}].[{nav.TargetTable}] AS [{nav.Alias}] WITH (NOLOCK) ON [{nav.ParentAlias}].[{nav.ForeignKeyColumn}] = [{nav.Alias}].[{nav.PrincipalKeyColumn}]");
            }

            // کوئری برای پیدا کردن حداکثر تعداد رکورد در FicheLeaveItem برای هر parent
            var ficheWhereClause = "";
            if (request.FichePaymentPeriodYear.HasValue && request.FichePaymentPeriodMonth.HasValue)
            {
                ficheWhereClause = $@"
                    AND EXISTS (
                        SELECT 1 FROM [Payroll].[Fiche] AS F
                        INNER JOIN [Payroll].[Payment_Period] AS PP ON F.[PaymentPeriodId] = PP.[Id]
                        WHERE F.[Id] = FLI.[FicheId]
                        AND PP.[ShamsiYear] = {request.FichePaymentPeriodYear.Value}
                        AND PP.[ShamsiMonth] = {request.FichePaymentPeriodMonth.Value}
                        AND F.[IsDeleted] = 0
                        AND PP.[IsDeleted] = 0
                    )";
            }

            var query = $@"
                SELECT MAX(ItemCount) as MaxCount
                FROM (
                    SELECT 
                        [{entity.TableName}].[EmployeeId],
                        COUNT(FLI.Id) as ItemCount
                    FROM [{entity.Schema}].[{entity.TableName}] WITH (NOLOCK)
                    {joinClauses}
                    LEFT JOIN [Payroll].[Fiche] AS F_Main WITH (NOLOCK)
                        ON [{entity.TableName}].[EmployeeId] = F_Main.[EmployeeId]
                        AND F_Main.[IsDeleted] = 0
                    LEFT JOIN [Payroll].[Payment_Period] AS PP_Main WITH (NOLOCK)
                        ON F_Main.[PaymentPeriodId] = PP_Main.[Id]
                        AND PP_Main.[IsDeleted] = 0
                    LEFT JOIN [Payroll].[Fiche_Leave_Item] AS FLI WITH (NOLOCK)
                        ON F_Main.[Id] = FLI.[FicheId]
                        AND FLI.[IsDeleted] = 0
                        {ficheWhereClause}
                    {whereClause}
                    GROUP BY [{entity.TableName}].[EmployeeId]
                ) AS Counts";

            using var command = new SqlCommand(query, connection);
            command.CommandTimeout = 60; // Set timeout to 60 seconds
            foreach (var param in parameters)
            {
                command.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
            }

            var result = command.ExecuteScalar();
            var maxCount = result != null && result != DBNull.Value ? Convert.ToInt32(result) : 0;
            
            // محدود کردن به حداکثر 50
            return Math.Min(maxCount, 50);
        }
        catch
        {
            // در صورت خطا، مقدار پیش‌فرض 0 را برمی‌گردانیم
            return 0;
        }
    }

    /// <summary>
    /// ساخت ستون‌های pivot برای FicheLeaveItem
    /// </summary>
    private List<string> BuildFicheLeaveItemPivotColumns(FicheLeaveItemInfo info)
    {
        var columns = new List<string>();

        for (int rowNum = 1; rowNum <= info.MaxRows; rowNum++)
        {
            var collectionAlias = $"FicheLeaveItem_{rowNum}";
            
            // ستون LeaveBalanceTicks (برای محاسبات)
            columns.Add($"[{collectionAlias}].[LeaveBalanceTicks] AS [FicheLeaveItem_LeaveBalanceTicks_{rowNum}]");
            
            // ستون LeaveAmountTicks (برای محاسبات)
            columns.Add($"[{collectionAlias}].[LeaveAmountTicks] AS [FicheLeaveItem_LeaveAmountTicks_{rowNum}]");
            
            // ستون LeaveBalanceFormatted (برای نمایش)
            columns.Add($"[{collectionAlias}].[LeaveBalanceFormatted] AS [FicheLeaveItem_LeaveBalanceFormatted_{rowNum}]");
            
            // ستون LeaveAmountFormatted (برای نمایش)
            columns.Add($"[{collectionAlias}].[LeaveAmountFormatted] AS [FicheLeaveItem_LeaveAmountFormatted_{rowNum}]");
            
            // ستون LeaveType.Title برای استفاده در هدر
            columns.Add($"[{collectionAlias}].[LeaveType_Title] AS [FicheLeaveItem_LeaveType_Title_{rowNum}]");
        }

        return columns;
    }
}

