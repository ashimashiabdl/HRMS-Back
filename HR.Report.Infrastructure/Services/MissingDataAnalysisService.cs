using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using DocumentFormat.OpenXml;
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
using Hr.Employee.infrastructure.Services;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Data;
using TempGlobalFileModel = HR.Report.Infrastructure.GlobalDBContext.Models.TempGlobalFile;
using UserPayLocationModel = HR.Report.Infrastructure.GlobalDBContext.Models.UserPayLocation;

namespace HR.Report.Infrastructure.Services;

/// <summary>
/// سرویس تحلیل اطلاعات ناقص
/// </summary>
public class MissingDataAnalysisService : IScopedServices
{
    private readonly IUnitOfWork<ReportContext> _unitOfWork;
    private readonly IUnitOfWork<BaseInfoContext> _baseInfoUnitOfWork;
    private readonly IUnitOfWork<IdentityContext> _identityUnitOfWork;
    private readonly GlobalDbContext _globalDbContext;
    private readonly EmployeeService _employeeService;
    private readonly UserResolverService _userResolverService;
    private readonly ILogger<MissingDataAnalysisService> _logger;
    private readonly string _connectionString;

    public MissingDataAnalysisService(
        IUnitOfWork<ReportContext> unitOfWork,
        IUnitOfWork<BaseInfoContext> baseInfoUnitOfWork,
        IUnitOfWork<IdentityContext> identityUnitOfWork,
        GlobalDbContext globalDbContext,
        EmployeeService employeeService,
        UserResolverService userResolverService,
        ILogger<MissingDataAnalysisService> logger)
    {
        _unitOfWork = unitOfWork;
        _baseInfoUnitOfWork = baseInfoUnitOfWork;
        _identityUnitOfWork = identityUnitOfWork;
        _globalDbContext = globalDbContext;
        _employeeService = employeeService;
        _userResolverService = userResolverService;
        _logger = logger;

        // Get connection string
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly()?.Location) ?? "")
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
    /// دریافت فهرست ReportableEntityId های مجاز برای کاربر جاری
    /// </summary>
    private HashSet<long>? GetAllowedReportableEntityIds()
    {
        if (_userResolverService.IsAdmin())
        {
            return null; // null یعنی همه موجودیت‌ها مجاز هستند
        }

        var userId = _userResolverService.GetUserId();
        if (userId <= 0)
        {
            return new HashSet<long>();
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
        
        if (allowedIds == null)
        {
            return true;
        }

        return allowedIds.Contains(entityId);
    }

    /// <summary>
    /// اعتبارسنجی PayLocationIds
    /// </summary>
    private OperationResult ValidatePayLocationIds(List<long> payLocationIds)
    {
        var currentUserId = _userResolverService.GetUserId();
        var userPayLocations = _globalDbContext.Set<UserPayLocationModel>()
            .Where(upl => upl.UserId == currentUserId && !upl.IsDeleted)
            .Select(upl => upl.PayLocationId)
            .ToList();

        var invalidIds = payLocationIds.Where(id => !userPayLocations.Contains(id)).ToList();
        if (invalidIds.Any())
        {
            return OperationResult.Failed($"شما به محل پرداخت‌های انتخاب شده دسترسی ندارید");
        }

        return OperationResult.Succeeded();
    }

    /// <summary>
    /// دریافت فهرست موجودیت‌های قابل بررسی
    /// </summary>
    public OperationResult GetAvailableEntities()
    {
        try
        {
            var allEntities = _unitOfWork.Context.Set<ReportableEntity>()
                .Where(e => e.IsActive)
                .OrderBy(e => e.FriendlyName)
                .ToList();

            // فیلتر کردن موجودیت‌ها بر اساس دسترسی کاربر
            var allowedIds = GetAllowedReportableEntityIds();
            var entities = allowedIds == null
                ? allEntities
                : allEntities.Where(e => allowedIds.Contains(e.Id)).ToList();

            var result = entities.Select(e => new AvailableEntityDTO
            {
                Id = e.Id,
                FriendlyName = e.FriendlyName,
                TechnicalName = e.TechnicalName,
                Schema = e.Schema,
                TableName = e.TableName,
                Description = e.Description,
                FieldCount = e.Fields.Count(f => f.IsActive)
            }).ToList();

            return OperationResult.Succeeded(payload: result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت موجودیت‌ها");
            return OperationResult.Failed("خطا در دریافت موجودیت‌ها");
        }
    }

    /// <summary>
    /// دریافت فیلدهای قابل بررسی برای یک موجودیت
    /// </summary>
    public OperationResult GetEntityFields(long entityId)
    {
        try
        {
            // بررسی دسترسی
            if (!HasAccessToReportableEntity(entityId))
            {
                return OperationResult.Failed("شما به این موجودیت دسترسی ندارید");
            }

            var fields = _unitOfWork.Context.Set<ReportableField>()
                .Include(f => f.FieldDataType)
                .Where(f => f.ReportableEntityId == entityId && f.IsActive)
                .OrderBy(f => f.Priority)
                .ThenBy(f => f.FriendlyName)
                .Select(f => new AvailableFieldDTO
                {
                    Id = f.Id,
                    FriendlyName = f.FriendlyName,
                    TechnicalName = f.TechnicalName,
                    DataTypeName = f.FieldDataType != null ? f.FieldDataType.TypeName : null,
                    NavigationPath = f.NavigationPath,
                    IsFilterable = f.IsFilterable,
                    IsSelectable = f.IsSelectable
                })
                .ToList();

            return OperationResult.Succeeded(payload: fields);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در دریافت فیلدها");
            return OperationResult.Failed("خطا در دریافت فیلدها");
        }
    }

    /// <summary>
    /// تحلیل اطلاعات ناقص
    /// </summary>
    public OperationResult AnalyzeMissingData(MissingDataAnalysisRequestDTO request)
    {
        try
        {
            var currentUserId = _userResolverService.GetUserId();
            var isAdmin = _userResolverService.IsAdmin();

            // بررسی دسترسی به موجودیت
            if (!HasAccessToReportableEntity(request.EntityId))
            {
                return OperationResult.Failed("شما به این موجودیت دسترسی ندارید");
            }

            // اعتبارسنجی PayLocationIds
            if (request.PayLocationIds != null && request.PayLocationIds.Any())
            {
                var validationResult = ValidatePayLocationIds(request.PayLocationIds);
                if (!validationResult.Success)
                {
                    return validationResult;
                }
            }

            // دریافت موجودیت
            var entity = _unitOfWork.Context.Set<ReportableEntity>()
                .Include(e => e.Fields)
                .FirstOrDefault(e => e.Id == request.EntityId && e.IsActive);

            if (entity == null)
                return OperationResult.Failed("موجودیت یافت نشد");

            // دریافت فیلدهای انتخابی
            var selectedFields = entity.Fields
                .Where(f => f.IsActive && 
                    (request.FieldIds == null || !request.FieldIds.Any() || request.FieldIds.Contains(f.Id)))
                .ToList();

            if (!selectedFields.Any())
                return OperationResult.Failed("هیچ فیلدی انتخاب نشده است");

            // تحلیل اطلاعات ناقص
            var missingDataResults = new List<MissingDataResultDTO>();
            var includeEmployeeInfo = ShouldIncludeEmployeeInfo(entity);
            var isEmpSchema = (entity.Schema?.ToLower() ?? "") == "emp";

            // اگر موجودیت Employee است، باید دسترسی‌ها را چک کنیم
            if (IsEmployeeEntity(entity))
            {
                // دریافت لیست کارکنان قابل دسترسی
                var accessibleEmployeeIds = isAdmin 
                    ? null // null یعنی همه کارکنان
                    : _employeeService.GetAccessibleEmployeeIds(currentUserId).ToList();

                missingDataResults = AnalyzeEmployeeMissingData(
                    entity, 
                    selectedFields, 
                    accessibleEmployeeIds,
                    request,
                    includeEmployeeInfo);
            }
            else if (isEmpSchema && IsRelatedToEmployee(entity))
            {
                // برای جداول emp که EmployeeId دارند، باید Employee هایی که هیچ رکوردی ندارند را بررسی کنیم
                var accessibleEmployeeIds = isAdmin 
                    ? null 
                    : _employeeService.GetAccessibleEmployeeIds(currentUserId).ToList();

                missingDataResults = AnalyzeEmpSchemaMissingData(
                    entity, 
                    selectedFields,
                    accessibleEmployeeIds,
                    request,
                    includeEmployeeInfo);
            }
            else
            {
                // برای موجودیت‌های دیگر
                missingDataResults = AnalyzeGenericMissingData(
                    entity, 
                    selectedFields,
                    request,
                    includeEmployeeInfo);
            }

            return OperationResult.Succeeded(payload: new MissingDataAnalysisResponseDTO
            {
                EntityId = entity.Id,
                EntityName = entity.FriendlyName,
                TotalRecords = missingDataResults.Count,
                Results = missingDataResults
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در تحلیل اطلاعات ناقص");
            return OperationResult.Failed($"خطا در تحلیل اطلاعات ناقص: {ex.Message}");
        }
    }

    /// <summary>
    /// بررسی اینکه آیا موجودیت Employee است
    /// </summary>
    private bool IsEmployeeEntity(ReportableEntity entity)
    {
        var technicalName = entity.TechnicalName?.ToLower() ?? "";
        var tableName = entity.TableName?.ToLower() ?? "";
        return technicalName.Contains("employee") || tableName.Contains("employee");
    }

    /// <summary>
    /// بررسی اینکه آیا موجودیت به Employee مرتبط است (دارای فیلد EmployeeId)
    /// </summary>
    private bool IsRelatedToEmployee(ReportableEntity entity)
    {
        // بررسی اینکه آیا فیلد EmployeeId دارد
        return entity.Fields.Any(f => 
            f.IsActive && 
            (f.TechnicalName?.Equals("EmployeeId", StringComparison.OrdinalIgnoreCase) == true ||
             f.TechnicalName?.Equals("emp.EmployeeId", StringComparison.OrdinalIgnoreCase) == true));
    }

    /// <summary>
    /// بررسی اینکه آیا باید اطلاعات Employee را اضافه کنیم
    /// </summary>
    private bool ShouldIncludeEmployeeInfo(ReportableEntity entity)
    {
        return IsEmployeeEntity(entity) || IsRelatedToEmployee(entity);
    }

    /// <summary>
    /// تحلیل اطلاعات ناقص برای Employee با در نظر گیری دسترسی‌ها
    /// </summary>
    private List<MissingDataResultDTO> AnalyzeEmployeeMissingData(
        ReportableEntity entity,
        List<ReportableField> fields,
        List<long>? accessibleEmployeeIds,
        MissingDataAnalysisRequestDTO request,
        bool includeEmployeeInfo = true)
    {
        var results = new List<MissingDataResultDTO>();
        var schema = entity.Schema ?? "emp";
        var tableName = entity.TableName ?? "Employee";

        // ساخت لیست فیلدهای قابل بررسی
        var fieldsToCheck = fields.Where(f => 
            !string.IsNullOrEmpty(f.TechnicalName)).ToList();

        foreach (var field in fieldsToCheck)
        {
            try
            {
                var sql = BuildMissingDataQuery(schema, tableName, field, accessibleEmployeeIds, request.PayLocationIds, includeEmployeeInfo);
                
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(sql, connection);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var recordId = reader.GetValue(0)?.ToString() ?? "";
                    var recordIdentifier = reader.GetValue(1)?.ToString() ?? recordId;

                    var result = new MissingDataResultDTO
                    {
                        RecordId = recordId,
                        RecordIdentifier = recordIdentifier,
                        FieldId = field.Id,
                        FieldName = field.FriendlyName ?? field.TechnicalName,
                        FieldTechnicalName = field.TechnicalName,
                        EntityName = entity.FriendlyName
                    };

                    // اگر باید اطلاعات Employee را اضافه کنیم
                    if (includeEmployeeInfo)
                    {
                        result.EmployeeFirstName = reader.IsDBNull(2) ? null : reader.GetString(2);
                        result.EmployeeLastName = reader.IsDBNull(3) ? null : reader.GetString(3);
                        result.EmployeeNationalNo = reader.IsDBNull(4) ? null : reader.GetString(4);
                        result.EmployeeBaseOrganisationTitle = reader.IsDBNull(5) ? null : reader.GetString(5);
                        result.EmployeeGenderTitle = reader.IsDBNull(6) ? null : reader.GetString(6);
                        result.EmployeeMaritalStatusTitle = reader.IsDBNull(7) ? null : reader.GetString(7);
                    }

                    results.Add(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"خطا در بررسی فیلد {field.TechnicalName}");
            }
        }

        return results;
    }

    /// <summary>
    /// تحلیل اطلاعات ناقص برای جداول emp که EmployeeId دارند
    /// این متد Employee هایی که هیچ رکوردی در جدول ندارند را پیدا می‌کند
    /// </summary>
    private List<MissingDataResultDTO> AnalyzeEmpSchemaMissingData(
        ReportableEntity entity,
        List<ReportableField> fields,
        List<long>? accessibleEmployeeIds,
        MissingDataAnalysisRequestDTO request,
        bool includeEmployeeInfo = true)
    {
        var results = new List<MissingDataResultDTO>();
        var schema = entity.Schema ?? "emp";
        var tableName = entity.TableName ?? entity.TechnicalName;

        try
        {
            // دریافت لیست Employee هایی که رکوردی در این جدول ندارند
            var sql = BuildMissingEmployeeRecordsQuery(schema, tableName, accessibleEmployeeIds, request.PayLocationIds, includeEmployeeInfo);
            
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            connection.Open();

            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var employeeId = reader.GetValue(0)?.ToString() ?? "";
                var employeeIdentifier = reader.GetValue(1)?.ToString() ?? employeeId;

                var result = new MissingDataResultDTO
                {
                    RecordId = employeeId,
                    RecordIdentifier = employeeIdentifier,
                    FieldId = 0, // برای این مورد فیلد خاصی نداریم
                    FieldName = "عدم وجود رکورد",
                    FieldTechnicalName = "NoRecord",
                    EntityName = entity.FriendlyName
                };

                // اگر باید اطلاعات Employee را اضافه کنیم
                if (includeEmployeeInfo)
                {
                    result.EmployeeFirstName = reader.IsDBNull(2) ? null : reader.GetString(2);
                    result.EmployeeLastName = reader.IsDBNull(3) ? null : reader.GetString(3);
                    result.EmployeeNationalNo = reader.IsDBNull(4) ? null : reader.GetString(4);
                    result.EmployeeBaseOrganisationTitle = reader.IsDBNull(5) ? null : reader.GetString(5);
                    result.EmployeeGenderTitle = reader.IsDBNull(6) ? null : reader.GetString(6);
                    result.EmployeeMaritalStatusTitle = reader.IsDBNull(7) ? null : reader.GetString(7);
                }

                results.Add(result);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, $"خطا در بررسی Employee های بدون رکورد در جدول {tableName}");
        }

        return results;
    }

    /// <summary>
    /// ساخت کوئری SQL برای یافتن Employee هایی که هیچ رکوردی در جدول ندارند
    /// </summary>
    private string BuildMissingEmployeeRecordsQuery(
        string schema,
        string tableName,
        List<long>? accessibleEmployeeIds,
        List<long>? payLocationIds,
        bool includeEmployeeInfo = false)
    {
        // اگر باید اطلاعات Employee را اضافه کنیم
        string employeeSelect = "";
        string employeeJoin = "";
        if (includeEmployeeInfo)
        {
            employeeSelect = @"
                , e.FirstName AS EmployeeFirstName
                , e.LastName AS EmployeeLastName
                , e.NationalNo AS EmployeeNationalNo
                , COALESCE(org.title, '') AS EmployeeBaseOrganisationTitle
                , COALESCE(gender.title, '') AS EmployeeGenderTitle
                , COALESCE(marital.title, '') AS EmployeeMaritalStatusTitle";
            employeeJoin = @"
                LEFT JOIN [Org].[Organisation_Chart] org ON org.Id = e.BaseOrganisationId AND org.IsDeleted = 0
                LEFT JOIN [bas].[Base_Table_Value] gender ON gender.Id = e.GenderId AND gender.IsDeleted = 0
                LEFT JOIN [bas].[Base_Table_Value] marital ON marital.Id = e.MaritalStatusId AND marital.IsDeleted = 0";
        }

        var sql = $@"
            SELECT TOP 10000
                CAST(e.Id AS NVARCHAR) AS EmployeeId,
                COALESCE(e.PersonelCode, CONCAT(e.FirstName, ' ', e.LastName), CAST(e.Id AS NVARCHAR)) AS EmployeeIdentifier
                {employeeSelect}
            FROM [emp].[Employee] e
            {employeeJoin}
            WHERE e.IsDeleted = 0
                AND NOT EXISTS (
                    SELECT 1 
                    FROM [{schema}].[{tableName}] t
                    WHERE t.EmployeeId = e.Id
                    AND t.IsDeleted = 0
                )";

        // اگر محدودیت دسترسی کارکنان وجود دارد
        if (accessibleEmployeeIds != null && accessibleEmployeeIds.Any())
        {
            var ids = string.Join(",", accessibleEmployeeIds);
            sql += $" AND e.Id IN ({ids})";
        }

        // اگر فیلتر PayLocation اعمال شده
        if (payLocationIds != null && payLocationIds.Any())
        {
            var payLocIds = string.Join(",", payLocationIds);
            sql += $@" AND EXISTS (
                SELECT 1 
                FROM [Order].[Recruit_Order] ro 
                INNER JOIN [Order].[Interdict_Order] io ON io.RecruitOrderId = ro.Id AND io.StatusId = 9
                WHERE ro.EmployeeId = e.Id
                AND ro.PayLocationId IN ({payLocIds})
            )";
        }

        return sql;
    }

    /// <summary>
    /// تحلیل اطلاعات ناقص برای موجودیت‌های عمومی
    /// </summary>
    private List<MissingDataResultDTO> AnalyzeGenericMissingData(
        ReportableEntity entity,
        List<ReportableField> fields,
        MissingDataAnalysisRequestDTO request,
        bool includeEmployeeInfo = false)
    {
        var results = new List<MissingDataResultDTO>();
        var schema = entity.Schema ?? "bas";
        var tableName = entity.TableName ?? entity.TechnicalName;

        var fieldsToCheck = fields.Where(f => 
            !string.IsNullOrEmpty(f.TechnicalName)).ToList();

        foreach (var field in fieldsToCheck)
        {
            try
            {
                var sql = BuildMissingDataQuery(schema, tableName, field, null, null, includeEmployeeInfo);
                
                using var connection = new SqlConnection(_connectionString);
                using var command = new SqlCommand(sql, connection);
                connection.Open();

                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var recordId = reader.GetValue(0)?.ToString() ?? "";
                    var recordIdentifier = reader.GetValue(1)?.ToString() ?? recordId;

                    var result = new MissingDataResultDTO
                    {
                        RecordId = recordId,
                        RecordIdentifier = recordIdentifier,
                        FieldId = field.Id,
                        FieldName = field.FriendlyName ?? field.TechnicalName,
                        FieldTechnicalName = field.TechnicalName,
                        EntityName = entity.FriendlyName
                    };

                    // اگر باید اطلاعات Employee را اضافه کنیم
                    if (includeEmployeeInfo)
                    {
                        result.EmployeeFirstName = reader.IsDBNull(2) ? null : reader.GetString(2);
                        result.EmployeeLastName = reader.IsDBNull(3) ? null : reader.GetString(3);
                        result.EmployeeNationalNo = reader.IsDBNull(4) ? null : reader.GetString(4);
                        result.EmployeeBaseOrganisationTitle = reader.IsDBNull(5) ? null : reader.GetString(5);
                        result.EmployeeGenderTitle = reader.IsDBNull(6) ? null : reader.GetString(6);
                        result.EmployeeMaritalStatusTitle = reader.IsDBNull(7) ? null : reader.GetString(7);
                    }

                    results.Add(result);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, $"خطا در بررسی فیلد {field.TechnicalName}");
            }
        }

        return results;
    }

    /// <summary>
    /// ساخت کوئری SQL برای یافتن رکوردهای با اطلاعات ناقص
    /// </summary>
    private string BuildMissingDataQuery(
        string schema, 
        string tableName, 
        ReportableField field, 
        List<long>? accessibleEmployeeIds,
        List<long>? payLocationIds,
        bool includeEmployeeInfo = false)
    {
        var fieldName = field.TechnicalName ?? "";
        var identifierField = "CAST(t.Id AS NVARCHAR)"; // فیلد شناسه برای نمایش
        var tableAlias = "t";

        // برای Employee از PersonelCode یا FirstName + LastName استفاده می‌کنیم
        if (tableName.ToLower() == "employee")
        {
            identifierField = "COALESCE(t.PersonelCode, CONCAT(t.FirstName, ' ', t.LastName), CAST(t.Id AS NVARCHAR))";
        }

        var whereClause = $"t.[{fieldName}] IS NULL OR t.[{fieldName}] = ''";

        // اگر Foreign Key است، باید null بودن را چک کنیم
        if (fieldName.EndsWith("Id"))
        {
            whereClause = $"t.[{fieldName}] IS NULL";
        }

        // اگر باید اطلاعات Employee را اضافه کنیم
        string employeeJoin = "";
        string employeeSelect = "";
        if (includeEmployeeInfo)
        {
            if (tableName.ToLower() == "employee")
            {
                // اگر خود جدول Employee است
                employeeSelect = @"
                    , t.FirstName AS EmployeeFirstName
                    , t.LastName AS EmployeeLastName
                    , t.NationalNo AS EmployeeNationalNo
                    , COALESCE(org.title, '') AS EmployeeBaseOrganisationTitle
                    , COALESCE(gender.title, '') AS EmployeeGenderTitle
                    , COALESCE(marital.title, '') AS EmployeeMaritalStatusTitle";
                employeeJoin = @"
                    LEFT JOIN [Org].[Organisation_Chart] org ON org.Id = t.BaseOrganisationId AND org.IsDeleted = 0
                    LEFT JOIN [bas].[Base_Table_Value] gender ON gender.Id = t.GenderId AND gender.IsDeleted = 0
                    LEFT JOIN [bas].[Base_Table_Value] marital ON marital.Id = t.MaritalStatusId AND marital.IsDeleted = 0";
            }
            else
            {
                // اگر جدول دیگری است که EmployeeId دارد
                employeeSelect = @"
                    , e.FirstName AS EmployeeFirstName
                    , e.LastName AS EmployeeLastName
                    , e.NationalNo AS EmployeeNationalNo
                    , COALESCE(org.title, '') AS EmployeeBaseOrganisationTitle
                    , COALESCE(gender.title, '') AS EmployeeGenderTitle
                    , COALESCE(marital.title, '') AS EmployeeMaritalStatusTitle";
                employeeJoin = $@"
                    LEFT JOIN [emp].[Employee] e ON e.Id = t.EmployeeId AND e.IsDeleted = 0
                    LEFT JOIN [Org].[Organisation_Chart] org ON org.Id = e.BaseOrganisationId AND org.IsDeleted = 0
                    LEFT JOIN [bas].[Base_Table_Value] gender ON gender.Id = e.GenderId AND gender.IsDeleted = 0
                    LEFT JOIN [bas].[Base_Table_Value] marital ON marital.Id = e.MaritalStatusId AND marital.IsDeleted = 0";
            }
        }

        var sql = $@"
            SELECT TOP 10000
                CAST(t.Id AS NVARCHAR) AS RecordId,
                {identifierField} AS RecordIdentifier
                {employeeSelect}
            FROM [{schema}].[{tableName}] {tableAlias}
            {employeeJoin}
            WHERE {whereClause}
                AND t.IsDeleted = 0";

        // اگر محدودیت دسترسی کارکنان وجود دارد
        if (accessibleEmployeeIds != null && accessibleEmployeeIds.Any())
        {
            var ids = string.Join(",", accessibleEmployeeIds);
            if (tableName.ToLower() == "employee")
            {
                sql += $" AND t.Id IN ({ids})";
            }
            else if (includeEmployeeInfo)
            {
                sql += $" AND t.EmployeeId IN ({ids})";
            }
        }

        // اگر فیلتر PayLocation اعمال شده
        if (payLocationIds != null && payLocationIds.Any() && tableName.ToLower() == "employee")
        {
            var payLocIds = string.Join(",", payLocationIds);
            sql += $@" AND EXISTS (
                SELECT 1 
                FROM [Order].[Recruit_Order] ro 
                INNER JOIN [Order].[Interdict_Order] io ON io.RecruitOrderId = ro.Id AND io.StatusId = 9
                WHERE ro.EmployeeId = t.Id
                AND ro.PayLocationId IN ({payLocIds})
            )";
        }

        return sql;
    }

    /// <summary>
    /// ساخت فایل اکسل از نتایج
    /// </summary>
    public OperationResult ExportToExcel(MissingDataAnalysisRequestDTO request)
    {
        try
        {
            // دریافت نتایج تحلیل
            var analysisResult = AnalyzeMissingData(request);
            if (!analysisResult.Success || analysisResult.Payload == null)
            {
                return OperationResult.Failed("خطا در تحلیل اطلاعات");
            }

            var response = analysisResult.Payload as MissingDataAnalysisResponseDTO;
            if (response == null || response.Results == null)
            {
                return OperationResult.Failed("نتیجه‌ای یافت نشد");
            }

            // ساخت اکسل از داده‌ها
            var excelBytes = CreateExcelFromResults(response);

            // ذخیره در TempGlobalFile
            var tempFile = new TempGlobalFileModel
            {
                Content = excelBytes,
                CreateDate = DateTime.Now,
                Ipaddress = _userResolverService.GetIP() ?? "MissingDataAnalysis",
                MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                Extension = ".xlsx",
                Size = excelBytes.LongLength,
                UniqueId = Guid.NewGuid(),
                Title = $"گزارش_اطلاعات_ناقص_{response.EntityName}_{DateTime.Now:yyyyMMdd_HHmmss}",
                IsDeleted = false
            };

            _globalDbContext.Set<TempGlobalFileModel>().Add(tempFile);
            _globalDbContext.SaveChanges();

            return OperationResult.Succeeded(payload: tempFile.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ساخت فایل اکسل");
            return OperationResult.Failed($"خطا در ساخت فایل اکسل: {ex.Message}");
        }
    }

    /// <summary>
    /// ساخت فایل اکسل از نتایج
    /// </summary>
    private byte[] CreateExcelFromResults(MissingDataAnalysisResponseDTO response)
    {
        using var stream = new MemoryStream();
        using var workbook = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);

        var workbookPart = workbook.AddWorkbookPart();
        workbook.WorkbookPart.Workbook = new Workbook();
        workbook.WorkbookPart.Workbook.Sheets = new Sheets();

        var sheetPart = workbook.WorkbookPart.AddNewPart<WorksheetPart>();
        var sheetData = new SheetData();
        sheetPart.Worksheet = new Worksheet(sheetData);

        var sheets = workbook.WorkbookPart.Workbook.GetFirstChild<Sheets>();
        string relationshipId = workbook.WorkbookPart.GetIdOfPart(sheetPart);

        var sheet = new Sheet() { Id = relationshipId, SheetId = 1, Name = "اطلاعات ناقص" };
        sheets.Append(sheet);

        // بررسی اینکه آیا اطلاعات Employee وجود دارد
        var hasEmployeeInfo = response.Results?.Any(r => 
            !string.IsNullOrEmpty(r.EmployeeFirstName) || 
            !string.IsNullOrEmpty(r.EmployeeLastName) || 
            !string.IsNullOrEmpty(r.EmployeeNationalNo) ||
            !string.IsNullOrEmpty(r.EmployeeBaseOrganisationTitle) ||
            !string.IsNullOrEmpty(r.EmployeeGenderTitle) ||
            !string.IsNullOrEmpty(r.EmployeeMaritalStatusTitle)) ?? false;

        // ساخت هدر
        var headerRow = new Row();
        var headers = new List<string> { "شناسه رکورد", "شناسه نمایشی", "فیلد", "نام فنی فیلد", "موجودیت" };
        
        // اگر اطلاعات Employee وجود دارد، ستون‌های اضافی اضافه می‌کنیم
        if (hasEmployeeInfo)
        {
            headers.AddRange(new[] { "نام", "نام خانوادگی", "کد ملی", "سازمان", "جنسیت", "تاهل" });
        }
        
        foreach (var header in headers)
        {
            var cell = new Cell
            {
                DataType = CellValues.String,
                CellValue = new CellValue(header)
            };
            headerRow.AppendChild(cell);
        }
        sheetData.AppendChild(headerRow);

        // ساخت ردیف‌های داده
        foreach (var result in response.Results)
        {
            var row = new Row();
            var values = new List<string> 
            { 
                result.RecordId ?? "",
                result.RecordIdentifier ?? "",
                result.FieldName ?? "",
                result.FieldTechnicalName ?? "",
                result.EntityName ?? ""
            };

            // اگر اطلاعات Employee وجود دارد، مقادیر اضافی اضافه می‌کنیم
            if (hasEmployeeInfo)
            {
                values.AddRange(new[] 
                { 
                    result.EmployeeFirstName ?? "",
                    result.EmployeeLastName ?? "",
                    result.EmployeeNationalNo ?? "",
                    result.EmployeeBaseOrganisationTitle ?? "",
                    result.EmployeeGenderTitle ?? "",
                    result.EmployeeMaritalStatusTitle ?? ""
                });
            }

            foreach (var value in values)
            {
                var cell = new Cell
                {
                    DataType = CellValues.String,
                    CellValue = new CellValue(value)
                };
                row.AppendChild(cell);
            }
            sheetData.AppendChild(row);
        }

        workbook.Save();
        return stream.ToArray();
    }
}

