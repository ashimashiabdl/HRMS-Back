using HR.Identity.Core.Entities;
using HR.SharedKernel.Data;
using HR.SharedKernel.Share;
using HR.SharedKernel.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.Identity.infrastructure.Data;
using System.Linq.Expressions;


namespace HR.Identity.infrastructure.Data;

public class IdentityContext : IdentityDbContext<AspNetUsers, AspNetRoles, long, UserClaim, UserRole, UserLogin, RoleClaim, UserToken>, IAppDbContext 
{
    public UserResolverService _userResolverService;
    public DbSet<Audit> AuditLogs { get; set; }
    protected string _connectionString;

    public IdentityContext()
    {

    }
    public IdentityContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {

    }
    public IdentityContext(DbContextOptions<IdentityContext> options, UserResolverService userService) : base(options)
    {
        try
        {
            _connectionString = ((SqlServerOptionsExtension)options.Extensions.Last()).ConnectionString;
            _userResolverService = userService;
        }
        catch (Exception)
        {
            // Handle exception silently
        }
    }
    public DbSet<AspNetUsers> AspNetUsers { get; set; }
    public DbSet<AspNetRoles> AspNetRoles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<RoleClaim> RoleClaims { get; set; }
    public DbSet<UserLogin> UserLogins { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<UserClaim> UserClaims { get; set; }
    public DbSet<UserToken> UserToken { get; set; }
    public DbSet<PermissionRoute> PermissionRoutes { get; set; }
    public DbSet<RoleMenu> RoleMenus { get; set; }
    public DbSet<UserMenu> UserMenus { get; set; }
    public DbSet<UserReport> UserReports { get; set; }
    public DbSet<RoleReport> RoleReports { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }
    public DbSet<RoleReportableEntity> RoleReportableEntities { get; set; }
    public DbSet<UserReportableEntity> UserReportableEntities { get; set; }
    public DbSet<Message> Messages { get; set; }
    public DbSet<MessageAttachment> MessageAttachments { get; set; }
    public DbSet<CommonPassword> CommonPasswords { get; set; }
    public DbSet<BlockedIp> BlockedIps { get; set; }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ChangeTracker.DetectChanges();
        
        // قبل از save، Id های پیام‌های جدید را ذخیره کن (اگر Id = 0 باشد، بعد از save باید UPDATE کنم)
        var newMessagesBeforeSave = ChangeTracker.Entries<Message>()
            .Where(e => e.State == EntityState.Added)
            .Select(e => e.Entity)
            .ToList();
        
        OnBeforeSaveChanges();
                var result = await base.SaveChangesAsync();
        
        // بعد از save، اطمینان حاصل کنید که هر Message جدیدی که اضافه شده، IsRead = false باشد
        // اما فقط برای پیام‌هایی که هنوز IsRead = false هستند (برای جلوگیری از تداخل با MarkAsRead)
        foreach (var message in newMessagesBeforeSave)
        {
            if (message.Id > 0 && !message.IsRead)
            {
                // UPDATE مستقیم SQL برای اطمینان از IsRead = false (فقط اگر false باشد)
                await Database.ExecuteSqlRawAsync(
                    "UPDATE [Identity].[Message] SET [IsRead] = 0, [ReadDate] = NULL WHERE [Id] = {0} AND [IsRead] = 0", 
                    message.Id);
            }
        }
        
        return result;
    }

    private void OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        
        // اطمینان حاصل کنید که هر Message جدیدی که اضافه می‌شود، IsRead = false باشد
        foreach (var entry in ChangeTracker.Entries<Message>())
        {
            if (entry.State == EntityState.Added)
            {
                // برای پیام جدید، همیشه IsRead = false
                entry.Entity.IsRead = false;
                entry.Entity.ReadDate = null;
            }
        }
        
        var auditEntries = new List<AuditEntry>();

        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is Audit || entry.State == EntityState.Detached || entry.State == EntityState.Unchanged)
                continue;
            
            var auditEntry = new AuditEntry(entry);
            auditEntry.TableName = entry.Entity.GetType().Name + " - " + entry.Entity.GetType().FullName;

            if (_userResolverService != null)
            {
                try
                {
                    var userId = _userResolverService.GetUserId();
                    auditEntry.UserId = userId > 0 ? userId.ToString() : "Anonymous";
                }
                catch (Exception)
                {
                    auditEntry.UserId = "System";
                }
            }
            else
            {
                auditEntry.UserId = "System";
            }

            auditEntries.Add(auditEntry);
            PropertyValues dbValuesModified = null;
            foreach (var property in entry.Properties)
            {
                string propertyName = property.Metadata.Name;

                if (property.Metadata.IsPrimaryKey())
                {
                    auditEntry.KeyValues[propertyName] = property.CurrentValue;
                    continue;
                }
                switch (entry.State)
                {
                    case EntityState.Added:
                        auditEntry.AuditType = Enums.AuditType.Create;
                        auditEntry.NewValues[propertyName] = property.CurrentValue;
                        if (auditEntry.TableName.Contains("AspNetUsers"))
                        {
                            auditEntry.UserId = "Initial Register";
                        }
                        break;
                    case EntityState.Deleted:
                        auditEntry.AuditType = Enums.AuditType.Delete;
                        auditEntry.OldValues[propertyName] = property.OriginalValue;
                        break;
                    case EntityState.Modified:
                        if (property.IsModified)
                        {
                            dbValuesModified ??= entry.GetDatabaseValues();
                            if (dbValuesModified == null)
                                continue; // Entity was deleted externally; avoid NRE and let SaveChanges raise concurrency
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.AuditType = Enums.AuditType.Update;
                            var old = dbValuesModified.GetValue<object>(propertyName);
                            auditEntry.OldValues[propertyName] = old;
                            auditEntry.NewValues[propertyName] = property.CurrentValue;
                        }
                        break;
                }
            }
        }
        foreach (var auditEntry in auditEntries)
        {
            string ipAddress = "Unknown";
            if (_userResolverService != null)
            {
                try
                {
                    ipAddress = _userResolverService.GetIP();
                    if (string.IsNullOrEmpty(ipAddress) || ipAddress == "Notfound")
                    {
                        ipAddress = "Local";
                    }
                }
                catch (Exception)
                {
                    ipAddress = "Local";
                }
            }
            
            AuditLogs.Add(auditEntry.ToAudit(ipAddress));
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Ensure all Identity entities use the "Identity" schema
        // This must be done after base.OnModelCreating to override default schema
        modelBuilder.Entity<AspNetUsers>()
            .ToTable("AspNetUsers", "Identity");

        modelBuilder.Entity<AspNetRoles>()
            .ToTable("AspNetRoles", "Identity");

        modelBuilder.Entity<UserRole>()
            .ToTable("User_Role", "Identity");

        modelBuilder.Entity<UserLogin>()
            .ToTable("User_Login", "Identity");

        modelBuilder.Entity<UserClaim>()
            .ToTable("User_Claim", "Identity");

        // Configure UserClaim.User navigation property to use existing UserId foreign key
        modelBuilder.Entity<UserClaim>()
            .HasOne(uc => uc.User)
            .WithMany()
            .HasForeignKey(uc => uc.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<RoleClaim>()
            .ToTable("Role_Claim", "Identity");

        // Configure RoleClaim.Role navigation property to use existing RoleId foreign key
        modelBuilder.Entity<RoleClaim>()
            .HasOne(rc => rc.Role)
            .WithMany()
            .HasForeignKey(rc => rc.RoleId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserToken>()
            .ToTable("User_Token", "Identity");

        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
              .SelectMany(t => t.GetForeignKeys())
              .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.Entity<AspNetUsers>()
          .HasIndex(u => u.UserName)
          .IsUnique();

        modelBuilder.Entity<UserRole>()
    .HasKey(ur => new { ur.UserId, ur.RoleId });


        modelBuilder.Entity<UserLogin>()
 .HasKey(l => new { l.LoginProvider, l.ProviderKey });
        ;


        modelBuilder.Entity<AspNetUsers>()
          .HasIndex(u => u.NationalNo)
          .IsUnique();

        // Configure RefreshToken entity
        modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => rt.Token);

        modelBuilder.Entity<RefreshToken>()
            .HasIndex(rt => rt.UserId);

        modelBuilder.Entity<RefreshToken>()
            .HasOne(rt => rt.User)
            .WithMany()
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PermissionRoute>()
            .HasIndex(p => p.Claim)
            .IsUnique();

        // Configure Message self-referencing relationships
        modelBuilder.Entity<Message>(entity =>
        {
            // Parent message relationship
            entity.HasOne(m => m.ParentMessage)
                .WithMany(m => m.Replies)
                .HasForeignKey(m => m.ParentMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Thread root message relationship
            entity.HasOne(m => m.ThreadRootMessage)
                .WithMany()
                .HasForeignKey(m => m.ThreadRootMessageId)
                .OnDelete(DeleteBehavior.Restrict);

            // Sender relationship
            entity.HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Receiver relationship
            entity.HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Configure MessageAttachment relationship
        modelBuilder.Entity<MessageAttachment>(entity =>
        {
            entity.HasOne(ma => ma.Message)
                .WithMany(m => m.Attachments)
                .HasForeignKey(ma => ma.MessageId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Handle IdentityPasskeyData entity if it exists (from ASP.NET Core Identity passkey support)
        // This entity may be discovered by EF Core but might not have a primary key configured
        try
        {
            var passkeyEntityTypes = modelBuilder.Model.GetEntityTypes()
                .Where(e => e.Name != null && (e.Name.Contains("IdentityPasskeyData") || 
                                               e.ClrType.Name == "IdentityPasskeyData" ||
                                               e.Name.EndsWith(".IdentityPasskeyData")))
                .ToList();

            foreach (var entityType in passkeyEntityTypes)
            {
                if (entityType.FindPrimaryKey() == null)
                {
                    // Configure as keyless entity since no primary key is defined
                    modelBuilder.Entity(entityType.ClrType).HasNoKey();
                }
            }
        }
        catch (Exception ex)
        {
            // If configuration fails, the entity might not exist or might be configured differently
            // This is not a critical error if passkeys are not being used
            // Log the exception for debugging if needed
            System.Diagnostics.Debug.WriteLine($"Warning: Could not configure IdentityPasskeyData: {ex.Message}");
        }
    }
    private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {


#if DEBUG
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               
                   .SetBasePath(Directory.GetCurrentDirectory()) 
               .AddJsonFile("appsettings.Development.json")
               .Build();
            var raw = configuration.GetConnectionString("HRMSConnection");
            var connectionString = HR.SharedKernel.Security.ConnectionStringProtector.TryUnprotect(raw) ?? raw;
            optionsBuilder.UseSqlServer(connectionString);
        }
#endif
#if !DEBUG
        if (!optionsBuilder.IsConfigured)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
               .SetBasePath(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location))
               .AddJsonFile("appsettings.json")
               .Build();
            var raw = configuration.GetConnectionString("HRMSConnection");
            var connectionString = HR.SharedKernel.Security.ConnectionStringProtector.TryUnprotect(raw) ?? raw;
            optionsBuilder.UseSqlServer(connectionString);
        }
#endif


        base.OnConfiguring(optionsBuilder);
    }

    void IAppDbContext.Add<TEntity>(TEntity objModel)
    {
        throw new NotImplementedException();
    }

    void IAppDbContext.Remove<TEntity>(TEntity objModel)
    {
        throw new NotImplementedException();
    }

    public void AddRange<TEntity>(IQueryable<TEntity> objModel) where TEntity : class
    {
        throw new NotImplementedException();
    }

    public IQueryable<TEntity> All<TEntity>(bool IgnoreExpired = true, DateTime? ImpleDate = null) where TEntity : class
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> GetIdAsync<TEntity>(long id) where TEntity : class
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate, bool IgnoreExpired = true) where TEntity : class
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> GetAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, bool IgnoreExpired = true) where TEntity : class
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetList<TEntity>(Expression<Func<TEntity, bool>> predicate, bool IgnoreExpired = true) where TEntity : class
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetListAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, bool IgnoreExpired = true) where TEntity : class
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetAllAsync<TEntity>(bool IgnoreExpired = true, string? SqlRaw = null) where TEntity : class
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetAll<TEntity>(bool IgnoreExpired = true) where TEntity : class
    {
        throw new NotImplementedException();
    }

    public int Count<TEntity>() where TEntity : class
    {
        throw new NotImplementedException();
    }

    public Task<int> CountAsync<TEntity>() where TEntity : class
    {
        throw new NotImplementedException();
    }

    void IAppDbContext.Update<TEntity>(TEntity objModel)
    {
        throw new NotImplementedException();
    }

    public void LogicalRemove<TEntity>(long Id) where TEntity : class
    {
        throw new NotImplementedException();
    }
}



