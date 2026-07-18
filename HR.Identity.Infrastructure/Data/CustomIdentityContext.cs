using HR.Identity.Core.Entities;
using HR.SharedKernel.Data;

using Microsoft.EntityFrameworkCore;

using HR.Identity.infrastructure.Data;
using HR.SharedKernel.Service;

namespace HR.Identity.infrastructure.Data;

public class CustomIdentityContext :  BaseDbContext
{
    public CustomIdentityContext()
    {

    }
    public CustomIdentityContext(DbContextOptions<CustomIdentityContext> options, UserResolverService userService) : base(options, userService)
    {

    }

    public DbSet<UserDefaultSetting> UserDefaultSettings { get; set; }
    //public DbSet<RoleAccess> RoleAccesses { get; set; }
    //public DbSet<SystemTree> SystemTrees { get; set; }
    //public DbSet<UserAccess> UserAccesses { get; set; }
    public DbSet<UserCostCenter> UserCostCenters { get; set; }
    public DbSet<UserOrganizationUnit> UserOrganizationUnits { get; set; }
    public DbSet<UserPayLocation> UserPayLocations { get; set; }
    public DbSet<UserWorkPlace> UserWorkPlaces { get; set; }
    public DbSet<UserLoginHistory> UserLoginHistories { get; set; }
    public DbSet<UserPasswordHistory> UserPasswordHistories { get; set; }
    public DbSet<SecurityAuditLog> SecurityAuditLogs { get; set; }
    public DbSet<LoginCredentialLog> LoginCredentialLogs { get; set; }
    public DbSet<PasswordChangeRateLimit> PasswordChangeRateLimits { get; set; }
 

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
              .SelectMany(t => t.GetForeignKeys())
              .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.Entity<AspNetUsers>()
          .HasIndex(u => u.UserName)
          .IsUnique();


        modelBuilder.Entity<AspNetUsers>()
          .HasIndex(u => u.NationalNo)
          .IsUnique();
    }
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        base.OnConfiguring(optionsBuilder);
    }
}



