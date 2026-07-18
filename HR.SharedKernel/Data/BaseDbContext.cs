using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using System.Linq.Expressions;


namespace HR.SharedKernel.Data;

public class BaseDbContext : Microsoft.EntityFrameworkCore.DbContext , IAppDbContext
{
    public UserResolverService _userResolverService;

    public DbSet<Audit> AuditLogs { get; set; }

    //private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;
    protected string _connectionString;

    public override int SaveChanges()
    {
        OnBeforeSaveChanges();
        return base.SaveChanges();
    }
    
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        OnBeforeSaveChanges();
        return await base.SaveChangesAsync();
    }
    private void OnBeforeSaveChanges()
    {
        ChangeTracker.DetectChanges();
        
        // Set CreatedBy and LastModifiedBy for BaseEntity entities
        SetAuditFields();
        
        var auditEntries = new List<AuditEntry>();


        foreach (var entry in ChangeTracker.Entries())
        {


            if (entry.Entity is Audit
                || entry.Entity.GetType().FullName == "HR.Identity.Core.Entities.SecurityAuditLog"
                || entry.State == EntityState.Detached
                || entry.State == EntityState.Unchanged)
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
                        if (auditEntry.TableName == "AspNetUsers")
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
                            auditEntry.ChangedColumns.Add(propertyName);
                            auditEntry.AuditType = Enums.AuditType.Update;
                            var old = entry.GetDatabaseValues().GetValue<object>(propertyName);
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
    
    private void SetAuditFields()
    {
        var currentUserInfo = GetCurrentUserInfo();
        
        foreach (var entry in ChangeTracker.Entries())
        {
            if (entry.Entity is BaseEntity baseEntity)
            {
                if (entry.State == EntityState.Added)
                {
                    // Set CreatedBy for new entities (combined string: FullName | Username | UserId)
                    if (string.IsNullOrEmpty(baseEntity.CreatedBy))
                    {
                        baseEntity.CreatedBy = currentUserInfo;
                    }
                    // Set LastModifiedBy for new entities
                    baseEntity.LastModifiedBy = currentUserInfo;
                }
                else if (entry.State == EntityState.Modified)
                {
                    // Set LastModifiedBy for modified entities
                    baseEntity.LastModifiedBy = currentUserInfo;
                }
            }
        }
    }
    
    private string GetCurrentUserInfo()
    {
        string fullName = "System";
        string username = "System";
        long userId = -1;
        
        if (_userResolverService != null)
        {
            try
            {
                // Get UserId
                userId = _userResolverService.GetUserId();
                
                // Try to get full name first
                var fullNameResult = _userResolverService.fullname();
                if (!string.IsNullOrEmpty(fullNameResult) && fullNameResult != "Unknown")
                {
                    fullName = fullNameResult;
                }
                
                // Get username
                var userNameResult = _userResolverService.GetUser();
                if (!string.IsNullOrEmpty(userNameResult) && userNameResult != "Unknown")
                {
                    username = userNameResult;
                    // If full name is not available, use username as fallback
                    if (fullName == "System")
                    {
                        fullName = username;
                    }
                }
            }
            catch (Exception)
            {
                // Ignore exceptions and use defaults
            }
        }
        
        // Combine all information: FullName | Username | UserId
        var parts = new List<string>();
        if (!string.IsNullOrEmpty(fullName) && fullName != "System")
        {
            parts.Add(fullName);
        }
        if (!string.IsNullOrEmpty(username) && username != "System" && username != fullName)
        {
            parts.Add(username);
        }
        if (userId > 0)
        {
            parts.Add($"UserId:{userId}");
        }
        
        return parts.Count > 0 ? string.Join(" | ", parts) : "System";
    }
    
    public BaseDbContext()
    {

    }
    public BaseDbContext(DbContextOptions options, UserResolverService userService) : base(options)
    {

        try
        {

            _connectionString = ((SqlServerOptionsExtension)options.Extensions.Last()).ConnectionString;
            _userResolverService = userService;
        }
        catch (Exception eee)
        {

            //throw;
        }

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var basePath = Directory.GetCurrentDirectory();
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .AddJsonFile($"appsettings.{environment}.json", optional: true, reloadOnChange: false)
                .AddEnvironmentVariables()
                .Build();

            var rawConn =
                configuration.GetConnectionString("HRMSConnection")
                ?? Environment.GetEnvironmentVariable("ConnectionStrings__HRMSConnection")
                ?? Environment.GetEnvironmentVariable("HRMS__Connection");
            var connectionString = HR.SharedKernel.Security.ConnectionStringProtector.TryUnprotect(rawConn) ?? rawConn;

            if (!string.IsNullOrWhiteSpace(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
        // Suppress EF relational warning for cross-table FK mapping during design-time
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.ForeignKeyPropertiesMappedToUnrelatedTables));
    }

    void IAppDbContext.Add<TEntity>(TEntity objModel)
    {
        if (objModel == null)
        {
            throw new ArgumentNullException(nameof(objModel));
        }

        // تنظیم مقادیر پایه برای موجودیت‌های مشتق از BaseEntity
        if (objModel is BaseEntity baseEntity)
        {
            baseEntity.CreateDate = DateTime.Now;
            if (_userResolverService != null)
            {
                try
                {
                    baseEntity.IPAddress = _userResolverService.GetIP();
                }
                catch
                {
                    baseEntity.IPAddress = "Local";
                }
            }
        }

        Set<TEntity>().Add(objModel);
    }

    public void AddRange<TEntity>(IQueryable<TEntity> objModel) where TEntity : class
    {
        if (objModel == null)
        {
            throw new ArgumentNullException(nameof(objModel));
        }

        foreach (var item in objModel)
        {
            if (item is BaseEntity baseEntity)
            {
                baseEntity.CreateDate = DateTime.Now;
                if (_userResolverService != null)
                {
                    try
                    {
                        baseEntity.IPAddress = _userResolverService.GetIP();
                    }
                    catch
                    {
                        baseEntity.IPAddress = "Local";
                    }
                }
            }
        }

        Set<TEntity>().AddRange(objModel);
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
        if (objModel == null)
        {
            throw new ArgumentNullException(nameof(objModel));
        }

        if (objModel is BaseEntity baseEntity)
        {
            // اعتبارسنجی تاریخ اعتبار در صورت وجود
            if (baseEntity.StartDate > baseEntity.EndDate)
            {
                throw new ArgumentException("تاریخ اعتبار صحیح وارد نشده");
            }

            baseEntity.LastModifiedDate = DateTime.Now;

            if (_userResolverService != null)
            {
                try
                {
                    baseEntity.IPAddress = _userResolverService.GetIP();
                }
                catch
                {
                    baseEntity.IPAddress = "Local";
                }
            }

            // نگه‌داشتن CreateDate اصلی
            var dbValues = Entry(objModel).GetDatabaseValues();
            if (dbValues != null)
            {
                var createDateObj = dbValues.GetValue<object>("CreateDate");
                if (createDateObj != null)
                {
                    baseEntity.CreateDate = Convert.ToDateTime(createDateObj);
                }
                else if (baseEntity.CreateDate == default)
                {
                    baseEntity.CreateDate = DateTime.Now;
                }
            }
        }

        Entry(objModel).State = EntityState.Modified;
    }

    public void LogicalRemove<TEntity>(long Id) where TEntity : class
    {
        if (Id == 0)
        {
            throw new ArgumentNullException(nameof(Id));
        }

        var entity = Set<TEntity>().Find(Id);
        if (entity == null)
        {
            throw new InvalidOperationException("Entity not found");
        }

        if (entity is BaseEntity baseEntity)
        {
            baseEntity.IsDeleted = true;
            baseEntity.LastModifiedDate = DateTime.Now;

            if (_userResolverService != null)
            {
                try
                {
                    baseEntity.IPAddress = _userResolverService.GetIP();
                }
                catch
                {
                    baseEntity.IPAddress = "Local";
                }
            }

            Entry(entity).State = EntityState.Modified;
        }
        else
        {
            // اگر موجودیت از BaseEntity نیست، حذف فیزیکی انجام می‌شود
            Set<TEntity>().Remove(entity);
        }
    }

    void IAppDbContext.Remove<TEntity>(TEntity objModel)
    {
        if (objModel == null)
        {
            throw new ArgumentNullException(nameof(objModel));
        }

        Set<TEntity>().Remove(objModel);
    }
}

