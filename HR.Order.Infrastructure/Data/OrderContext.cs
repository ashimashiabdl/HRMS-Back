using HR.Order.Core.Data;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Order.Infrastructure.Data;

public class OrderContext : BaseDbContext
{
    public OrderContext()
    {

    }
    public OrderContext(DbContextOptions<OrderContext> options, UserResolverService userService) : base(options, userService)
    {
    }

    public DbSet<RecruitOrder> RecruitOrders { get; set; }

    public DbSet<InterdictOrder> InterdictOrders { get; set; }
    public DbSet<InterdictOrderWageItem> InterdictOrderWageItems { get; set; }
    public DbSet<InterdictOrderCoefficientItem> InterdictOrderCoefficientItems { get; set; }
    public DbSet<InterdictOrderArchive> InterdictOrderArchives { get; set; }
    public DbSet<InterdictOrderCopy> InterdictOrderCopies { get; set; }
    public DbSet<InterdictOrderPromissory> InterdictOrderPromissories { get; set; }
    public DbSet<BatchRequest> BatchRequests { get; set; }
    public DbSet<BatchRequestDetail> BatchRequestDetails { get; set; }
    public DbSet<BatchRequestDetailReference> BatchRequestDetailReferences { get; set; }
    public DbSet<BatchRequestFile> BatchRequestFiles { get; set; }
    public DbSet<OrderTempFile> OrderTempFiles { get; set; }
    public DbSet<HR.WorkFlow.Core.Data.Definition> Definitions { get; set; }
    public DbSet<HR.WorkFlow.Core.Data.NodeUserRel> NodeUserRels { get; set; }
    public DbSet<HR.WorkFlow.Core.Data.NodeRoleRel> NodeRoleRels { get; set; }
    public DbSet<HR.WorkFlow.Core.Data.WorkFlowInstance> WorkFlowInstances { get; set; }
    public DbSet<HR.WorkFlow.Core.Data.ActivityTemplate> ActivityTemplates { get; set; }
    public DbSet<HR.WorkFlow.Core.Data.Node> Nodes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
              .SelectMany(t => t.GetForeignKeys())
              .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        ConfigureInterdictOrderDefaults(modelBuilder);
        ConfigureInterdictOrderArchive(modelBuilder);

        modelBuilder.Entity<BatchRequestFile>()
.Property(u => u.Extension)
.HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))");



        modelBuilder.Entity<OrderTempFile>()
.Property(u => u.Extension)
.HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))");

        // Configure BaseTableValue relationships without FK constraints
        ConfigureBaseTableValueRelationships(modelBuilder);

    }

    private static void ConfigureInterdictOrderDefaults(ModelBuilder modelBuilder)
    {
        const string bitFalse = "(CONVERT([bit],(0)))";
        const string intZero = "(0)";
        const string emptyString = "(N'')";

        var entity = modelBuilder.Entity<InterdictOrder>();

        entity.Property(e => e.Code).HasDefaultValueSql(emptyString);
        entity.Property(e => e.CreatorUserName).HasDefaultValueSql(emptyString);
        entity.Property(e => e.Description).HasDefaultValueSql(emptyString);
        entity.Property(e => e.DrivingLicenseNumber).HasDefaultValueSql(emptyString);
        entity.Property(e => e.ExperienceRecorded).HasDefaultValueSql(emptyString);
        entity.Property(e => e.RetiredRecorded).HasDefaultValueSql(emptyString);
        entity.Property(e => e.YearRecorded).HasDefaultValueSql(emptyString);
        entity.Property(e => e.ApproverSignatureGuid).HasDefaultValueSql(emptyString);
        entity.Property(e => e.AccountNumber).HasDefaultValueSql(emptyString);
        entity.Property(e => e.OtherVeterans).HasDefaultValueSql(emptyString);
        entity.Property(e => e.FirstName).HasDefaultValueSql(emptyString);
        entity.Property(e => e.LastName).HasDefaultValueSql(emptyString);
        entity.Property(e => e.FatherName).HasDefaultValueSql(emptyString);
        entity.Property(e => e.PersonelCode).HasDefaultValueSql(emptyString);
        entity.Property(e => e.IdentityNo).HasDefaultValueSql(emptyString);
        entity.Property(e => e.NationalNo).HasDefaultValueSql(emptyString);
        entity.Property(e => e.OrderReason).HasDefaultValueSql(emptyString);
        entity.Property(e => e.PayRollApproveUser).HasDefaultValueSql(emptyString);
        entity.Property(e => e.IPAddress).HasDefaultValueSql(emptyString);
        entity.Property(e => e.CreatedBy).HasDefaultValueSql(emptyString);
        entity.Property(e => e.LastModifiedBy).HasDefaultValueSql(emptyString);

        // ValueGeneratedNever: always send non-nullable values on INSERT even when they match CLR defaults.
        // Without this, EF omits the column and INSERT fails if the DB lacks a matching default constraint.
        entity.Property(e => e.PayRollAprove).HasDefaultValueSql(bitFalse).ValueGeneratedNever();
        entity.Property(e => e.IsWomenHead).HasDefaultValueSql(bitFalse).ValueGeneratedNever();
        entity.Property(e => e.IsArrears).HasDefaultValueSql(bitFalse).ValueGeneratedNever();
        entity.Property(e => e.IsDeleted).HasDefaultValueSql(bitFalse).ValueGeneratedNever();
        entity.Property(e => e.IsWar).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsCaptivity).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsBasij).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsIsar).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsMartyrs).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.RetiredFlagOk).HasDefaultValueSql(bitFalse);

        entity.Property(e => e.ChildCount).HasDefaultValueSql(intZero).ValueGeneratedNever();
        entity.Property(e => e.ItemCount).HasDefaultValueSql(intZero).ValueGeneratedNever();
        entity.Property(e => e.HistoryOut).HasDefaultValueSql(intZero);
        entity.Property(e => e.HistoryStop).HasDefaultValueSql(intZero);
        entity.Property(e => e.WarDuration).HasDefaultValueSql(intZero);
        entity.Property(e => e.CaptivityDuration).HasDefaultValueSql(intZero);
        entity.Property(e => e.BasijDuration).HasDefaultValueSql(intZero);
        entity.Property(e => e.JobDegree).HasDefaultValueSql(intZero);
        entity.Property(e => e.WifeCount).HasDefaultValueSql(intZero);
        entity.Property(e => e.GradScore).HasDefaultValueSql(intZero);
        entity.Property(e => e.DrivingLicenseTypeId).HasDefaultValueSql(intZero);
        entity.Property(e => e.Serial).HasDefaultValueSql(intZero);
        entity.Property(e => e.SponsorshipCount).HasDefaultValueSql(intZero);
        entity.Property(e => e.YearCoefficient).HasDefaultValueSql(intZero);

        entity.Property(e => e.SumWageFactors).HasColumnType("decimal(18,2)").HasDefaultValueSql("((0))");
        entity.Property(e => e.IsarPercent).HasDefaultValueSql("((0))");
    }

    private static void ConfigureInterdictOrderArchive(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InterdictOrderArchive>()
            .HasIndex(a => a.InterdictOrderId)
            .IsUnique();
    }

    private void ConfigureBaseTableValueRelationships(ModelBuilder modelBuilder)
    {
        // InterdictOrder entity relationships
        modelBuilder.Entity<InterdictOrder>()
            .Property<long>("IssueTypeId");
        modelBuilder.Entity<InterdictOrder>()
            .HasOne(i => i.IssueType)
            .WithMany()
            .HasForeignKey("IssueTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<InterdictOrder>()
            .Property<long?>("MarriageStatusId");
        modelBuilder.Entity<InterdictOrder>()
            .HasOne(i => i.MarriageStatus)
            .WithMany()
            .HasForeignKey("MarriageStatusId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<InterdictOrder>()
            .Property<long>("InsuranceTypeId");
        modelBuilder.Entity<InterdictOrder>()
            .HasOne(i => i.InsuranceType)
            .WithMany()
            .HasForeignKey("InsuranceTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // BatchRequest entity relationships
        modelBuilder.Entity<BatchRequest>()
            .Property<long>("RequestStateId");
        modelBuilder.Entity<BatchRequest>()
            .HasOne(b => b.RequestState)
            .WithMany()
            .HasForeignKey("RequestStateId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<BatchRequest>()
            .Property<long?>("ArchiveStateId");
        modelBuilder.Entity<BatchRequest>()
            .HasOne(b => b.ArchiveState)
            .WithMany()
            .HasForeignKey("ArchiveStateId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<BatchRequest>()
            .Property<long>("RequestTypeId");
        modelBuilder.Entity<BatchRequest>()
            .HasOne(b => b.RequestType)
            .WithMany()
            .HasForeignKey("RequestTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // BatchRequestFile entity relationships
        modelBuilder.Entity<BatchRequestFile>()
            .Property<long>("FileTypeId");
        modelBuilder.Entity<BatchRequestFile>()
            .HasOne(b => b.FileType)
            .WithMany()
            .HasForeignKey("FileTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // Add other BaseTableValue relationships for Order entities...
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
     
        
        base.OnConfiguring(optionsBuilder);
    }

}
