using HR.Report.Core.Entity;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.Data;

public class ReportContext : BaseDbContext
{
    public ReportContext()
    {

    }
    public ReportContext(DbContextOptions<ReportContext> options, UserResolverService userService) : base(options, userService)
    {

    }

    public DbSet<DynamicReport> DynamicReports { get; set; }
    public DbSet<DynamicReportParameter> DynamicReportParameter { get; set; }
    public DbSet<FieldDataType> FieldDataTypes { get; set; }
    public DbSet<FieldOperator> FieldOperators { get; set; }
    public DbSet<ReportableEntity> ReportableEntities { get; set; }
    public DbSet<ReportableField> ReportableFields { get; set; }
    public DbSet<EmployeeProperty> EmployeeProperties { get; set; }
    public DbSet<PayLocationProgressReport> PayLocationProgressReports { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
              .SelectMany(t => t.GetForeignKeys())
              .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        // == Report Builder Fluent API Configurations ==

        modelBuilder.Entity<ReportableEntity>(entity =>
        {
            entity.HasMany(e => e.Fields)
                  .WithOne(f => f.ReportableEntity)
                  .HasForeignKey(f => f.ReportableEntityId);
        });

        modelBuilder.Entity<FieldDataType>(entity =>
        {
            entity.HasMany(e => e.ReportableFields)
                  .WithOne(f => f.FieldDataType)
                  .HasForeignKey(f => f.FieldDataTypeId);

            entity.HasMany(e => e.FieldOperators)
                  .WithOne(o => o.FieldDataType)
                  .HasForeignKey(o => o.FieldDataTypeId);
        });

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        base.OnConfiguring(optionsBuilder);
    }
}
