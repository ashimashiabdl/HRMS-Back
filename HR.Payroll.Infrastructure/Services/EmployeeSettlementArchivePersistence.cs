using HR.Payroll.Core.Data.EmployeeRelated;
using HR.Payroll.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.Payroll.Infrastructure.Services;

internal static class EmployeeSettlementArchivePersistence
{
    public static bool Exists(PayrollContext context, long employeeSettlementId)
    {
        return context.EmployeeSettlementArchives.AsNoTracking().Any(a => a.EmployeeSettlementId == employeeSettlementId);
    }

    public static EmployeeSettlementArchive Replace(
        PayrollContext context,
        long employeeSettlementId,
        byte[]? rawPdfBytes,
        byte[]? formattedPdfBytes,
        string? ipAddress = null)
    {
        var existingArchives = context.EmployeeSettlementArchives
            .Where(a => a.EmployeeSettlementId == employeeSettlementId)
            .ToList();

        if (existingArchives.Count > 0)
        {
            context.EmployeeSettlementArchives.RemoveRange(existingArchives);
            context.SaveChanges();
        }

        var archive = new EmployeeSettlementArchive
        {
            EmployeeSettlementId = employeeSettlementId,
            PdfrawByteArray = rawPdfBytes,
            PdfbyteArray = formattedPdfBytes,
            CreateDate = DateTime.Now,
            IPAddress = ipAddress ?? string.Empty,
            IsDeleted = false,
        };

        context.EmployeeSettlementArchives.Add(archive);
        context.SaveChanges();
        return archive;
    }
}
