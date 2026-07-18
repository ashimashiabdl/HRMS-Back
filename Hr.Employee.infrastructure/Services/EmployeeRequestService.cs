using AutoMapper;
using Dapper;
using HR.Employee.Core.DTOs;
using HR.Employee.Core.Entities;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using System.Data;

using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

public class EmployeeRequestService(
    IMapper mapper,
    IUnitOfWork<EmployeeContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService,
    EmployeeService employeeService)
    : BaseService<EmployeeRequest, EmployeeContext, EmployeeRequestDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
        IScopedServices
{
    private readonly EmployeeService _employeeService = employeeService;
    public Task<OperationResult> ResolveOrganisationChartIdAsync(long employeeId)
    {
        if (employeeId <= 0)
        {
            return Task.FromResult(OperationResult.Failed("کاربر جاری معتبر نیست"));
        }

        const string sql = """
            SELECT TOP 1 ro.PayLocationId
            FROM [Order].[Recruit_Order] ro
            INNER JOIN [Order].[Interdict_Order] i
                ON i.RecruitOrderId = ro.Id
                AND i.StatusId = 9
                AND i.IsDeleted = 0
            WHERE ro.EmployeeId = @EmployeeId
                AND ro.IsDeleted = 0
            ORDER BY i.Id DESC
            """;

        var parameters = new DynamicParameters();
        parameters.Add("@EmployeeId", employeeId, DbType.Int64);

        var payLocationId = dapper.Get<long?>(sql, parameters, CommandType.Text);
        if (payLocationId is null or <= 0)
        {
            return Task.FromResult(OperationResult.Failed("محل پرداخت کاربر از روی حکم جاری یافت نشد"));
        }

        return Task.FromResult(OperationResult.Succeeded(payload: payLocationId.Value));
    }

    public async Task<OperationResult> SubmitForEmployeeAsync(
        SubmitEmployeeRequestDTO dto,
        long employeeId,
        long organisationChartId,
        string requirementTitle,
        IReadOnlyDictionary<long, (string Title, bool IsRequired)> requirementDetails)
    {
        if (employeeId <= 0)
        {
            return OperationResult.Failed("کاربر جاری معتبر نیست");
        }

        if (dto.RequestDocumentRequirementId <= 0)
        {
            return OperationResult.Failed("نوع درخواست انتخاب نشده است");
        }

        if (dto.Details == null || dto.Details.Count == 0)
        {
            return OperationResult.Failed("حداقل یک سند باید ارسال شود");
        }

        foreach (var detail in requirementDetails.Where(x => x.Value.IsRequired))
        {
            var submitted = dto.Details.FirstOrDefault(x => x.RequestDocumentRequirementDetailId == detail.Key);
            if (submitted == null || submitted.FileId is null or <= 0)
            {
                return OperationResult.Failed($"بارگذاری سند «{detail.Value.Title}» الزامی است");
            }
        }

        var invalidDetail = dto.Details.FirstOrDefault(x =>
            !requirementDetails.ContainsKey(x.RequestDocumentRequirementDetailId));
        if (invalidDetail != null)
        {
            return OperationResult.Failed("یکی از اسناد ارسالی معتبر نیست");
        }

        var fileIds = dto.Details
            .Where(x => x.FileId.HasValue && x.FileId.Value > 0)
            .Select(x => x.FileId!.Value)
            .Distinct()
            .ToList();

        if (fileIds.Count > 0)
        {
            var existingFileCount = await _unitOfWork.Context.Files
                .CountAsync(x => fileIds.Contains(x.Id) && !x.IsDeleted);
            if (existingFileCount != fileIds.Count)
            {
                return OperationResult.Failed("یکی از فایل‌های ارسالی یافت نشد");
            }
        }

        try
        {
            var ipAddress = GetIpAddress();
            var now = DateTime.Now;
            var request = new EmployeeRequest
            {
                title = requirementTitle,
                EmployeeId = employeeId,
                OrganisationChartId = organisationChartId,
                RequestDocumentRequirementId = dto.RequestDocumentRequirementId,
                EmployeeRequestStatusId = 1,
                Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
                CreateDate = now,
                LastModifiedDate = now,
                IPAddress = ipAddress,
                IsDeleted = false
            };

            foreach (var detailDto in dto.Details.Where(x => x.FileId.HasValue && x.FileId.Value > 0))
            {
                var detailTitle = requirementDetails[detailDto.RequestDocumentRequirementDetailId].Title;
                request.Details.Add(new EmployeeRequestDetail
                {
                    title = detailTitle,
                    RequestDocumentRequirementDetailId = detailDto.RequestDocumentRequirementDetailId,
                    FileId = detailDto.FileId,
                    Description = string.IsNullOrWhiteSpace(detailDto.Description) ? null : detailDto.Description.Trim(),
                    CreateDate = now,
                    LastModifiedDate = now,
                    IPAddress = ipAddress,
                    IsDeleted = false
                });
            }

            _unitOfWork.Context.EmployeeRequests.Add(request);
            if (await _unitOfWork.Save() <= 0)
            {
                return OperationResult.Failed("خطا در ثبت درخواست");
            }

            return OperationResult.Succeeded(msg: "درخواست با موفقیت ثبت شد", payload: request.Id);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در ثبت درخواست: {ex.Message}");
        }
    }

    public OperationResult GetCartablePagedData(
        long currentUserId,
        int currentPage = 0,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string sortDirection = "",
        bool ignoreExpired = true)
    {
        var datasource = BuildAccessibleRequestsQuery(currentUserId);
        return GetPagedData(
            currentPage: currentPage,
            pageSize: pageSize,
            filter: filter,
            activeSortColumn: activeSortColumn,
            Sortdirection: sortDirection,
            IgnoreExpired: ignoreExpired,
            CustomDataSource: datasource,
            IgnoreDefaultOrganId: true);
    }

    public OperationResult GetForCartable(long id, long currentUserId)
    {
        if (id <= 0)
        {
            return OperationResult.Failed("درخواست نامعتبر است");
        }

        var request = _unitOfWork.Context.EmployeeRequests
            .AsNoTracking()
            .Include(x => x.Employee)
            .Include(x => x.RequestDocumentRequirement)
            .Include(x => x.EmployeeRequestStatus)
            .Include(x => x.Details.Where(d => !d.IsDeleted))
                .ThenInclude(d => d.File)
            .Include(x => x.Details.Where(d => !d.IsDeleted))
                .ThenInclude(d => d.RequestDocumentRequirementDetail)
            .FirstOrDefault(x => x.Id == id && !x.IsDeleted);

        if (request == null)
        {
            return OperationResult.NotFound();
        }

        if (!userService.IsAdmin() && !_employeeService.CheckAccess(currentUserId, request.EmployeeId))
        {
            return OperationResult.Failed("کاربر جاری به کارمند مورد نظر دسترسی ندارد");
        }

        return OperationResult.Succeeded(payload: _mapper.Map<EmployeeRequestDTO>(request));
    }

    public OperationResult DownloadFileForCartable(long fileId, long currentUserId)
    {
        if (fileId <= 0)
        {
            return OperationResult.Failed("درخواست نامعتبر است");
        }

        var detail = _unitOfWork.Context.EmployeeRequestDetails
            .AsNoTracking()
            .Include(d => d.EmployeeRequest)
            .Include(d => d.File)
            .FirstOrDefault(d =>
                d.FileId == fileId
                && !d.IsDeleted
                && d.EmployeeRequest != null
                && !d.EmployeeRequest.IsDeleted);

        if (detail?.EmployeeRequest == null || detail.File == null || detail.File.IsDeleted)
        {
            return OperationResult.Failed("فایل یافت نشد یا دسترسی مجاز نیست");
        }

        if (!userService.IsAdmin() && !_employeeService.CheckAccess(currentUserId, detail.EmployeeRequest.EmployeeId))
        {
            return OperationResult.Failed("کاربر جاری به کارمند مورد نظر دسترسی ندارد");
        }

        return OperationResult.Succeeded(payload: new EmployeeRequestFileDownloadDTO
        {
            FileName = detail.File.title,
            MimeType = detail.File.MimeType,
            Extension = detail.File.Extension,
            ContentBase64 = Convert.ToBase64String(detail.File.Content),
        });
    }

    private IQueryable<EmployeeRequest> BuildAccessibleRequestsQuery(long currentUserId)
    {
        IQueryable<EmployeeRequest> query = _unitOfWork.Context.EmployeeRequests
            .AsNoTracking()
            .Include(x => x.Employee)
            .Include(x => x.RequestDocumentRequirement)
            .Include(x => x.EmployeeRequestStatus)
            .Include(x => x.Details.Where(d => !d.IsDeleted))
                .ThenInclude(d => d.File)
            .Include(x => x.Details.Where(d => !d.IsDeleted))
                .ThenInclude(d => d.RequestDocumentRequirementDetail)
            .Where(x => !x.IsDeleted);

        if (!userService.IsAdmin())
        {
            var accessibleEmployeeIds = _employeeService.GetAccessibleEmployeeIds(currentUserId).ToList();
            if (!accessibleEmployeeIds.Any())
            {
                return query.Where(x => false);
            }

            query = query.Where(x => accessibleEmployeeIds.Contains(x.EmployeeId));
        }

        return query;
    }

    public OperationResult GetByEmployee(long employeeId)
    {
        var requests = _unitOfWork.Context.EmployeeRequests
            .Include(x => x.RequestDocumentRequirement)
            .Include(x => x.EmployeeRequestStatus)
            .Include(x => x.Details.Where(d => !d.IsDeleted))
                .ThenInclude(d => d.File)
            .Include(x => x.Details.Where(d => !d.IsDeleted))
                .ThenInclude(d => d.RequestDocumentRequirementDetail)
            .Where(x => x.EmployeeId == employeeId && !x.IsDeleted)
            .OrderByDescending(x => x.CreateDate)
            .ToList();

        return OperationResult.Succeeded(payload: _mapper.Map<List<EmployeeRequestDTO>>(requests));
    }

    public OperationResult DownloadFileForEmployee(long fileId, long employeeId)
    {
        if (fileId <= 0 || employeeId <= 0)
        {
            return OperationResult.Failed("درخواست نامعتبر است");
        }

        var file = _unitOfWork.Context.EmployeeRequestDetails
            .AsNoTracking()
            .Where(d =>
                d.FileId == fileId
                && !d.IsDeleted
                && d.EmployeeRequest != null
                && d.EmployeeRequest.EmployeeId == employeeId
                && !d.EmployeeRequest.IsDeleted)
            .Select(d => d.File)
            .FirstOrDefault(f => f != null && !f.IsDeleted);

        if (file == null)
        {
            return OperationResult.Failed("فایل یافت نشد یا دسترسی مجاز نیست");
        }

        return OperationResult.Succeeded(payload: new EmployeeRequestFileDownloadDTO
        {
            FileName = file.title,
            MimeType = file.MimeType,
            Extension = file.Extension,
            ContentBase64 = Convert.ToBase64String(file.Content),
        });
    }

    private string GetIpAddress()
    {
        try
        {
            var ipAddress = userService.GetIP();
            return string.IsNullOrEmpty(ipAddress) || ipAddress == "Notfound" ? "Local" : ipAddress;
        }
        catch
        {
            return "Local";
        }
    }
}
