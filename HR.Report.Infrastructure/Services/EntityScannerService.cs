
using HR.Report.Core.Entity;
using HR.Report.Infrastructure.Data;
using HR.Report.Infrastructure.GlobalDBContext;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

namespace HR.Report.Infrastructure.Services;

/// <summary>
/// سرویس اسکن و ثبت خودکار Entity ها و فیلدهای آنها
/// </summary>
public class EntityScannerService : IScopedServices
{
    private readonly IUnitOfWork<ReportContext> _unitOfWork;
    private readonly GlobalDbContext _globalDbContext;
    private readonly string _connectionString;

    // Type mapping for SQL types to our FieldDataType IDs
    private readonly Dictionary<Type, long> _typeMapping = new()
    {
        { typeof(string), 1 },      // Text
        { typeof(int), 2 },         // Number
        { typeof(long), 2 },        // Number
        { typeof(short), 2 },       // Number
        { typeof(byte), 2 },        // Number
        { typeof(decimal), 2 },     // Number
        { typeof(double), 2 },      // Number
        { typeof(float), 2 },       // Number
        { typeof(DateTime), 3 },    // Date
        { typeof(DateTimeOffset), 3 }, // Date
        { typeof(bool), 4 },        // Boolean
        { typeof(Guid), 5 },        // Guid
    };

    // Maximum depth for nested navigation properties
    private const int MaxNavigationDepth = 3;

    public EntityScannerService(IUnitOfWork<ReportContext> unitOfWork, GlobalDbContext globalDbContext, IConfiguration configuration)
    {
        _unitOfWork = unitOfWork;
        _globalDbContext = globalDbContext;
        
        // Get connection string
        var raw = configuration.GetConnectionString("HRMSConnection");
        var dec = HR.SharedKernel.Security.ConnectionStringProtector.TryUnprotect(raw);
        _connectionString = dec ?? raw ?? "";
    }

    /// <summary>
    /// اسکن تمام Entity های سیستم و پر کردن جداول Reportable_Entity و Reportable_Field
    /// </summary>
    public OperationResult ScanAndPopulateEntities()
    {
        try
        {
            var scannedEntities = 0;
            var scannedFields = 0;

            // Clear existing data
            //var existingFields = _unitOfWork.Context.Set<ReportableField>().ToList();
            //_unitOfWork.Context.Set<ReportableField>().RemoveRange(existingFields);

            //var existingEntities = _unitOfWork.Context.Set<ReportableEntity>().ToList();
            //_unitOfWork.Context.Set<ReportableEntity>().RemoveRange(existingEntities);

            //_unitOfWork.Context.SaveChanges();

            // Get all entity types from GlobalDbContext
            var entityTypes = _globalDbContext.Model.GetEntityTypes()
                .Where(t => !t.IsOwned() && t.ClrType != null)
                .OrderBy(t => t.ClrType.FullName)
                .ToList();

            foreach (var entityType in entityTypes)
            {
                try
                {
                    var clrType = entityType.ClrType;
                    
                    // Skip abstract classes and interfaces
                    if (clrType.IsAbstract || clrType.IsInterface)
                        continue;

                    // Get table info
                    var tableName = entityType.GetTableName();

                    if (tableName != "User_Login_History")
                    {
                        continue;
                    }

                    var schema = entityType.GetSchema() ?? "dbo";

                    if (string.IsNullOrEmpty(tableName))
                        continue;

                    // Create ReportableEntity
                    var reportableEntity = new ReportableEntity
                    {
                        TechnicalName = clrType.FullName ?? clrType.Name,
                        FriendlyName = GetFriendlyName(clrType),
                        Schema = schema,
                        TableName = tableName,
                        Description = $"اطلاعات {GetFriendlyName(clrType)}",
                        IsActive = true,
                        title = GetFriendlyName(clrType),
                        CreateDate = DateTime.Now,
                        IPAddress = "127.0.0.1",
                        IsDeleted = false
                    };

                    _unitOfWork.Context.Set<ReportableEntity>().Add(reportableEntity);
                    _unitOfWork.Context.SaveChanges();
                    scannedEntities++;

                    // Get all properties
                    var properties = entityType.GetProperties()
                        .Where(p => !p.IsShadowProperty())
                        .OrderBy(p => p.Name)
                        .ToList();

                    foreach (var property in properties)
                    {
                        try
                        {
                            var propertyInfo = clrType.GetProperty(property.Name);
                            if (propertyInfo == null)
                                continue;

                            var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                            // Determine field data type
                            long fieldDataTypeId = DetermineFieldDataType(propertyType, property);

                            // Check if it's a navigation property
                            var navigation = entityType.FindNavigation(property.Name);
                            bool isNavigation = navigation != null;

                            // Skip collection navigations
                            if (isNavigation && navigation!.IsCollection)
                                continue;

                            var reportableField = new ReportableField
                            {
                                ReportableEntityId = reportableEntity.Id,
                                TechnicalName = property.Name,
                                FriendlyName = GetPropertyFriendlyName(propertyInfo, property.Name),
                                FieldDataTypeId = fieldDataTypeId,
                                NavigationPath = null, // Direct property
                                IsFilterable = !isNavigation && IsFilterable(propertyType),
                                IsSelectable = true,
                                IsSortable = !isNavigation && IsSortable(propertyType),
                                IsActive = true,
                                title = GetPropertyFriendlyName(propertyInfo, property.Name),
                                CreateDate = DateTime.Now,
                                IPAddress = "127.0.0.1",
                                IsDeleted = false
                            };

                            _unitOfWork.Context.Set<ReportableField>().Add(reportableField);
                            scannedFields++;

                            // For foreign key properties, also add navigation property fields (recursively)
                            if (property.IsForeignKey() && !isNavigation)
                            {
                                var foreignKeys = property.GetContainingForeignKeys();
                                foreach (var fk in foreignKeys)
                                {
                                    var navigationProperty = fk.DependentToPrincipal;

                                    if (navigationProperty != null)
                                    {
                                        // Recursively scan navigation properties
                                        var navFields = ScanNavigationProperties(
                                            reportableEntity.Id,
                                            navigationProperty,
                                            navigationProperty.Name,
                                            GetPropertyFriendlyName(propertyInfo, navigationProperty.Name),
                                            depth: 1
                                        );

                                        foreach (var navField in navFields)
                                        {
                                            _unitOfWork.Context.Set<ReportableField>().Add(navField);
                                            scannedFields++;
                                        }
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log and continue with next property
                            Console.WriteLine($"Error processing property {property.Name}: {ex.Message}");
                        }
                    }

                    _unitOfWork.Context.SaveChanges();
                }
                catch (Exception ex)
                {
                    // Log and continue with next entity
                    Console.WriteLine($"Error processing entity {entityType.Name}: {ex.Message}");
                }
            }

            return OperationResult.Succeeded(
                 $"اسکن با موفقیت انجام شد. تعداد Entity: {scannedEntities}, تعداد Field: {scannedFields}",
                payload: new { EntitiesScanned = scannedEntities, FieldsScanned = scannedFields }
            );
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در اسکن Entity ها: {ex.Message}");
        }
    }

    /// <summary>
    /// اسکن و پر کردن فیلدهای یک Entity خاص
    /// </summary>
    public OperationResult ScanAndPopulateFieldsForEntity(long reportableEntityId, string technicalName)
    {
        try
        {
            var scannedFields = 0;

            // بررسی وجود ReportableEntity
            var reportableEntity = _unitOfWork.Context.Set<ReportableEntity>()
                .FirstOrDefault(e => e.Id == reportableEntityId);

            if (reportableEntity == null)
            {
                return OperationResult.Failed($"موجودیت با شناسه {reportableEntityId} یافت نشد");
            }

            // بررسی وجود فیلدهای قبلی - اگر وجود داشتند، حذف می‌کنیم
            var existingFields = _unitOfWork.Context.Set<ReportableField>()
                .Where(f => f.ReportableEntityId == reportableEntityId)
                .ToList();

            if (existingFields.Any())
            {
                _unitOfWork.Context.Set<ReportableField>().RemoveRange(existingFields);
                _unitOfWork.Context.SaveChanges();
            }

            // پیدا کردن Entity type از GlobalDbContext
            var entityType = _globalDbContext.Model.GetEntityTypes()
                .FirstOrDefault(t => !t.IsOwned() && t.ClrType != null && 
                    (t.ClrType.FullName == technicalName || t.ClrType.Name == technicalName));

            if (entityType == null)
            {
                return OperationResult.Failed($"Entity با نام فنی '{technicalName}' در GlobalDbContext یافت نشد");
            }

            var clrType = entityType.ClrType;

            // Get all properties
            var properties = entityType.GetProperties()
                .Where(p => !p.IsShadowProperty())
                .OrderBy(p => p.Name)
                .ToList();

            foreach (var property in properties)
            {
                try
                {
                    var propertyInfo = clrType.GetProperty(property.Name);
                    if (propertyInfo == null)
                        continue;

                    var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                    // Determine field data type
                    long fieldDataTypeId = DetermineFieldDataType(propertyType, property);

                    // Check if it's a navigation property
                    var navigation = entityType.FindNavigation(property.Name);
                    bool isNavigation = navigation != null;

                    // Skip collection navigations
                    if (isNavigation && navigation!.IsCollection)
                        continue;

                    var reportableField = new ReportableField
                    {
                        ReportableEntityId = reportableEntityId,
                        TechnicalName = property.Name,
                        FriendlyName = GetPropertyFriendlyName(propertyInfo, property.Name),
                        FieldDataTypeId = fieldDataTypeId,
                        NavigationPath = null, // Direct property
                        IsFilterable = !isNavigation && IsFilterable(propertyType),
                        IsSelectable = true,
                        IsSortable = !isNavigation && IsSortable(propertyType),
                        IsActive = true,
                        title = GetPropertyFriendlyName(propertyInfo, property.Name),
                        CreateDate = DateTime.Now,
                        IPAddress = "127.0.0.1",
                        IsDeleted = false
                    };

                    _unitOfWork.Context.Set<ReportableField>().Add(reportableField);
                    scannedFields++;

                    // For foreign key properties, also add navigation property fields (recursively)
                    if (property.IsForeignKey() && !isNavigation)
                    {
                        var foreignKeys = property.GetContainingForeignKeys();
                        foreach (var fk in foreignKeys)
                        {
                            var navigationProperty = fk.DependentToPrincipal;

                            if (navigationProperty != null)
                            {
                                // Recursively scan navigation properties
                                var navFields = ScanNavigationProperties(
                                    reportableEntityId,
                                    navigationProperty,
                                    navigationProperty.Name,
                                    GetPropertyFriendlyName(propertyInfo, navigationProperty.Name),
                                    depth: 1
                                );

                                foreach (var navField in navFields)
                                {
                                    _unitOfWork.Context.Set<ReportableField>().Add(navField);
                                    scannedFields++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log and continue with next property
                    Console.WriteLine($"Error processing property {property.Name}: {ex.Message}");
                }
            }

            _unitOfWork.Context.SaveChanges();

            return OperationResult.Succeeded(
                $"اسکن فیلدها با موفقیت انجام شد. تعداد فیلد: {scannedFields}",
                payload: new { FieldsScanned = scannedFields }
            );
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در اسکن فیلدهای Entity: {ex.Message}");
        }
    }

    /// <summary>
    /// اسکن بازگشتی navigation properties برای پشتیبانی از چند سطح
    /// </summary>
    private List<ReportableField> ScanNavigationProperties(
        long reportableEntityId,
        INavigation navigation,
        string currentPath,
        string currentFriendlyName,
        int depth)
    {
        var fields = new List<ReportableField>();

        // Check depth limit to prevent infinite recursion
        if (depth > MaxNavigationDepth)
            return fields;

        var principalEntityType = navigation.ForeignKey.PrincipalEntityType;
        var displayFields = new[] { "Title", "Name", "FriendlyName", "Description" };

        // Add display fields from current navigation level
        foreach (var displayField in displayFields)
        {
            var navProperty = principalEntityType.FindProperty(displayField);
            if (navProperty != null)
            {
                var navFieldDataTypeId = DetermineFieldDataType(
                    navProperty.ClrType,
                    navProperty
                );

                var navReportableField = new ReportableField
                {
                    ReportableEntityId = reportableEntityId,
                    TechnicalName = displayField,
                    FriendlyName = $"{currentFriendlyName} - {displayField}",
                    FieldDataTypeId = navFieldDataTypeId,
                    NavigationPath = currentPath,
                    IsFilterable = true, // Enable filtering for nested navigation fields
                    IsSelectable = true,
                    IsSortable = true,
                    IsActive = true,
                    title = $"{currentFriendlyName} - {displayField}",
                    CreateDate = DateTime.Now,
                    IPAddress = "127.0.0.1",
                    IsDeleted = false
                };

                fields.Add(navReportableField);
            }
        }

        // Recursively scan nested navigation properties
        var nestedNavigations = principalEntityType.GetNavigations()
            .Where(n => !n.IsCollection && n.ForeignKey != null);

        foreach (var nestedNav in nestedNavigations)
        {
            try
            {
                var nestedPath = $"{currentPath}.{nestedNav.Name}";
                var nestedFriendlyName = $"{currentFriendlyName} > {nestedNav.Name}";

                var nestedFields = ScanNavigationProperties(
                    reportableEntityId,
                    nestedNav,
                    nestedPath,
                    nestedFriendlyName,
                    depth + 1
                );

                fields.AddRange(nestedFields);
            }
            catch (Exception ex)
            {
                // Log and continue with next nested navigation
                Console.WriteLine($"Error scanning nested navigation {nestedNav.Name}: {ex.Message}");
            }
        }

        return fields;
    }

    /// <summary>
    /// تعیین نوع داده فیلد
    /// </summary>
    private long DetermineFieldDataType(Type propertyType, IProperty property)
    {
        // Check if it's an enum
        if (propertyType.IsEnum)
            return 6; // Enum

        // Check type mapping
        if (_typeMapping.TryGetValue(propertyType, out var typeId))
            return typeId;

        // Check for nullable types
        var underlyingType = Nullable.GetUnderlyingType(propertyType);
        if (underlyingType != null && _typeMapping.TryGetValue(underlyingType, out var nullableTypeId))
            return nullableTypeId;

        // Default to Text
        return 1;
    }

    /// <summary>
    /// بررسی قابلیت فیلتر بودن
    /// </summary>
    private bool IsFilterable(Type propertyType)
    {
        // Most simple types are filterable
        return propertyType.IsPrimitive
            || propertyType == typeof(string)
            || propertyType == typeof(DateTime)
            || propertyType == typeof(DateTimeOffset)
            || propertyType == typeof(decimal)
            || propertyType == typeof(Guid)
            || propertyType.IsEnum
            || Nullable.GetUnderlyingType(propertyType) != null;
    }

    /// <summary>
    /// بررسی قابلیت مرتب‌سازی
    /// </summary>
    private bool IsSortable(Type propertyType)
    {
        // Same as filterable for now
        return IsFilterable(propertyType);
    }

    /// <summary>
    /// دریافت نام فارسی Entity
    /// </summary>
    private string GetFriendlyName(Type type)
    {
        // Try to get from DisplayName attribute
        var displayAttr = type.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>();
        if (displayAttr != null)
            return displayAttr.DisplayName;

        // Try to get from Description attribute
        var descAttr = type.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
        if (descAttr != null)
            return descAttr.Description;

        // Use class name
        return type.Name;
    }

    /// <summary>
    /// دریافت نام فارسی Property
    /// </summary>
    private string GetPropertyFriendlyName(PropertyInfo propertyInfo, string defaultName)
    {
        // Try to get from DisplayName attribute
        var displayAttr = propertyInfo.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>();
        if (displayAttr != null)
            return displayAttr.DisplayName;

        // Try to get from Description attribute
        var descAttr = propertyInfo.GetCustomAttribute<System.ComponentModel.DescriptionAttribute>();
        if (descAttr != null)
            return descAttr.Description;

        // Try to get from Display attribute
        var displayAttribute = propertyInfo.GetCustomAttribute<System.ComponentModel.DataAnnotations.DisplayAttribute>();
        if (displayAttribute != null && !string.IsNullOrEmpty(displayAttribute.Name))
            return displayAttribute.Name;

        // Use property name
        return defaultName;
    }

    /// <summary>
    /// دریافت لیست Schema های دیتابیس
    /// </summary>
    public OperationResult GetDatabaseSchemas()
    {
        try
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return OperationResult.Failed("Connection string یافت نشد");
            }

            var schemas = new List<string>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var sql = @"
                SELECT DISTINCT TABLE_SCHEMA 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE = 'BASE TABLE'
                ORDER BY TABLE_SCHEMA";

            using var command = new SqlCommand(sql, connection);
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                var schemaName = reader.GetString(0);
                if (!string.IsNullOrEmpty(schemaName))
                {
                    schemas.Add(schemaName);
                }
            }

            return OperationResult.Succeeded(payload: schemas);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در دریافت Schema ها: {ex.Message}");
        }
    }

    /// <summary>
    /// دریافت لیست جداول یک Schema
    /// </summary>
    public OperationResult GetTablesBySchema(string schemaName)
    {
        try
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                return OperationResult.Failed("Connection string یافت نشد");
            }

            if (string.IsNullOrEmpty(schemaName))
            {
                return OperationResult.Failed("نام Schema را وارد کنید");
            }

            var tables = new List<TableInfoDTO>();
            using var connection = new SqlConnection(_connectionString);
            connection.Open();

            var sql = @"
                SELECT TABLE_SCHEMA, TABLE_NAME
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_TYPE = 'BASE TABLE' 
                AND TABLE_SCHEMA = @SchemaName
                ORDER BY TABLE_NAME";

            using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@SchemaName", schemaName);
            using var reader = command.ExecuteReader();
            
            while (reader.Read())
            {
                tables.Add(new TableInfoDTO
                {
                    Schema = reader.GetString(0),
                    TableName = reader.GetString(1)
                });
            }

            return OperationResult.Succeeded(payload: tables);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در دریافت جداول: {ex.Message}");
        }
    }

    /// <summary>
    /// اسکن و ثبت یک جدول خاص
    /// </summary>
    public OperationResult ScanAndPopulateSingleTable(string schemaName, string tableName)
    {
        try
        {
            if (string.IsNullOrEmpty(schemaName) || string.IsNullOrEmpty(tableName))
            {
                return OperationResult.Failed("نام Schema و Table را وارد کنید");
            }

            // بررسی اینکه آیا این جدول قبلاً ثبت شده است
            var existingEntity = _unitOfWork.Context.Set<ReportableEntity>()
                .FirstOrDefault(e => e.Schema == schemaName && 
                                    e.TableName == tableName && 
                                    !e.IsDeleted);

            if (existingEntity != null)
            {
                return OperationResult.Failed($"این جدول قبلاً ثبت شده است. شناسه موجودیت: {existingEntity.Id}");
            }

            // پیدا کردن Entity type از GlobalDbContext
            var entityType = _globalDbContext.Model.GetEntityTypes()
                .FirstOrDefault(t => !t.IsOwned() && 
                                    t.ClrType != null &&
                                    t.GetSchema() == schemaName &&
                                    t.GetTableName() == tableName);

            if (entityType == null)
            {
                return OperationResult.Failed($"جدول '{schemaName}.{tableName}' در GlobalDbContext یافت نشد");
            }

            var clrType = entityType.ClrType;
            
            // Skip abstract classes and interfaces
            if (clrType.IsAbstract || clrType.IsInterface)
            {
                return OperationResult.Failed("نمی‌توان Entity های Abstract یا Interface را اسکن کرد");
            }

            var scannedFields = 0;

            // Create ReportableEntity
            var reportableEntity = new ReportableEntity
            {
                TechnicalName = clrType.FullName ?? clrType.Name,
                FriendlyName = GetFriendlyName(clrType),
                Schema = schemaName,
                TableName = tableName,
                Description = $"اطلاعات {GetFriendlyName(clrType)}",
                IsActive = true,
                title = GetFriendlyName(clrType),
                CreateDate = DateTime.Now,
                IPAddress = "127.0.0.1",
                IsDeleted = false
            };

            _unitOfWork.Context.Set<ReportableEntity>().Add(reportableEntity);
            _unitOfWork.Context.SaveChanges();

            // Get all properties
            var properties = entityType.GetProperties()
                .Where(p => !p.IsShadowProperty())
                .OrderBy(p => p.Name)
                .ToList();

            foreach (var property in properties)
            {
                try
                {
                    var propertyInfo = clrType.GetProperty(property.Name);
                    if (propertyInfo == null)
                        continue;

                    var propertyType = Nullable.GetUnderlyingType(propertyInfo.PropertyType) ?? propertyInfo.PropertyType;

                    // Determine field data type
                    long fieldDataTypeId = DetermineFieldDataType(propertyType, property);

                    // Check if it's a navigation property
                    var navigation = entityType.FindNavigation(property.Name);
                    bool isNavigation = navigation != null;

                    // Skip collection navigations
                    if (isNavigation && navigation!.IsCollection)
                        continue;

                    var reportableField = new ReportableField
                    {
                        ReportableEntityId = reportableEntity.Id,
                        TechnicalName = property.Name,
                        FriendlyName = GetPropertyFriendlyName(propertyInfo, property.Name),
                        FieldDataTypeId = fieldDataTypeId,
                        NavigationPath = null,
                        IsFilterable = !isNavigation && IsFilterable(propertyType),
                        IsSelectable = true,
                        IsSortable = !isNavigation && IsSortable(propertyType),
                        IsActive = true,
                        title = GetPropertyFriendlyName(propertyInfo, property.Name),
                        CreateDate = DateTime.Now,
                        IPAddress = "127.0.0.1",
                        IsDeleted = false
                    };

                    _unitOfWork.Context.Set<ReportableField>().Add(reportableField);
                    scannedFields++;

                    // For foreign key properties, also add navigation property fields (recursively)
                    if (property.IsForeignKey() && !isNavigation)
                    {
                        var foreignKeys = property.GetContainingForeignKeys();
                        foreach (var fk in foreignKeys)
                        {
                            var navigationProperty = fk.DependentToPrincipal;

                            if (navigationProperty != null)
                            {
                                // Recursively scan navigation properties
                                var navFields = ScanNavigationProperties(
                                    reportableEntity.Id,
                                    navigationProperty,
                                    navigationProperty.Name,
                                    GetPropertyFriendlyName(propertyInfo, navigationProperty.Name),
                                    depth: 1
                                );

                                foreach (var navField in navFields)
                                {
                                    _unitOfWork.Context.Set<ReportableField>().Add(navField);
                                    scannedFields++;
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Log and continue with next property
                    Console.WriteLine($"Error processing property {property.Name}: {ex.Message}");
                }
            }

            _unitOfWork.Context.SaveChanges();

            return OperationResult.Succeeded(
                $"اسکن جدول '{schemaName}.{tableName}' با موفقیت انجام شد. تعداد فیلد: {scannedFields}",
                payload: new { EntityId = reportableEntity.Id, FieldsScanned = scannedFields }
            );
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در اسکن جدول: {ex.Message}");
        }
    }
}

/// <summary>
/// DTO برای اطلاعات جدول
/// </summary>
public class TableInfoDTO
{
    public string Schema { get; set; } = "";
    public string TableName { get; set; } = "";
}

