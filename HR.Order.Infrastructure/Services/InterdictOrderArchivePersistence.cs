using HR.Order.Core.Data;
using HR.Order.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.Order.Infrastructure.Services;

internal static class InterdictOrderArchivePersistence
{
    public static bool Exists(OrderContext context, long interdictOrderId)
    {
        return context.InterdictOrderArchives.AsNoTracking().Any(a => a.InterdictOrderId == interdictOrderId);
    }

    public static InterdictOrderArchive Replace(
        OrderContext context,
        long interdictOrderId,
        byte[]? rawPdfBytes,
        byte[]? formattedPdfBytes,
        string? ipAddress = null)
    {
        var existingArchives = context.InterdictOrderArchives
            .Where(a => a.InterdictOrderId == interdictOrderId)
            .ToList();

        if (existingArchives.Count > 0)
        {
            context.InterdictOrderArchives.RemoveRange(existingArchives);
            context.SaveChanges();
        }

        var archive = new InterdictOrderArchive
        {
            InterdictOrderId = interdictOrderId,
            PdfrawByteArray = rawPdfBytes,
            PdfbyteArray = formattedPdfBytes,
            CreateDate = DateTime.Now,
            IPAddress = ipAddress ?? string.Empty,
            IsDeleted = false,
        };

        context.InterdictOrderArchives.Add(archive);
        context.SaveChanges();
        return archive;
    }
}
