using HR.Attendance.Core.Entities;
using HR.Attendance.Core.Entities.EmployeeSpecific;
using EmployeeShiftAssignment = HR.Attendance.Core.Entities.EmployeeSpecific.EmployeeShiftAssignment;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

namespace HR.Attendance.Infrastructure.Data;

public class AttendanceContext : BaseDbContext
{
    public AttendanceContext()
    {
    }

    public AttendanceContext(DbContextOptions<AttendanceContext> options, UserResolverService userService)
        : base(options, userService)
    {
    }

    public DbSet<AttendanceLocation> AttendanceLocations { get; set; }
    public DbSet<AttendanceDevice> AttendanceDevices { get; set; }
    public DbSet<EmployeeAttendanceLog> AttendanceLogs { get; set; }
    public DbSet<AttendanceCalendar> AttendanceCalendars { get; set; }
    public DbSet<Shift> Shifts { get; set; }
    public DbSet<ShiftDetail> ShiftDetails { get; set; }
    public DbSet<EmployeeShiftAssignment> EmployeeShiftAssignments { get; set; }
    public DbSet<EmployeeAttendanceDailyResult> EmployeeAttendanceDailyResults { get; set; }
    public DbSet<EmployeeAttendanceException> EmployeeAttendanceExceptions { get; set; }
    public DbSet<EmployeeExceptionJustificationRequest> EmployeeExceptionJustificationRequests { get; set; }
    public DbSet<AbsenceType> AbsenceTypes { get; set; }
    public DbSet<EmployeeExceptionJustificationRequestState> EmployeeExceptionJustificationRequestStates { get; set; }
    public DbSet<ShiftOverride> ShiftOverrides { get; set; }
    public DbSet<ShiftOverrideDetail> ShiftOverrideDetails { get; set; }
    public DbSet<EmployeeMonthlySummary> EmployeeMonthlySummaries { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetForeignKeys())
            .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.Entity<HR.BaseInfo.Core.Entities.AttendanceHoliday>()
            .ToTable("AttendanceHoliday", "bas", t => t.ExcludeFromMigrations());

        modelBuilder.Entity<HR.BaseInfo.Core.Entities.LeaveType>()
            .ToTable("LeaveType", "bas", t => t.ExcludeFromMigrations());

        modelBuilder.Entity<AttendanceCalendar>()
            .HasOne(c => c.Holiday)
            .WithMany()
            .HasForeignKey(c => c.HolidayId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<AttendanceCalendar>()
            .HasIndex(c => c.Date)
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<EmployeeShiftAssignment>()
            .HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeShiftAssignment>()
            .HasOne(a => a.Shift)
            .WithMany()
            .HasForeignKey(a => a.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Shift>()
            .HasMany(s => s.Details)
            .WithOne(d => d.Shift)
            .HasForeignKey(d => d.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShiftDetail>()
            .HasIndex(d => new { d.ShiftId, d.WeekDay })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<ShiftOverride>()
            .HasOne(o => o.Shift)
            .WithMany()
            .HasForeignKey(o => o.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShiftOverride>()
            .HasMany(o => o.Details)
            .WithOne(d => d.ShiftOverride)
            .HasForeignKey(d => d.ShiftOverrideId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ShiftOverrideDetail>()
            .HasIndex(d => new { d.ShiftOverrideId, d.WeekDay })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<ShiftOverride>()
            .HasIndex(o => new { o.OrganisationChartId, o.ShiftId, o.StartDate, o.EndDate })
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<AttendanceLocation>()
            .HasOne(a => a.RelatedOrganisationChart)
            .WithMany()
            .HasForeignKey(a => a.RelatedOrganisationChartId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeAttendanceDailyResult>()
            .HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeAttendanceDailyResult>()
            .HasOne(a => a.Shift)
            .WithMany()
            .HasForeignKey(a => a.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeAttendanceDailyResult>()
            .HasOne(a => a.AttendanceCalendar)
            .WithMany()
            .HasForeignKey(a => a.AttendanceCalendarId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeAttendanceDailyResult>()
            .HasIndex(a => new { a.OrganisationChartId, a.EmployeeId, a.AttendanceCalendarId })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<EmployeeAttendanceException>()
            .HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeAttendanceException>()
            .HasOne(a => a.Shift)
            .WithMany()
            .HasForeignKey(a => a.ShiftId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeAttendanceException>()
            .HasOne(a => a.AttendanceCalendar)
            .WithMany()
            .HasForeignKey(a => a.AttendanceCalendarId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeAttendanceException>()
            .HasOne(a => a.AbsenceType)
            .WithMany()
            .HasForeignKey(a => a.AbsenceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeAttendanceException>()
            .HasIndex(a => new { a.OrganisationChartId, a.EmployeeId, a.AttendanceCalendarId })
            .HasFilter("[IsDeleted] = 0");

        modelBuilder.Entity<EmployeeExceptionJustificationRequest>()
            .HasOne(a => a.EmployeeAttendanceException)
            .WithMany()
            .HasForeignKey(a => a.EmployeeAttendanceExceptionId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeExceptionJustificationRequest>()
            .HasOne(a => a.AbsenceType)
            .WithMany()
            .HasForeignKey(a => a.AbsenceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeExceptionJustificationRequest>()
            .HasOne(a => a.EmployeeExceptionJustificationRequestState)
            .WithMany()
            .HasForeignKey(a => a.EmployeeExceptionJustificationRequestStateId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeExceptionJustificationRequest>()
            .HasOne(a => a.LeaveType)
            .WithMany()
            .HasForeignKey(a => a.LeaveTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeMonthlySummary>()
            .HasOne(a => a.Employee)
            .WithMany()
            .HasForeignKey(a => a.EmployeeId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeMonthlySummary>()
            .HasOne(a => a.CostCenter)
            .WithMany()
            .HasForeignKey(a => a.CostCenterId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeMonthlySummary>()
            .HasOne(a => a.OrganizationUnit)
            .WithMany()
            .HasForeignKey(a => a.OrganizationUnitId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeMonthlySummary>()
            .HasOne(a => a.WorkPlace)
            .WithMany()
            .HasForeignKey(a => a.WorkPlaceId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<EmployeeMonthlySummary>()
            .HasIndex(a => new { a.OrganisationChartId, a.EmployeeId, a.Year, a.Month })
            .IsUnique()
            .HasFilter("[IsDeleted] = 0");
    }
}
