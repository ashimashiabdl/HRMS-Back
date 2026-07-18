using AutoMapper;
using HR.Report.Core.DTOs;
using HR.Report.Core.Entity;
using HR.Report.Infrastructure.Data;
using HR.Report.Infrastructure.GlobalDBContext;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace HR.Report.Infrastructure.Services;

public class ReportableEntityService(IMapper mapper, IUnitOfWork<ReportContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService, GlobalDbContext globalDbContext, EntityScannerService scannerService)
    : BaseService<ReportableEntity, ReportContext, ReportableEntityDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private readonly GlobalDbContext _globalDbContext = globalDbContext;
    private readonly EntityScannerService _scannerService = scannerService;

    /// <summary>
    /// حذف فیزیکی موجودیت قابل گزارش و فیلدهای مرتبط
    /// </summary>
    public new OperationResult DeleteRecord(long id)
    {
        var entity = _unitOfWork.Context.Set<ReportableEntity>()
                .Include(e => e.Fields)
                .FirstOrDefault(e => e.Id == id);

        if (entity == null)
        {
            return OperationResult.NotFound();
        }

        // حذف فیزیکی فیلدهای مرتبط
        if (entity.Fields != null && entity.Fields.Any())
        {
            _unitOfWork.Context.Set<ReportableField>().RemoveRange(entity.Fields);
        }

        // حذف فیزیکی موجودیت
        _unitOfWork.Context.Set<ReportableEntity>().Remove(entity);

        _unitOfWork.Context.SaveChanges();

        return OperationResult.Succeeded("رکورد با موفقیت حذف شد");
    }

    /// <summary>
    /// دریافت فهرست جداول موجود در GlobalDbContext که در ReportableEntity نیستند
    /// </summary>
    public OperationResult GetAvailableTables()
    {
        try
        {
            // دریافت تمام Entity types از GlobalDbContext
            var entityTypes = _globalDbContext.Model.GetEntityTypes()
                .Where(t => !t.IsOwned() && t.ClrType != null)
                .OrderBy(t => t.ClrType.FullName)
                .ToList();

            // دریافت تمام TechnicalName های موجود در ReportableEntity
            var existingTechnicalNames = _unitOfWork.Context.Set<ReportableEntity>()
                .Where(e => !e.IsDeleted)
                .Select(e => e.TechnicalName)
                .ToList();

            var availableTables = new List<object>();

            foreach (var entityType in entityTypes)
            {
                try
                {
                    var clrType = entityType.ClrType;

                    // Skip abstract classes and interfaces
                    if (clrType.IsAbstract || clrType.IsInterface)
                        continue;

                    var technicalName = clrType.FullName ?? clrType.Name;
                    var tableName = entityType.GetTableName();
                    var schema = entityType.GetSchema() ?? "dbo";

                    if (string.IsNullOrEmpty(tableName))
                        continue;

                    // بررسی تکراری نبودن
                    if (existingTechnicalNames.Contains(technicalName))
                        continue;

                    // دریافت نام فارسی
                    var friendlyName = GetFriendlyName(clrType);
                    var displayName = friendlyName != clrType.Name ? $"{friendlyName} ({tableName})" : tableName;

                    availableTables.Add(new
                    {
                        TechnicalName = technicalName,
                        FriendlyName = friendlyName,
                        TableName = tableName,
                        Schema = schema,
                        DisplayName = displayName
                    });
                }
                catch (Exception ex)
                {
                    // Log and continue with next entity
                    Console.WriteLine($"Error processing entity {entityType.Name}: {ex.Message}");
                }
            }

            return OperationResult.Succeeded(payload: availableTables);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در دریافت فهرست جداول: {ex.Message}");
        }
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
    /// Override CreateForAsync to scan and populate fields after entity creation
    /// </summary>
    public new async Task<OperationResult> CreateForAsync(ReportableEntityDTO dto)
    {
        // بررسی تکراری نبودن بر اساس TechnicalName
        var existingEntity = _unitOfWork.Context.Set<ReportableEntity>()
            .FirstOrDefault(e => e.TechnicalName == dto.TechnicalName && !e.IsDeleted);

        if (existingEntity != null)
        {
            return OperationResult.Failed($"موجودیت با نام فنی '{dto.TechnicalName}' قبلاً ثبت شده است.");
        }

        // ایجاد موجودیت
        var result = await base.CreateForAsync(dto);

        if (result.Success && result.Payload is long entityId && entityId > 0)
        {
            // اسکن و پر کردن فیلدها برای موجودیت ایجاد شده
            var scanResult = _scannerService.ScanAndPopulateFieldsForEntity(entityId, dto.TechnicalName);
            
            if (!scanResult.Success)
            {
                // Log warning but don't fail the entity creation
                Console.WriteLine($"Warning: Failed to scan fields for entity {entityId}: {scanResult.Message}");
            }
        }

        return result;
    }
}

