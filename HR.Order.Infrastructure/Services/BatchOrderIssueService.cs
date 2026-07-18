using HR.Order.Core.Data;
using HR.Order.Core.DTOs;
using HR.Order.Infrastructure.Data;
using HR.SharedKernel.Excel;
using HR.SharedKernel.Share;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel;
using PdfSharp.Pdf.IO;
using PdfSharp.Pdf;
using Castle.Components.DictionaryAdapter.Xml;
using System.Security;
using HR.SharedKernel.Service;

namespace HR.Order.Infrastructure.Services;

public class BatchOrderIssueService : IScopedServices
{
    private OrderService _os;
    private BatchRequestService _batchRequestService;
    private OrderContext _ctx;
    public ILogger<BatchOrderIssueService> _logger;

    public BatchOrderIssueService(OrderService OrderService, BatchRequestService BatchRequestService, ILogger<BatchOrderIssueService> logger)
    {

        _os = OrderService;
        _ctx = (OrderContext)OrderService._db;
        //_aspNetUsersService = AspNetUsersService;
        _logger = logger;
        _batchRequestService = BatchRequestService;
    }
    public byte[] CombinePDFs(List<byte[]> srcPDFs)
    {
        using (var ms = new MemoryStream())
        {
            using (var resultPDF = new PdfDocument(ms))
            {
                foreach (var pdf in srcPDFs)
                {
                    using (var src = new MemoryStream(pdf))
                    {
                        using (var srcPDF = PdfReader.Open(src, PdfDocumentOpenMode.Import))
                        {
                            for (var i = 0; i < srcPDF.PageCount; i++)
                            {
                                resultPDF.AddPage(srcPDF.Pages[i]);
                            }
                        }
                    }
                }
                resultPDF.Save(ms);
                return ms.ToArray();
            }
        }
    }

    private void Archive()
    {
        try
        {
            List<long?> validStates = new List<long?>() { (long)Enums.BatchRequestArchiveState.ReArchive, (long)Enums.BatchRequestArchiveState.InitialArchive };
            var needToRunRequests = _ctx.BatchRequests.Where(i => validStates.Contains(i.ArchiveStateId)).ToList();
            foreach (var item in needToRunRequests)
            {
                try
                {
                    _ctx.Entry(item).Property("ArchiveStateId").CurrentValue = (long)Enums.BatchRequestArchiveState.InProgress;
                    item.ArchiveLastPoolingTime = DateTime.Now;
                    item.LastModifiedDate = DateTime.Now;
                    _ctx.BatchRequests.Update(item);
                    _ctx.SaveChanges();
                    var requestDetail = _ctx.BatchRequestDetails.Where(i => i.BatchRequestId == item.Id).ToList();
                    foreach (var detail in requestDetail.Where(i => i.InterdictId > 0))
                    {
                        try
                        {
                            item.ArchivePoolingEmployeeId = Convert.ToInt32(detail.EmployeeId);
                            item.LastModifiedDate = DateTime.Now;
                            _ctx.BatchRequests.Update(item);
                            _ctx.SaveChanges();

                            var Order = _ctx.InterdictOrders.Find(detail.InterdictId);
                            if (Order == null)
                            {
                                continue;
                            }

                            if (InterdictOrderArchivePersistence.Exists(_ctx, Order.Id))
                            {
                                continue;
                            }

                            byte[]? rawBytes = null;
                            byte[]? formattedBytes = null;
                            try
                            {
                                var pdfRawResult = _os.DownloadOrderPDF(Order.Id, true);
                                if (pdfRawResult.Success && pdfRawResult.Payload != null)
                                {
                                    rawBytes = (byte[])pdfRawResult.Payload;
                                    detail.PdfrawByteArrayFinalMessage = "OK";
                                }
                                else
                                {
                                    detail.PdfrawByteArrayFinalMessage = pdfRawResult.Message ?? "خطا در تولید PDF خام";
                                    _logger.LogError("خطا در تولید PDF خام حکم {OrderId}: {Message}", Order.Id, detail.PdfrawByteArrayFinalMessage);
                                }
                            }
                            catch (Exception ex)
                            {
                                detail.PdfrawByteArrayFinalMessage = ex.Message;
                                _logger.LogError(ex, ex.Message);
                            }
                            try
                            {
                                var pdfResult = _os.DownloadOrderPDF(Order.Id, false);
                                if (pdfResult.Success && pdfResult.Payload != null)
                                {
                                    formattedBytes = (byte[])pdfResult.Payload;
                                    detail.PdfByteArrayFinalMessage = "OK";
                                }
                                else
                                {
                                    detail.PdfByteArrayFinalMessage = pdfResult.Message ?? "خطا در تولید PDF قالب‌دار";
                                    _logger.LogError("خطا در تولید PDF قالب‌دار حکم {OrderId}: {Message}", Order.Id, detail.PdfByteArrayFinalMessage);
                                }
                            }
                            catch (Exception ex)
                            {
                                detail.PdfByteArrayFinalMessage = ex.Message;
                                _logger.LogError(ex, ex.Message);
                            }

                            if (rawBytes == null && formattedBytes == null)
                            {
                                detail.ArchiveFinalMessage = "هیچ فایل PDF برای آرشیو تولید نشد";
                                _ctx.BatchRequestDetails.Update(detail);
                                _ctx.SaveChanges();
                                continue;
                            }

                            var arch = InterdictOrderArchivePersistence.Replace(
                                _ctx,
                                Order.Id,
                                rawBytes,
                                formattedBytes,
                                "Backgroud");
                            detail.ArchiveFinalMessage = " ثبت موفق با شناسه : " + arch.Id;
                            _ctx.BatchRequestDetails.Update(detail);
                            _ctx.SaveChanges();
                        }
                        catch (Exception ex)
                        {
                            detail.ArchiveFinalMessage = ex.Message;
                            _ctx.BatchRequestDetails.Update(detail);
                            _ctx.SaveChanges();
                            _logger.LogError(ex, "خطا در شماره درخواست آرشیو گروهی : " + item.Id + " برای شناسه کارمندی :  " + detail.EmployeeId);
                        }
                    }

                    _ctx.Entry(item).Property("ArchiveStateId").CurrentValue = (long)Enums.BatchRequestArchiveState.EndLoop;
                    item.FinishDateTime = DateTime.Now;
                    item.LastModifiedDate = DateTime.Now;
                    _ctx.BatchRequests.Update(item);
                    _ctx.SaveChanges();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در شماره درخواست آرشیو گروهی : " + item.Id);
                }
                _ctx.Entry(item).Property("ArchiveStateId").CurrentValue = (long)Enums.BatchRequestArchiveState.EndLoop;

                _ctx.BatchRequests.Update(item);
                _ctx.SaveChanges();



                try
                {


                    var results = new List<InterdictOrderArchive>();

                    using (SqlConnection con = new SqlConnection(_ctx.Database.GetConnectionString()))
                    {
                        SqlCommand cmd = new SqlCommand("[Order].[GetBatchRequestPdfrawByteArray]", con);
                        cmd.Parameters.AddWithValue("@BatchRequestId", item.Id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        var retList = new List<GetBatchRequestFiles_Result>();
                        while (rdr.Read())
                        {
                            results.Add(rdr.ConvertToObject<InterdictOrderArchive>());
                        }
                    }

                    if (results != null)
                    {
                        if (results.Any())
                        {
                            var combined = CombinePDFs(results.Select(i => i.PdfrawByteArray).ToList());
                            var newMergedFile = new BatchRequestFile()
                            {
                                Content = combined,
                                CreateDate = DateTime.Now,
                                Size = combined.Length,
                                MimeType = Helper.GetMimeType(".pdf"),
                                Description = "تعداد : " + results.Count(),
                                BatchRequestId = item.Id,
                                IPAddress = "Background",
                                IsDeleted = false,
                                UniqueId = Guid.NewGuid(),
                                title = "فایل_تجمعی_شماره_درخواست_" + item.Id + ".pdf"
                            };
                            _ctx.BatchRequestFiles.Add(newMergedFile);
                            _ctx.Entry(newMergedFile).Property("FileTypeId").CurrentValue = (long)Enums.BatchRequestFileType.PdfrawByteArrayCombined;
                            _ctx.SaveChanges();
                        }
                    }


                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در ادغام فایل های خام" + item.Id);
                }

                try
                {
                    var results = new List<InterdictOrderArchive>();

                    using (SqlConnection con = new SqlConnection(_ctx.Database.GetConnectionString()))
                    {
                        SqlCommand cmd = new SqlCommand("[Order].[GetBatchRequestPdfByteArray]", con);
                        cmd.Parameters.AddWithValue("@BatchRequestId", item.Id);
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();
                        SqlDataReader rdr = cmd.ExecuteReader();
                        var retList = new List<GetBatchRequestFiles_Result>();
                        while (rdr.Read())
                        {
                            results.Add(rdr.ConvertToObject<InterdictOrderArchive>());
                        }
                    }
                    if (results != null)
                    {
                        if (results.Any())
                        {
                            var combined = CombinePDFs(results.Select(i => i.PdfbyteArray).ToList());
                            var newMergedFile = new BatchRequestFile()
                            {
                                Content = combined,
                                CreateDate = DateTime.Now,
                                Size = combined.Length,
                                MimeType = Helper.GetMimeType(".pdf"),
                                Description = "تعداد : " + results.Count(),
                                BatchRequestId = item.Id,
                                IPAddress = "Background",
                                IsDeleted = false,
                                UniqueId = Guid.NewGuid(),
                                title = "فایل_تجمعی_شماره_درخواست_" + item.Id + ".pdf"
                            };
                            _ctx.BatchRequestFiles.Add(newMergedFile);
                            _ctx.Entry(newMergedFile).Property("FileTypeId").CurrentValue = (long)Enums.BatchRequestFileType.PdfbyteArrayCombined;
                            _ctx.SaveChanges();
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "خطا در ادغام فایل های قالب دار" + item.Id);
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در سرویس آرشیو گروهی احکام واکشی");
        }

    }

    public void DoWork(object state)
    {
        var startTime = DateTime.Now;
        
        try
        {
            _ctx.ChangeTracker.Clear();
            _logger.LogInformation("شروع سرکشی سرویس پس زمینه حکم در تاریخ {DateTime}", DateTime.Now);

            List<long> validStates = new List<long>() 
            { 
                (long)Enums.BatchRequestState.Resume, 
                (long)Enums.BatchRequestState.Runing, 
                (long)Enums.BatchRequestState.Initial 
            };
            
            var needToRunRequests = _ctx.BatchRequests
                .Where(i => validStates.Contains(i.RequestStateId))
                .ToList();
            
            _logger.LogInformation("تعداد {Count} درخواست برای پردازش یافت شد", needToRunRequests.Count);

            if (!needToRunRequests.Any())
            {
                _logger.LogInformation("هیچ درخواستی برای پردازش یافت نشد");
                return;
            }

            foreach (var item in needToRunRequests)
            {
                var requestStartTime = DateTime.Now;
                _logger.LogInformation("شروع پردازش درخواست گروهی شماره {RequestId}", item.Id);

                try
                {
                    // به‌روزرسانی وضعیت درخواست به در حال اجرا
                    try
                    {
                        _ctx.Entry(item).Property("RequestStateId").CurrentValue = (long)Enums.BatchRequestState.Runing;
                        item.LastPoolingTime = DateTime.Now;
                        item.LastModifiedDate = DateTime.Now;
                        _ctx.BatchRequests.Update(item);
                        _ctx.SaveChanges();
                        _logger.LogDebug("وضعیت درخواست {RequestId} به 'در حال اجرا' تغییر یافت", item.Id);
                    }
                    catch (Exception updateEx)
                    {
                        _logger.LogError(updateEx, "خطا در به‌روزرسانی وضعیت درخواست {RequestId} به 'در حال اجرا'", item.Id);
                        throw;
                    }

                    // بارگذاری فایل‌های Excel مرتبط
                    _batchRequestService._currentUserDefaultOrganId = item.OrganisationChartId;
                    _os._currentUserDefaultOrganId = item.OrganisationChartId;

                    List<BatchGridModelForExcel> wageOverRideExcel = new List<BatchGridModelForExcel>();
                    List<BatchGridModelForExcel> coefOverRideExcel = new List<BatchGridModelForExcel>();
                    var datesFromExcel = false;
                    var datesFromWageExcel = item.DatesFromWageExcel;
                    
                    try
                    {
                        var batchRequestFiles = _ctx.BatchRequestFiles
                            .Where(f => f.BatchRequestId == item.Id)
                            .ToList();
                        
                        if (batchRequestFiles.Any())
                        {
                            _logger.LogDebug("تعداد {Count} فایل برای درخواست {RequestId} یافت شد", batchRequestFiles.Count, item.Id);
                            
                            var wageFile = batchRequestFiles
                                .FirstOrDefault(i => i.FileTypeId == (long)Enums.BatchRequestFileType.WageItemsExcel);
                            
                            if (wageFile != null)
                            {
                                try
                                {
                                    var friendlyWageExcel = _batchRequestService.getWageExcelPreview(wageFile.Id, false, item.UseMappedExcelColumns);
                                    if (friendlyWageExcel.Success == true && friendlyWageExcel.Payload != null)
                                    {
                                        wageOverRideExcel = (List<BatchGridModelForExcel>)friendlyWageExcel.Payload;
                                        _logger.LogDebug("تعداد {Count} ردیف اکسل عوامل حقوقی برای درخواست {RequestId} بارگذاری شد", wageOverRideExcel.Count, item.Id);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("خطا در بارگذاری فایل Excel آیتم‌های حقوقی برای درخواست {RequestId}: {Message}", 
                                            item.Id, friendlyWageExcel.Message);
                                    }
                                }
                                catch (Exception excelEx)
                                {
                                    _logger.LogError(excelEx, "خطا در پردازش فایل Excel آیتم‌های حقوقی برای درخواست {RequestId}", item.Id);
                                }
                            }
                            
                            var coefFile = batchRequestFiles
                                .FirstOrDefault(i => i.FileTypeId == (long)Enums.BatchRequestFileType.CoefficientsExcel);
                            
                            if (coefFile != null)
                            {
                                try
                                {
                                    var friendlyCoefExcel = _batchRequestService.getCoefExcelPreview(coefFile.Id, false, item.UseMappedExcelColumns);
                                    if (friendlyCoefExcel.Success == true && friendlyCoefExcel.Payload != null)
                                    {
                                        coefOverRideExcel = (List<BatchGridModelForExcel>)friendlyCoefExcel.Payload;
                                        _logger.LogDebug("فایل Excel ضریب‌ها برای درخواست {RequestId} با موفقیت بارگذاری شد", item.Id);
                                    }
                                    else
                                    {
                                        _logger.LogWarning("خطا در بارگذاری فایل Excel ضریب‌ها برای درخواست {RequestId}: {Message}", 
                                            item.Id, friendlyCoefExcel.Message);
                                    }
                                }
                                catch (Exception excelEx)
                                {
                                    _logger.LogError(excelEx, "خطا در پردازش فایل Excel ضریب‌ها برای درخواست {RequestId}", item.Id);
                                }
                            }

                            datesFromExcel = batchRequestFiles.Any(i => i.FileTypeId == (long)Enums.BatchRequestFileType.OrderDatesExcel);
                            datesFromWageExcel = item.DatesFromWageExcel && wageFile != null;
                        }
                    }
                    catch (Exception fileEx)
                    {
                        _logger.LogError(fileEx, "خطا در بارگذاری فایل‌های Excel برای درخواست {RequestId}", item.Id);
                        // ادامه پردازش حتی در صورت خطا در بارگذاری فایل‌ها
                    }

                    // دریافت جزئیات درخواست
                    var requestDetail = _ctx.BatchRequestDetails
                        .Where(i => i.BatchRequestId == item.Id)
                        .ToList();
                    
                    _logger.LogInformation("تعداد {Count} جزئیات برای درخواست {RequestId} یافت شد", requestDetail.Count, item.Id);

                    if (!requestDetail.Any())
                    {
                        _logger.LogWarning("هیچ جزئیاتی برای درخواست {RequestId} یافت نشد", item.Id);
                    }

                    int successCount = 0;
                    int errorCount = 0;
                    int skippedCount = 0;

                    foreach (var detail in requestDetail)
                    {
                        var detailStartTime = DateTime.Now;
                        _logger.LogDebug("شروع پردازش جزئیات {DetailId} برای کارمند {EmployeeId} در درخواست {RequestId}", 
                            detail.Id, detail.EmployeeId, item.Id);

                        try
                        {
                            // بررسی اینکه آیا قبلاً پردازش شده است
                            if (detail.InterdictId > 0)
                            {
                                _logger.LogDebug("جزئیات {DetailId} برای کارمند {EmployeeId} قبلاً پردازش شده است (InterdictId: {InterdictId})", 
                                    detail.Id, detail.EmployeeId, detail.InterdictId);
                                skippedCount++;
                                continue;
                            }

                            // به‌روزرسانی وضعیت درخواست با شناسه کارمند فعلی
                            try
                            {
                                item.PoolingEmployeeId = Convert.ToInt32(detail.EmployeeId);
                                item.LastModifiedDate = DateTime.Now;
                                _ctx.BatchRequests.Update(item);
                                _ctx.SaveChanges();
                            }
                            catch (Exception updateEx)
                            {
                                _logger.LogWarning(updateEx, "خطا در به‌روزرسانی PoolingEmployeeId برای درخواست {RequestId}", item.Id);
                            }

                            // به‌روزرسانی زمان آخرین تلاش
                            detail.LastTryDateTime = DateTime.Now;
                            detail.LastModifiedDate = DateTime.Now;
                            _ctx.BatchRequestDetails.Update(detail);
                            _ctx.SaveChanges();

                            // دریافت آخرین حکم فعال
                            long lastorderId = 0;
                            var employeeStartDate = (datesFromExcel || datesFromWageExcel) ? detail.StartDate : item.StartDate;
                            var employeeEndDate = (datesFromExcel || datesFromWageExcel) ? detail.EndDate : item.EndDate;

                            if (!employeeStartDate.HasValue)
                            {
                                var errorMsg = datesFromExcel || datesFromWageExcel
                                    ? datesFromWageExcel
                                        ? "تاریخ اجرای حکم برای این کارمند در اکسل عوامل حقوقی یافت نشد"
                                        : "تاریخ اجرای حکم برای این کارمند در فایل اکسل یافت نشد"
                                    : "تاریخ شروع برای درخواست تعریف نشده است";
                                _logger.LogError(errorMsg + " - درخواست {RequestId}", item.Id);
                                SaveErrorToDetail(detail, errorMsg, item.Id);
                                errorCount++;
                                continue;
                            }

                            try
                            {
                                lastorderId = _os.GetLastOrderByImpleDate(
                                    detail.EmployeeId, 
                                    employeeStartDate.Value, 
                                    0, 
                                    item.OrganisationChartId);
                                
                                _logger.LogDebug("آخرین حکم فعال برای کارمند {EmployeeId}: {LastOrderId}", detail.EmployeeId, lastorderId);
                            }
                            catch (Exception getOrderEx)
                            {
                                var errorMsg = $"خطا در دریافت آخرین حکم فعال: {getOrderEx.Message}";
                                if (getOrderEx.InnerException != null)
                                {
                                    errorMsg += $" | InnerException: {getOrderEx.InnerException.Message}";
                                }
                                _logger.LogError(getOrderEx, errorMsg + " - کارمند {EmployeeId} در درخواست {RequestId}", 
                                    detail.EmployeeId, item.Id);
                                SaveErrorToDetail(detail, errorMsg, item.Id);
                                errorCount++;
                                continue;
                            }

                            if (lastorderId <= 0)
                            {
                                var errorMsg = item.ForceRecruitIssue
                                    ? "حکم فعالی یافت نشد. برای اولین حکم، نوع استخدام، وضعیت استخدام و مرکز هزینه را در مرحله انتخاب افراد مشخص کنید و گزینه حکم استخدامی را فعال نگه دارید."
                                    : "با توجه به تاریخ اجرای انتخابی حکم فعالی یافت نشد";
                                _logger.LogWarning(errorMsg + " - کارمند {EmployeeId} در درخواست {RequestId}",
                                    detail.EmployeeId, item.Id);
                                SaveErrorToDetail(detail, errorMsg, item.Id);
                                errorCount++;
                                continue;
                            }

                            // دریافت اطلاعات حکم
                            InterdictOrderFlatDTO? InterdictOrderDTO = null;
                            try
                            {
                                var orderFlatResult = _os.GetOrderFlat(lastorderId);
                                if (orderFlatResult == null || !orderFlatResult.Success || orderFlatResult.Payload == null)
                                {
                                    var errorMsg = $"خطا در دریافت اطلاعات حکم {lastorderId}: {orderFlatResult?.Message ?? "نتیجه نامعتبر"}";
                                    _logger.LogError(errorMsg + " - کارمند {EmployeeId} در درخواست {RequestId}",
                                        detail.EmployeeId, item.Id);
                                    SaveErrorToDetail(detail, errorMsg, item.Id);
                                    errorCount++;
                                    continue;
                                }
                                InterdictOrderDTO = orderFlatResult.Payload;
                                _logger.LogDebug("اطلاعات حکم {LastOrderId} برای کارمند {EmployeeId} با موفقیت دریافت شد",
                                    lastorderId, detail.EmployeeId);
                            }
                            catch (Exception getOrderFlatEx)
                            {
                                var errorMsg = $"خطا در دریافت اطلاعات حکم {lastorderId}: {getOrderFlatEx.Message}";
                                if (getOrderFlatEx.InnerException != null)
                                {
                                    errorMsg += $" | InnerException: {getOrderFlatEx.InnerException.Message}";
                                }
                                _logger.LogError(getOrderFlatEx, errorMsg + " - کارمند {EmployeeId} در درخواست {RequestId}",
                                    detail.EmployeeId, item.Id);
                                SaveErrorToDetail(detail, errorMsg, item.Id);
                                errorCount++;
                                continue;
                            }

                            // بررسی مجوز صدور
                            long userId = 0;
                            bool hasPermission = false;
                            string? permissionMessage = null;
                            
                            try
                            {
                                var permissionResult = _os.GetIssuePermission(new GetOrderListByEmployeeIdRequest()
                                {
                                    EmployeeId = detail.EmployeeId,
                                    EmployeeTypeId = InterdictOrderDTO.EmployeeTypeId,
                                    OrderTypeId = item.OrderTypeId,
                                    CurrentUserId = userId,
                                    PayLocationId = item.OrganisationChartId,
                                    StartDate = employeeStartDate,
                                    EndDate = employeeEndDate,
                                    PageNo = 0,
                                    PageSize = 1000,
                                    SortColumn = "",
                                    SortOrder = ""
                                });

                                hasPermission = permissionResult.Item1;
                                permissionMessage = permissionResult.Item2;
                                
                                _logger.LogDebug("نتیجه بررسی مجوز برای کارمند {EmployeeId}: {HasPermission}", 
                                    detail.EmployeeId, hasPermission);
                            }
                            catch (Exception permissionEx)
                            {
                                var errorMsg = $"خطا در بررسی مجوز صدور: {permissionEx.Message}";
                                if (permissionEx.InnerException != null)
                                {
                                    errorMsg += $" | InnerException: {permissionEx.InnerException.Message}";
                                }
                                _logger.LogError(permissionEx, errorMsg + " - کارمند {EmployeeId} در درخواست {RequestId}", 
                                    detail.EmployeeId, item.Id);
                                SaveErrorToDetail(detail, errorMsg, item.Id);
                                errorCount++;
                                continue;
                            }

                            if (!hasPermission)
                            {
                                var errorMsg = permissionMessage ?? "مجوز صدور حکم وجود ندارد";
                                _logger.LogWarning(errorMsg + " - کارمند {EmployeeId} در درخواست {RequestId}", 
                                    detail.EmployeeId, item.Id);
                                SaveErrorToDetail(detail, errorMsg, item.Id);
                                errorCount++;
                                continue;
                            }

                            // صدور حکم
                            try
                            {
                                // در پس‌زمینه HttpContext وجود ندارد؛ ارگان و کاربر باید صریح تنظیم شوند تا IssueOrder مانند فراخوانی از API رفتار کند.
                                _os._currentUserDefaultOrganId = item.OrganisationChartId;

                                _logger.LogDebug("شروع صدور حکم برای کارمند {EmployeeId} در درخواست {RequestId}",
                                    detail.EmployeeId, item.Id);

                                // UserId برای GetOrderList و AspNetUsersId حکم استفاده می‌شود (در Batch بدون HttpContext مقدار _userService.GetUserId() برابر -1 است).
                                var batchUserId = item.AspNetUsersId; // شناسه کاربر سرویس/سیستم برای صدور گروهی؛ در صورت وجود فیلد CreatorUserId در BatchRequest می‌توان از آن استفاده کرد.
                                var issueResponse = _os.IssueOrder(new IssueOrderRequest()
                                {
                                    EmployeeId = detail.EmployeeId,
                                    wageOverRideExcel = wageOverRideExcel,
                                    coefOverRideExcel = coefOverRideExcel,
                                    IsBatch = true,
                                    DoFinalCalc = true,
                                    IssueTypeId = (long)Enums.IssueType.Normal,
                                    StatusId = item.SendToCartable == true ? (int)Enums.OrderStatus.Pending : (int)Enums.OrderStatus.Draft,
                                    StartDate = employeeStartDate,
                                    EndDate = employeeEndDate,
                                    OrderTypeId = item.OrderTypeId,
                                    PayLocationId = item.OrganisationChartId,
                                    CostCenterId = InterdictOrderDTO.CostCenterId,
                                    EmployeeTypeId = InterdictOrderDTO.EmployeeTypeId,
                                    IgnoreEqualToInputesInBatch = item.IgnoreEqualToInputes,
                                    EmployeeStatusId = InterdictOrderDTO.EmployeeStatusId,
                                    lastorderId = lastorderId,
                                    OrderLevelId = (long)Enums.OrderLevel.interdict,
                                    WorkPlaceId = InterdictOrderDTO.WorkPlaceId,
                                    OrganizationUnitId = InterdictOrderDTO.OrganizationUnitId,
                                    OrganizationJobId = InterdictOrderDTO.OrganizationJobId,
                                    InterdictOrderDTO = InterdictOrderDTO,
                                    UserId = batchUserId
                                });

                                if (issueResponse == null)
                                {
                                    var errorMsg = "نتیجه صدور حکم نامعتبر است (null)";
                                    _logger.LogError(errorMsg + " - کارمند {EmployeeId} در درخواست {RequestId}", 
                                        detail.EmployeeId, item.Id);
                                    SaveErrorToDetail(detail, errorMsg, item.Id);
                                    errorCount++;
                                    continue;
                                }

                                if (issueResponse.Success == true && issueResponse.Payload != null)
                                {
                                    try
                                    {
                                        detail.InterdictId = Convert.ToInt64(issueResponse.Payload);
                                        if (detail.InterdictId == 0)
                                        {
                                            detail.InterdictId = null;
                                        }

                                        if (detail.InterdictId > 0)
                                        {
                                            detail.FinalMessage = $"ثبت موفق - شناسه حکم: {detail.InterdictId}";
                                            detail.DoDatetime = DateTime.Now;
                                            _ctx.BatchRequestDetails.Update(detail);
                                            _ctx.SaveChanges();

                                            item.SuccessCount = item.SuccessCount + 1;
                                            _ctx.BatchRequests.Update(item);
                                            _ctx.SaveChanges();

                                            successCount++;
                                            var duration = (DateTime.Now - detailStartTime).TotalMilliseconds;
                                            _logger.LogInformation("صدور حکم برای کارمند {EmployeeId} با موفقیت انجام شد. شناسه حکم: {InterdictId} - مدت زمان: {Duration}ms", 
                                                detail.EmployeeId, detail.InterdictId, duration);
                                        }
                                        else
                                        {
                                            var errorMsg = "شناسه حکم صادر شده نامعتبر است (صفر یا منفی)";
                                            _logger.LogWarning(errorMsg + " - کارمند {EmployeeId} در درخواست {RequestId}", 
                                                detail.EmployeeId, item.Id);
                                            SaveErrorToDetail(detail, errorMsg, item.Id);
                                            errorCount++;
                                        }
                                    }
                                    catch (Exception saveSuccessEx)
                                    {
                                        var errorMsg = $"خطا در ذخیره نتیجه موفق: {saveSuccessEx.Message}";
                                        if (saveSuccessEx.InnerException != null)
                                        {
                                            errorMsg += $" | InnerException: {saveSuccessEx.InnerException.Message}";
                                        }
                                        _logger.LogError(saveSuccessEx, errorMsg + " - کارمند {EmployeeId} در درخواست {RequestId}", 
                                            detail.EmployeeId, item.Id);
                                        SaveErrorToDetail(detail, errorMsg, item.Id);
                                        errorCount++;
                                    }
                                }
                                else
                                {
                                    var errorMsg = issueResponse.Message ?? "خطا در صدور حکم (بدون پیام مشخص)";
                                    _logger.LogWarning("صدور حکم برای کارمند {EmployeeId} ناموفق بود: {Message}", 
                                        detail.EmployeeId, errorMsg);
                                    SaveErrorToDetail(detail, errorMsg, item.Id);
                                    errorCount++;
                                }
                            }
                            catch (Exception issueEx)
                            {
                                var errorMsg = $"خطا در فرآیند صدور حکم: {issueEx.Message}";
                                if (issueEx.InnerException != null)
                                {
                                    errorMsg += $" | InnerException: {issueEx.InnerException.Message}";
                                }
                                if (!string.IsNullOrEmpty(issueEx.StackTrace))
                                {
                                    errorMsg += $" | StackTrace: {issueEx.StackTrace.Substring(0, Math.Min(500, issueEx.StackTrace.Length))}";
                                }
                                _logger.LogError(issueEx, errorMsg + " - کارمند {EmployeeId} در درخواست {RequestId}", 
                                    detail.EmployeeId, item.Id);
                                SaveErrorToDetail(detail, errorMsg, item.Id);
                                errorCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            var errorMsg = $"خطای غیرمنتظره در پردازش جزئیات: {ex.Message}";
                            if (ex.InnerException != null)
                            {
                                errorMsg += $" | InnerException: {ex.InnerException.Message}";
                            }
                            if (!string.IsNullOrEmpty(ex.StackTrace))
                            {
                                errorMsg += $" | StackTrace: {ex.StackTrace.Substring(0, Math.Min(500, ex.StackTrace.Length))}";
                            }
                            
                            _logger.LogError(ex, "خطای غیرمنتظره در پردازش جزئیات {DetailId} برای کارمند {EmployeeId} در درخواست {RequestId}", 
                                detail.Id, detail.EmployeeId, item.Id);
                            
                            try
                            {
                                SaveErrorToDetail(detail, errorMsg, item.Id);
                                errorCount++;
                            }
                            catch (Exception saveErrorEx)
                            {
                                _logger.LogError(saveErrorEx, "خطا در ذخیره پیام خطا برای جزئیات {DetailId} در درخواست {RequestId}", 
                                    detail.Id, item.Id);
                            }
                        }
                    }

                    // به‌روزرسانی وضعیت نهایی درخواست
                    try
                    {
                        _ctx.Entry(item).Property("RequestStateId").CurrentValue = (long)Enums.BatchRequestState.EndLoop;
                        item.FinishDateTime = DateTime.Now;
                        item.LastModifiedDate = DateTime.Now;
                        _ctx.BatchRequests.Update(item);
                        _ctx.SaveChanges();
                        
                        var requestDuration = (DateTime.Now - requestStartTime).TotalSeconds;
                        _logger.LogInformation("پایان پردازش درخواست {RequestId} - موفق: {SuccessCount}, خطا: {ErrorCount}, رد شده: {SkippedCount} - مدت زمان: {Duration}ثانیه", 
                            item.Id, successCount, errorCount, skippedCount, requestDuration);
                    }
                    catch (Exception finalUpdateEx)
                    {
                        _logger.LogError(finalUpdateEx, "خطا در به‌روزرسانی وضعیت نهایی درخواست {RequestId}", item.Id);
                        
                        // تلاش مجدد با context پاک شده
                        try
                        {
                            _ctx.ChangeTracker.Clear();
                            var requestToUpdate = _ctx.BatchRequests.Find(item.Id);
                            if (requestToUpdate != null)
                            {
                                _ctx.Entry(requestToUpdate).Property("RequestStateId").CurrentValue = (long)Enums.BatchRequestState.EndLoop;
                                requestToUpdate.FinishDateTime = DateTime.Now;
                                requestToUpdate.LastModifiedDate = DateTime.Now;
                                _ctx.BatchRequests.Update(requestToUpdate);
                                _ctx.SaveChanges();
                                _logger.LogInformation("وضعیت نهایی درخواست {RequestId} با context جدید ذخیره شد", item.Id);
                            }
                        }
                        catch (Exception retryEx)
                        {
                            _logger.LogError(retryEx, "خطا در تلاش مجدد برای ذخیره وضعیت نهایی درخواست {RequestId}", item.Id);
                        }
                    }
                }
                catch (Exception ex)
                {
                    var errorMsg = $"خطای کلی در پردازش درخواست {item.Id}: {ex.Message}";
                    if (ex.InnerException != null)
                    {
                        errorMsg += $" | InnerException: {ex.InnerException.Message}";
                    }
                    
                    _logger.LogError(ex, "خطای کلی در پردازش درخواست گروهی {RequestId}", item.Id);
                    
                    // تلاش برای ذخیره وضعیت خطا
                    try
                    {
                        _ctx.ChangeTracker.Clear();
                        var requestToUpdate = _ctx.BatchRequests.Find(item.Id);
                        if (requestToUpdate != null)
                        {
                            _ctx.Entry(requestToUpdate).Property("RequestStateId").CurrentValue = (long)Enums.BatchRequestState.EndLoop;
                            requestToUpdate.FinishDateTime = DateTime.Now;
                            requestToUpdate.LastModifiedDate = DateTime.Now;
                            _ctx.BatchRequests.Update(requestToUpdate);
                            _ctx.SaveChanges();
                        }
                    }
                    catch (Exception saveStateEx)
                    {
                        _logger.LogError(saveStateEx, "خطا در ذخیره وضعیت خطا برای درخواست {RequestId}", item.Id);
                    }
                }
            }

            var totalDuration = (DateTime.Now - startTime).TotalSeconds;
            _logger.LogInformation("پایان سرکشی سرویس پس زمینه حکم - مدت زمان کل: {Duration}ثانیه", totalDuration);
        }
        catch (Exception ex)
        {
            var errorMsg = $"خطای بحرانی در سرویس صدور گروهی احکام: {ex.Message}";
            if (ex.InnerException != null)
            {
                errorMsg += $" | InnerException: {ex.InnerException.Message}";
            }
            if (!string.IsNullOrEmpty(ex.StackTrace))
            {
                errorMsg += $" | StackTrace: {ex.StackTrace.Substring(0, Math.Min(1000, ex.StackTrace.Length))}";
            }
            
            _logger.LogError(ex, "خطای بحرانی در سرویس صدور گروهی احکام واکشی");
        }
    }

    /// <summary>
    /// ذخیره پیام خطا در جدول BatchRequestDetail
    /// </summary>
    private void SaveErrorToDetail(BatchRequestDetail detail, string errorMessage, long requestId)
    {
        // محدود کردن طول پیام خطا در صورت نیاز
        const int maxMessageLength = 2000;
        
        try
        {
            if (errorMessage.Length > maxMessageLength)
            {
                errorMessage = errorMessage.Substring(0, maxMessageLength) + "...";
            }

            detail.FinalMessage = errorMessage;
            detail.LastTryDateTime = DateTime.Now;
            detail.LastModifiedDate = DateTime.Now;
            
            _ctx.BatchRequestDetails.Update(detail);
            _ctx.SaveChanges();
            
            _logger.LogDebug("پیام خطا برای جزئیات {DetailId} (کارمند {EmployeeId}) در درخواست {RequestId} ذخیره شد", 
                detail.Id, detail.EmployeeId, requestId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در ذخیره پیام خطا برای جزئیات {DetailId} (کارمند {EmployeeId}) در درخواست {RequestId}", 
                detail.Id, detail.EmployeeId, requestId);
            
            // تلاش مجدد با context پاک شده
            try
            {
                _ctx.ChangeTracker.Clear();
                var detailToUpdate = _ctx.BatchRequestDetails.Find(detail.Id);
                if (detailToUpdate != null)
                {
                    detailToUpdate.FinalMessage = errorMessage.Length > maxMessageLength 
                        ? errorMessage.Substring(0, maxMessageLength) + "..." 
                        : errorMessage;
                    detailToUpdate.LastTryDateTime = DateTime.Now;
                    detailToUpdate.LastModifiedDate = DateTime.Now;
                    _ctx.BatchRequestDetails.Update(detailToUpdate);
                    _ctx.SaveChanges();
                    _logger.LogInformation("پیام خطا برای جزئیات {DetailId} با context جدید ذخیره شد", detail.Id);
                }
            }
            catch (Exception retryEx)
            {
                _logger.LogError(retryEx, "خطا در تلاش مجدد برای ذخیره پیام خطا برای جزئیات {DetailId}", detail.Id);
            }
        }
    }

}
