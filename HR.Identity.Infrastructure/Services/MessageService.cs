using AutoMapper;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using System.Linq;

using Microsoft.Extensions.Configuration;

namespace HR.Identity.infrastructure.Services;

public class MessageService : BaseService<Message, IdentityContext, MessageDTO>, IScopedServices
{
    private readonly UserResolverService _userService;

    public MessageService(IMapper mapper, IUnitOfWork<IdentityContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) 
        : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// دریافت صندوق ورودی کاربر جاری با پشتیبانی از pagination
    /// </summary>
    /// <param name="showAsReceiver">اگر true باشد، فقط پیام‌هایی که کاربر گیرنده است نمایش داده می‌شود. اگر false باشد، فقط پیام‌هایی که کاربر فرستنده است نمایش داده می‌شود.</param>
    /// <param name="currentPage">شماره صفحه (از 0 شروع می‌شود)</param>
    /// <param name="pageSize">تعداد رکورد در هر صفحه</param>
    /// <param name="filter">فیلتر جستجو</param>
    /// <param name="activeSortColumn">ستون مرتب‌سازی</param>
    /// <param name="sortDirection">جهت مرتب‌سازی (asc یا desc)</param>
    /// <param name="userId">فیلتر بر اساس شناسه کاربر (null = همه کاربران)</param>
    /// <param name="fromDate">فیلتر از تاریخ (null = بدون محدودیت)</param>
    /// <param name="toDate">فیلتر تا تاریخ (null = بدون محدودیت)</param>
    public OperationResult GetInbox(bool showAsReceiver = true, int currentPage = 0, int pageSize = 10, string filter = "", string activeSortColumn = "", string sortDirection = "", long? userId = null, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var currentUserId = _userService.GetUserId();
        
        // دریافت پیام‌ها بر اساس نوع فیلتر
        // فقط thread root messages (پیام‌هایی که ParentMessageId = null دارند)
        var query = _unitOfWork.Context.Set<Message>()
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Include(m => m.Attachments)
            .Include(m => m.Replies)
            .Where(m => !m.IsDeleted && !m.ParentMessageId.HasValue); // فقط thread root messages

        // فیلتر بر اساس گیرنده یا فرستنده
        if (showAsReceiver)
        {
            // در صندوق ورودی: نمایش thread هایی که در آن‌ها پیامی وجود دارد که کاربر receiver آن است
            // این شامل:
            // 1. Thread root هایی که کاربر receiver است
            // 2. Thread root هایی که کاربر sender است و در thread reply جدیدی دریافت کرده است
            
            // پیدا کردن تمام thread root ID هایی که در thread آن‌ها پیامی وجود دارد که کاربر receiver آن است
            var allMessagesWhereUserIsReceiver = _unitOfWork.Context.Set<Message>()
                .Where(m => !m.IsDeleted && m.ReceiverId == currentUserId)
                .Select(m => m.ThreadRootMessageId ?? m.Id)
                .Distinct()
                .ToList();

            query = query.Where(m => allMessagesWhereUserIsReceiver.Contains(m.Id));
        }
        else
        {
            // فقط پیام‌هایی که کاربر فرستنده است
            query = query.Where(m => m.SenderId == currentUserId);
        }

        // فیلتر بر اساس کاربر
        if (userId.HasValue && userId.Value > 0)
        {
            if (showAsReceiver)
            {
                // در صندوق ورودی: فیلتر بر اساس فرستنده (sender)
                // پیدا کردن thread root هایی که در آن‌ها پیامی از کاربر مشخص شده وجود دارد
                var threadRootsWithUserAsSender = _unitOfWork.Context.Set<Message>()
                    .Where(m => !m.IsDeleted && m.SenderId == userId.Value)
                    .Select(m => m.ThreadRootMessageId ?? m.Id)
                    .Distinct()
                    .ToList();
                
                query = query.Where(m => threadRootsWithUserAsSender.Contains(m.Id));
            }
            else
            {
                // در صندوق ارسالی: فیلتر بر اساس گیرنده (receiver)
                // پیدا کردن thread root هایی که در آن‌ها پیامی به کاربر مشخص شده ارسال شده است
                var threadRootsWithUserAsReceiver = _unitOfWork.Context.Set<Message>()
                    .Where(m => !m.IsDeleted && m.ReceiverId == userId.Value)
                    .Select(m => m.ThreadRootMessageId ?? m.Id)
                    .Distinct()
                    .ToList();
                
                query = query.Where(m => threadRootsWithUserAsReceiver.Contains(m.Id));
            }
        }

        // فیلتر بر اساس تاریخ
        if (fromDate.HasValue)
        {
            // تنظیم زمان به ابتدای روز
            var fromDateStart = fromDate.Value.Date;
            query = query.Where(m => m.CreateDate >= fromDateStart);
        }

        if (toDate.HasValue)
        {
            // تنظیم زمان به انتهای روز
            var toDateEnd = toDate.Value.Date.AddDays(1).AddTicks(-1);
            query = query.Where(m => m.CreateDate <= toDateEnd);
        }

        // اعمال فیلتر جستجو
        if (!string.IsNullOrWhiteSpace(filter))
        {
            var filterLower = filter.ToLower();
            query = query.Where(m => 
                (m.Subject != null && m.Subject.ToLower().Contains(filterLower)) ||
                (m.Body != null && m.Body.ToLower().Contains(filterLower)) ||
                (m.Sender != null && (m.Sender.FirstName + " " + m.Sender.LastName).ToLower().Contains(filterLower)) ||
                (m.Receiver != null && (m.Receiver.FirstName + " " + m.Receiver.LastName).ToLower().Contains(filterLower))
            );
        }

        // محاسبه تعداد کل رکوردها قبل از pagination
        var totalCount = query.Count();

        // مرتب‌سازی
        if (!string.IsNullOrWhiteSpace(activeSortColumn))
        {
            if (sortDirection?.ToLower() == "desc")
            {
                switch (activeSortColumn.ToLower())
                {
                    case "subject":
                        query = query.OrderByDescending(m => m.Subject);
                        break;
                    case "createdate":
                    case "date":
                        query = query.OrderByDescending(m => m.CreateDate);
                        break;
                    case "sender":
                        query = query.OrderByDescending(m => m.Sender != null ? m.Sender.FirstName + " " + m.Sender.LastName : "");
                        break;
                    case "receiver":
                        query = query.OrderByDescending(m => m.Receiver != null ? m.Receiver.FirstName + " " + m.Receiver.LastName : "");
                        break;
                    default:
                        query = query.OrderByDescending(m => m.CreateDate);
                        break;
                }
            }
            else
            {
                switch (activeSortColumn.ToLower())
                {
                    case "subject":
                        query = query.OrderBy(m => m.Subject);
                        break;
                    case "createdate":
                    case "date":
                        query = query.OrderBy(m => m.CreateDate);
                        break;
                    case "sender":
                        query = query.OrderBy(m => m.Sender != null ? m.Sender.FirstName + " " + m.Sender.LastName : "");
                        break;
                    case "receiver":
                        query = query.OrderBy(m => m.Receiver != null ? m.Receiver.FirstName + " " + m.Receiver.LastName : "");
                        break;
                    default:
                        query = query.OrderByDescending(m => m.CreateDate);
                        break;
                }
            }
        }
        else
        {
            // مرتب‌سازی پیش‌فرض: جدیدترین اول
            query = query.OrderByDescending(m => m.CreateDate);
        }

        // اعمال pagination
        var inboxMessages = query
            .Skip(currentPage * pageSize)
            .Take(pageSize)
            .ToList();

        var result = inboxMessages.Select(m => MapToDTO(m, currentUserId, showAsReceiver)).ToList();

        return OperationResult.Succeeded(payload: result, rowCount: totalCount);
    }

    /// <summary>
    /// دریافت تعداد پیام‌های خوانده نشده
    /// </summary>
    public OperationResult GetUnreadMessageCount()
    {
        var currentUserId = _userService.GetUserId();
        
        var unreadCount = _unitOfWork.Context.Set<Message>()
            .Count(m => m.ReceiverId == currentUserId && !m.IsRead && !m.IsDeleted);

        return OperationResult.Succeeded(payload: unreadCount);
    }

    /// <summary>
    /// دریافت یک پیام کامل با thread آن
    /// </summary>
    public OperationResult GetMessageWithThread(long messageId)
    {
        var currentUserId = _userService.GetUserId();
        
        var message = _unitOfWork.Context.Set<Message>()
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Include(m => m.Attachments)
            .Include(m => m.Replies)
                .ThenInclude(r => r.Sender)
            .Include(m => m.Replies)
                .ThenInclude(r => r.Receiver)
            .Include(m => m.Replies)
                .ThenInclude(r => r.Attachments)
            .Include(m => m.ThreadRootMessage)
            .FirstOrDefault(m => m.Id == messageId && !m.IsDeleted);

        if (message == null)
        {
            return OperationResult.Failed("پیام یافت نشد");
        }

        // پیدا کردن thread root
        Message threadRoot = message;
        long threadRootId = message.ThreadRootMessageId ?? message.Id;
        
        if (message.ThreadRootMessageId.HasValue && message.ThreadRootMessage != null)
        {
            threadRoot = message.ThreadRootMessage;
            threadRootId = threadRoot.Id;
        }

        // بررسی دسترسی - کاربر باید در thread مشارکت داشته باشد (فرستنده یا گیرنده در هر پیامی از thread)
        var hasAccess = _unitOfWork.Context.Set<Message>()
            .Any(m => !m.IsDeleted && 
                     (m.ThreadRootMessageId == threadRootId || m.Id == threadRootId) &&
                     (m.SenderId == currentUserId || m.ReceiverId == currentUserId));

        if (!hasAccess)
        {
            return OperationResult.Failed("شما دسترسی به این پیام ندارید");
        }

        // بارگذاری مجدد thread root با همه reply ها
        threadRoot = _unitOfWork.Context.Set<Message>()
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Include(m => m.Attachments)
            .Include(m => m.Replies)
                .ThenInclude(r => r.Sender)
            .Include(m => m.Replies)
                .ThenInclude(r => r.Receiver)
            .Include(m => m.Replies)
                .ThenInclude(r => r.Attachments)
            .FirstOrDefault(m => m.Id == threadRootId && !m.IsDeleted);

        if (threadRoot == null)
        {
            return OperationResult.Failed("thread root یافت نشد");
        }

        // علامت‌گذاری پیام‌های خوانده‌نشده در thread (فقط پیام‌های دریافتی کاربر جاری)
        var unreadMessages = _unitOfWork.Context.Set<Message>()
            .Where(m => !m.IsDeleted && 
                       !m.IsRead &&
                       m.ReceiverId == currentUserId &&
                       m.SenderId != currentUserId &&
                       (m.ThreadRootMessageId == threadRootId || m.Id == threadRootId))
            .ToList();

        if (unreadMessages.Any())
        {
            foreach (var unreadMsg in unreadMessages)
            {
                unreadMsg.IsRead = true;
                unreadMsg.ReadDate = DateTime.Now;
            }
            _unitOfWork.Context.SaveChanges();
            
            // بارگذاری مجدد thread root برای دریافت اطلاعات به‌روز
            threadRoot = _unitOfWork.Context.Set<Message>()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.Attachments)
                .Include(m => m.Replies)
                    .ThenInclude(r => r.Sender)
                .Include(m => m.Replies)
                    .ThenInclude(r => r.Receiver)
                .Include(m => m.Replies)
                    .ThenInclude(r => r.Attachments)
                .FirstOrDefault(m => m.Id == threadRootId && !m.IsDeleted);
        }

        var dto = MapToDTOWithThread(threadRoot, currentUserId);

        return OperationResult.Succeeded(payload: dto);
    }

    /// <summary>
    /// ارسال پیام جدید
    /// </summary>
    public async Task<OperationResult> SendMessage(SendMessageDTO dto)
    {
        var currentUserId = _userService.GetUserId();

        // اگر این یک پاسخ است، thread root را پیدا کن و ReceiverId را از parent message بگیر
        long? threadRootId = null;
        long? parentMessageId = null;
        long receiverId = dto.ReceiverId; // default برای پیام جدید
        
        if (dto.ParentMessageId.HasValue)
        {
            var parentMessage = _unitOfWork.Context.Set<Message>()
                .FirstOrDefault(m => m.Id == dto.ParentMessageId.Value && !m.IsDeleted);

            if (parentMessage == null)
            {
                return OperationResult.Failed("پیام والد یافت نشد");
            }

            // بررسی دسترسی
            if (parentMessage.SenderId != currentUserId && parentMessage.ReceiverId != currentUserId)
            {
                return OperationResult.Failed("شما دسترسی به این پیام ندارید");
            }

            // برای reply، ReceiverId باید طرف مقابل باشد
            // اگر من sender بودم، reply را به receiver می‌فرستم
            // اگر من receiver بودم، reply را به sender می‌فرستم
            if (parentMessage.SenderId == currentUserId)
            {
                // من sender بودم، پس reply را به receiver می‌فرستم
                receiverId = parentMessage.ReceiverId;
            }
            else
            {
                // من receiver بودم، پس reply را به sender می‌فرستم
                receiverId = parentMessage.SenderId;
            }

            // thread root را پیدا کن (parent یا thread root آن)
            threadRootId = parentMessage.ThreadRootMessageId ?? parentMessage.Id;
            parentMessageId = dto.ParentMessageId;
        }

        // بررسی اینکه کاربر گیرنده وجود دارد (بعد از تعیین receiverId)
        var receiver = _unitOfWork.Context.Set<AspNetUsers>()
            .FirstOrDefault(u => u.Id == receiverId && u.LockoutEnabled != true );

        if (receiver == null)
        {
            return OperationResult.Failed("کاربر گیرنده یافت نشد");
        }

        // اعتبارسنجی: باید متن پیام یا پیوست وجود داشته باشد
        var hasBody = !string.IsNullOrWhiteSpace(dto.Body);
        var hasAttachments = dto.Attachments != null && dto.Attachments.Any();
        
        if (!hasBody && !hasAttachments)
        {
            return OperationResult.Failed("لطفا متن پیام یا فایل پیوست وارد کنید");
        }

        var message = new Message
        {
            SenderId = currentUserId,
            ReceiverId = receiverId,
            Subject = dto.Subject,
            Body = dto.Body ?? string.Empty, // اگر null باشد، به empty string تبدیل می‌شود
            IsRead = false, // همیشه false برای پیام جدید - گیرنده هنوز نخوانده
            ReadDate = null, // ReadDate باید null باشد تا زمانی که خوانده شود
            ParentMessageId = parentMessageId,
            ThreadRootMessageId = threadRootId,
            CreateDate = DateTime.Now,
            title = dto.Subject, // BaseEntity requires title
            IPAddress = _userService.GetIP(),
            IsDeleted = false
        };

        // افزودن پیوست‌ها
        if (dto.Attachments != null && dto.Attachments.Any())
        {
            foreach (var attDto in dto.Attachments)
            {
                try
                {
                    var contentBytes = Convert.FromBase64String(attDto.ContentBase64);
                    var attachment = new MessageAttachment
                    {
                        MessageId = 0, // بعد از save message تنظیم می‌شود
                        FileName = attDto.FileName,
                        Extension = attDto.Extension,
                        MimeType = attDto.MimeType,
                        Size = contentBytes.Length,
                        Content = contentBytes,
                        UniqueId = Guid.NewGuid(),
                        CreateDate = DateTime.Now,
                        title = attDto.FileName, // BaseEntity requires title
                        IPAddress = _userService.GetIP(),
                        IsDeleted = false
                    };
                    message.Attachments.Add(attachment);
                }
                catch
                {
                    return OperationResult.Failed($"خطا در پردازش فایل {attDto.FileName}");
                }
            }
        }

        // اطمینان حاصل کنید که IsRead = false است
        message.IsRead = false;
        message.ReadDate = null;
        
        _unitOfWork.Context.Set<Message>().Add(message);
        
        if (await _unitOfWork.Save() > 0)
        {
            var messageId = message.Id;
            
            // If this is a new thread (no parent), set thread root to itself
            if (!parentMessageId.HasValue)
            {
                var reloadedMessage = _unitOfWork.Context.Set<Message>()
                    .FirstOrDefault(m => m.Id == messageId);
                
                if (reloadedMessage != null)
                {
                    reloadedMessage.ThreadRootMessageId = reloadedMessage.Id;
                    // مطمئن شوید که IsRead همچنان false است
                    reloadedMessage.IsRead = false;
                    reloadedMessage.ReadDate = null;
                    await _unitOfWork.Save();
                }
            }
            
            // تنظیم MessageId برای attachments
            foreach (var att in message.Attachments)
            {
                att.MessageId = messageId;
            }
            if (message.Attachments.Any())
            {
                await _unitOfWork.Save();
            }

            // بارگذاری مجدد message از دیتابیس
            var savedMessage = _unitOfWork.Context.Set<Message>()
                .Include(m => m.Sender)
                .Include(m => m.Receiver)
                .Include(m => m.Attachments)
                .FirstOrDefault(m => m.Id == messageId);
            
            if (savedMessage != null)
            {
                var resultDto = MapToDTO(savedMessage, currentUserId);
                return OperationResult.Succeeded(payload: resultDto);
            }

            var resultDto2 = MapToDTO(message, currentUserId);
            return OperationResult.Succeeded(payload: resultDto2);
        }

        return OperationResult.Failed("خطا در ارسال پیام");
    }

    /// <summary>
    /// دانلود فایل پیوست
    /// </summary>
    public OperationResult DownloadAttachment(long attachmentId)
    {
        var currentUserId = _userService.GetUserId();

        var attachment = _unitOfWork.Context.Set<MessageAttachment>()
            .Include(a => a.Message)
            .FirstOrDefault(a => a.Id == attachmentId && !a.IsDeleted);

        if (attachment == null)
        {
            return OperationResult.Failed("فایل یافت نشد");
        }

        // بررسی دسترسی - کاربر باید فرستنده، گیرنده یا ادمین باشد
        var isAdmin = _userService.IsAdmin();
        var isSenderOrReceiver = attachment.Message.SenderId == currentUserId || attachment.Message.ReceiverId == currentUserId;
        if (!isAdmin && !isSenderOrReceiver)
        {
            return OperationResult.Failed("شما دسترسی به این فایل ندارید");
        }

        var dto = new MessageAttachmentDTO
        {
            Id = attachment.Id,
            MessageId = attachment.MessageId,
            FileName = attachment.FileName,
            Extension = attachment.Extension,
            MimeType = attachment.MimeType,
            Size = attachment.Size,
            ContentBase64 = Convert.ToBase64String(attachment.Content),
            UniqueId = attachment.UniqueId
        };

        return OperationResult.Succeeded(payload: dto);
    }

    /// <summary>
    /// دریافت فهرست کاربران فعال برای انتخاب گیرنده
    /// </summary>
    public OperationResult GetActiveUsers()
    {
        var users = _unitOfWork.Context.Set<AspNetUsers>()
            .Where(u => u.LockoutEnabled != true)
            .OrderBy(u => u.FirstName)
            .ThenBy(u => u.LastName)
            .Select(u => new
            {
                Id = u.Id,
                FullName = ((u.FirstName ?? "") + " " + (u.LastName ?? "")).Trim(),
                UserName = u.UserName,
                NationalNo = u.NationalNo
            })
            .ToList();

        return OperationResult.Succeeded(payload: users);
    }

    /// <summary>
    /// تبدیل Message به MessageDTO
    /// </summary>
    /// <param name="inboxAsReceiver">
    /// true = صندوق ورودی، false = صندوق ارسالی، null = وضعیت واقعی پیام (مثلاً داخل thread)
    /// </param>
    private MessageDTO MapToDTO(Message message, long currentUserId, bool? inboxAsReceiver = null)
    {
        var dto = _mapper.Map<MessageDTO>(message);
        
        // اطمینان حاصل کنید که Body به درستی map شده است
        dto.Body = message.Body ?? "";
        
        if (message.Sender != null)
        {
            dto.SenderFullName = (message.Sender.FirstName ?? "") + " " + (message.Sender.LastName ?? "");
        }
        
        if (message.Receiver != null)
        {
            dto.ReceiverFullName = (message.Receiver.FirstName ?? "") + " " + (message.Receiver.LastName ?? "");
        }

        var threadRootId = message.ThreadRootMessageId ?? message.Id;

        // پیدا کردن آخرین پیام در thread (همیشه آخرین پیام ملاک است)
        var latestMessageInThread = _unitOfWork.Context.Set<Message>()
            .Include(m => m.Sender)
            .Where(m => !m.IsDeleted &&
                       (m.ThreadRootMessageId == threadRootId ||
                        (m.ThreadRootMessageId == null && m.Id == threadRootId)))
            .OrderByDescending(m => m.CreateDate)
            .ThenByDescending(m => m.Id)
            .FirstOrDefault() ?? message;

        // تنظیم نام فرستنده آخرین پیام
        if (latestMessageInThread.Sender != null)
        {
            dto.LatestSenderFullName = (latestMessageInThread.Sender.FirstName ?? "") + " " + (latestMessageInThread.Sender.LastName ?? "");
        }
        else if (latestMessageInThread.SenderId == message.SenderId && message.Sender != null)
        {
            dto.LatestSenderFullName = dto.SenderFullName;
        }

        if (inboxAsReceiver == true)
        {
            // صندوق ورودی: خوانده‌نشده = هر پیامی که کاربر جاری گیرندهٔ آن است و هنوز نخوانده
            var unreadCountInThread = _unitOfWork.Context.Set<Message>()
                .Count(m => !m.IsDeleted &&
                           (m.ThreadRootMessageId == threadRootId ||
                            (m.ThreadRootMessageId == null && m.Id == threadRootId)) &&
                           !m.IsRead &&
                           m.ReceiverId == currentUserId);

            if (unreadCountInThread > 0)
            {
                dto.UnreadCount = 1;
                dto.IsRead = false;
                dto.ReadDate = null;
            }
            else
            {
                dto.UnreadCount = 0;
                dto.IsRead = true;
                var lastReadMessage = _unitOfWork.Context.Set<Message>()
                    .Where(m => !m.IsDeleted &&
                               (m.ThreadRootMessageId == threadRootId ||
                                (m.ThreadRootMessageId == null && m.Id == threadRootId)) &&
                               m.IsRead &&
                               m.ReceiverId == currentUserId &&
                               m.ReadDate.HasValue)
                    .OrderByDescending(m => m.ReadDate)
                    .FirstOrDefault();

                dto.ReadDate = lastReadMessage?.ReadDate;
            }
        }
        else if (inboxAsReceiver == false)
        {
            // صندوق ارسالی: خوانده‌نشده = آخرین پیام ارسالی من هنوز توسط گیرنده خوانده نشده
            if (latestMessageInThread.SenderId == currentUserId && !latestMessageInThread.IsRead)
            {
                dto.UnreadCount = 0;
                dto.IsRead = false;
                dto.ReadDate = null;
            }
            else
            {
                dto.UnreadCount = 0;
                dto.IsRead = true;
                dto.ReadDate = latestMessageInThread.SenderId == currentUserId
                    ? latestMessageInThread.ReadDate
                    : null;
            }
        }
        else
        {
            dto.UnreadCount = 0;
            dto.IsRead = message.IsRead;
            dto.ReadDate = message.ReadDate;
        }

        // mapping attachments
        if (message.Attachments != null && message.Attachments.Any())
        {
            dto.Attachments = message.Attachments
                .Where(a => !a.IsDeleted)
                .Select(a => new MessageAttachmentDTO
                {
                    Id = a.Id,
                    MessageId = a.MessageId,
                    FileName = a.FileName,
                    Extension = a.Extension,
                    MimeType = a.MimeType,
                    Size = a.Size,
                    UniqueId = a.UniqueId
                })
                .ToList();
        }

        dto.IsThreadRoot = message.ThreadRootMessageId == null || message.ThreadRootMessageId == message.Id;

        return dto;
    }

    /// <summary>
    /// تبدیل Message به MessageDTO همراه با thread کامل
    /// </summary>
    private MessageDTO MapToDTOWithThread(Message message, long currentUserId)
    {
        var dto = MapToDTO(message, currentUserId);

        // افزودن replies
        if (message.Replies != null && message.Replies.Any())
        {
            dto.Replies = message.Replies
                .Where(r => !r.IsDeleted)
                .OrderBy(r => r.CreateDate)
                .Select(r => MapToDTO(r, currentUserId))
                .ToList();
        }

        return dto;
    }

    /// <summary>
    /// دریافت تمام پیام‌ها با فیلتر بر اساس فرستنده و گیرنده
    /// </summary>
    public OperationResult GetAllMessages(long? senderId = null, long? receiverId = null)
    {
        var query = _unitOfWork.Context.Set<Message>()
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .Include(m => m.Attachments)
            .Where(m => !m.IsDeleted);

        // فیلتر بر اساس فرستنده
        if (senderId.HasValue && senderId.Value > 0)
        {
            query = query.Where(m => m.SenderId == senderId.Value);
        }

        // فیلتر بر اساس گیرنده
        if (receiverId.HasValue && receiverId.Value > 0)
        {
            query = query.Where(m => m.ReceiverId == receiverId.Value);
        }

        var messages = query
            .OrderByDescending(m => m.CreateDate)
            .ToList();

        var currentUserId = _userService.GetUserId();
        var result = messages.Select(m => MapToDTO(m, currentUserId)).ToList();

        return OperationResult.Succeeded(payload: result);
    }

    /// <summary>
    /// ارسال پیام به همه کاربران سیستم
    /// </summary>
    public async Task<OperationResult> SendMessageToAllUsers(SendMessageDTO dto)
    {
        var currentUserId = _userService.GetUserId();

        // دریافت تمام کاربران فعال
        var allUsers = _unitOfWork.Context.Set<AspNetUsers>()
            .Where(u => u.LockoutEnabled != true && u.Id != currentUserId) // کاربر جاری را شامل نشود
            .ToList();

        if (!allUsers.Any())
        {
            return OperationResult.Failed("هیچ کاربر فعالی در سیستم یافت نشد");
        }

        var successCount = 0;
        var failCount = 0;
        var errors = new List<string>();

        foreach (var user in allUsers)
        {
            try
            {
                var messageDto = new SendMessageDTO
                {
                    ReceiverId = user.Id,
                    Subject = dto.Subject,
                    Body = dto.Body,
                    Attachments = dto.Attachments
                };

                var result = await SendMessage(messageDto);
                if (result.Success)
                {
                    successCount++;
                }
                else
                {
                    failCount++;
                    var fullName = (user.FirstName ?? "") + " " + (user.LastName ?? "");
                    errors.Add($"خطا در ارسال به {fullName.Trim()}: {result.Message}");
                }
            }
            catch (Exception ex)
            {
                failCount++;
                var fullName = (user.FirstName ?? "") + " " + (user.LastName ?? "");
                errors.Add($"خطا در ارسال به {fullName.Trim()}: {ex.Message}");
            }
        }

        var message = $"پیام به {successCount} کاربر ارسال شد";
        if (failCount > 0)
        {
            message += $" و {failCount} کاربر با خطا مواجه شد";
        }

        return OperationResult.Succeeded(
            msg: message,
            payload: new { SuccessCount = successCount, FailCount = failCount, Errors = errors }
        );
    }

    public bool Validate(Message entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
