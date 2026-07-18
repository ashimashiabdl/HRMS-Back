using AutoMapper;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Share;
using LinqKit;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

using System.Linq.Expressions;
using System.Reflection;



namespace HR.SharedKernel.Service;

public abstract class BaseService<T, C, D> : GenericRepository<T, C>
    where T : BaseEntity
    where D : BaseDTO
    where C : DbContext, IAppDbContext

{
    public IMapper _mapper;

    public long _currentUserDefaultOrganId;
    public long _currentUserDefaultPaymentPeriod;
    public readonly IUnitOfWork<C> _unitOfWork;
    protected readonly IDapper _dapper;

    protected string _connectionString;
    protected BaseService(C Context, IMapper mapper, IUnitOfWork<C> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(Context, userService)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
        _dapper = dapper;

        var raw = configuration.GetConnectionString("HRMSConnection");
        var dec = HR.SharedKernel.Security.ConnectionStringProtector.TryUnprotect(raw);
        _connectionString = dec ?? raw;
    }

    public new IQueryable<T> All(bool IgnoreExpired = true, DateTime? ImpleDate = null, bool ignoreDefaultOrganId = false)
    {
        var query = base.All(IgnoreExpired, ImpleDate);
        return query.WhereDefaultOrganisationChartId(_currentUserDefaultOrganId, ignoreDefaultOrganId);
    }

    public OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: All().OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.Id,
            value = i.title
        }));
    }

    public string? GetSettingById(long SettingId)
    {
        if (_currentUserDefaultOrganId == 0)
        {
            return "15";
        }

        using SqlConnection con = new(_connectionString);
        using SqlCommand cmd = new("[Setting].[GetCurrentOrganSettingValue]", con);
        cmd.Parameters.AddWithValue("@PayLocationId", _currentUserDefaultOrganId);
        cmd.Parameters.AddWithValue("@SettingId", SettingId);
        cmd.CommandType = System.Data.CommandType.StoredProcedure;
        con.Open();
        using SqlDataReader rdr = cmd.ExecuteReader();
        string? str = null;
        while (rdr.Read())
        {
            str = rdr.GetString(0);
        }

        return str;
    }

    public async Task<OperationResult> UpdateForAsync(D entityToUpdate)
    {
        var mappedTodo = _mapper.Map<T>(entityToUpdate);
        if (string.IsNullOrEmpty(mappedTodo.title))
        {
            mappedTodo.title = "";
        }

        if (!CheckDateRangeNoOverLap(mappedTodo)
            && !typeof(T).GetInterfaces().Contains(typeof(IignoreDateRangeValidation)))
        {
            return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
        }

        Update(mappedTodo);
        if (await _unitOfWork.Save() > 0)
        {
            return OperationResult.Succeeded(payload: 1);
        }

        return OperationResult.Failed();
    }

    public OperationResult GetPagedData(
        int currentPage = 1,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string Sortdirection = "",
        bool IgnoreExpired = true,
        long? SelectedEmployeeTypeId = null,
        long? EmployeeId = null,
        IQueryable<T> CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {

        var all = CustomDataSource ?? All(IgnoreExpired, ignoreDefaultOrganId: IgnoreDefaultOrganId);
        if (CustomDataSource != null)
            all = all.WhereDefaultOrganisationChartId(_currentUserDefaultOrganId, IgnoreDefaultOrganId);

        // در صورت استفاده از DataSource پیش‌فرض، navigation ها را مثل متد Get با استفاده از EF metadata include می‌کنیم
        if (CustomDataSource == null && typeof(T).Name != "OrganisationMRT")
        {
            var entityType = _unitOfWork.Context.Model.FindEntityType(typeof(T));
            if (entityType != null)
            {
                var navigations = entityType.GetNavigations();
                foreach (var navigation in navigations)
                {
                    all = all.Include(navigation.Name);
                }
            }
        }
        if (SelectedEmployeeTypeId > 0 && HasEntityProperty("EmployeeTypeId"))
        {
            var xParam = Expression.Parameter(typeof(T), "x");
            var colExpr = Expression.Property(xParam, "EmployeeTypeId");
            var constExpr = Expression.Constant(SelectedEmployeeTypeId);
            var lambdaBody = Expression.MakeBinary(ExpressionType.Equal, colExpr, constExpr);
            var lambda = Expression.Lambda<Func<T, bool>>(lambdaBody, xParam);
            all = all.Where(lambda);
        }

        if (EmployeeId > 0 && HasEntityProperty("EmployeeId"))
        {
            var xParam = Expression.Parameter(typeof(T), "x");
            var colExpr = Expression.Property(xParam, "EmployeeId");
            var constExpr = Expression.Constant(EmployeeId);
            var lambdaBody = Expression.MakeBinary(ExpressionType.Equal, colExpr, constExpr);
            var lambda = Expression.Lambda<Func<T, bool>>(lambdaBody, xParam);
            all = all.Where(lambda);
        }
        int rowCount = 0;
        var FlatList = _mapper.Map<List<D>>(PagerUtility<T>.GetPagedData(all, out rowCount, currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection));
        return OperationResult.Succeeded(payload: FlatList, rowCount: rowCount);

    }

    public OperationResult Get(long id)
    {

        var all = All(false);
        var entityType = _unitOfWork.Context.Model.FindEntityType(typeof(T));
        if (entityType != null)
        {
            var navigations = entityType.GetNavigations();
            foreach (var navigation in navigations)
            {
                all = all.Include(navigation.Name);
            }
        }
        var row = all.SingleOrDefault(i => i.Id == id);
        var record = _mapper.Map<D>(row);
        if (record == null)
        {
            return OperationResult.NotFound();
        }
        else
        {
            return OperationResult.Succeeded(payload: record);
        }


    }
    public async Task<OperationResult> CreateForAsync(D entityToCreate)
    {

        var mappedTodo = _mapper.Map<T>(entityToCreate);
        if (string.IsNullOrEmpty(mappedTodo.title))
        {
            mappedTodo.title = "";
        }
        if (typeof(T).GetInterfaces().Contains(typeof(IOrganisationChartId)))
        {
            if (_currentUserDefaultOrganId > 0)
            {
                PropertyInfo? propertyInfo = mappedTodo.GetType().GetProperty("OrganisationChartId");
                if (propertyInfo == null)
                {
                    throw new InvalidOperationException("OrganisationChartId property was not found on entity.");
                }

                propertyInfo.SetValue(
                    mappedTodo,
                    Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType),
                    null);
            }
            else
            {
                throw new InvalidOperationException("سازمان پیش فرض مشخض نشده است");
            }

        }

        if (CheckDateRangeNoOverLap(mappedTodo) || typeof(T).GetInterfaces().Contains(typeof(IignoreDateRangeValidation)))
        {
            Add(mappedTodo);
            if (await _unitOfWork.Save() > 0)
            {
                return OperationResult.Succeeded(payload: mappedTodo.Id);
            }
            return OperationResult.Failed();
        }
        else
        {
            return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
        }


    }
    public OperationResult DeleteRecord(long Id)
    {
        LogicalRemove(Id);
        try
        {
            if (_unitOfWork.Save().GetAwaiter().GetResult() > 0)
            {
                return OperationResult.Succeeded(payload: 1);
            }

            return OperationResult.Failed();
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.GetBaseException().Message);
        }
    }

    private static bool HasEntityProperty(string propertyName)
    {
        return typeof(T).GetProperty(propertyName) != null;
    }

    public bool CheckDateRangeNoOverLap(T Entity)
    {
        if (Entity.StartDate == null && !typeof(T).GetInterfaces().Contains(typeof(IignoreDateRangeValidation))) return false;
        var OverLappedRecored = _db.Set<T>().Where(HR.SharedKernel.Extensions.DateValidityExtension<T>.CheckDateRangeOverLap(Entity));
        if (typeof(T).GetInterfaces().Contains(typeof(IignoreDateRangeValidation)))
        {
            return true;
        }

        if (!OverLappedRecored.Any())
        {
            return true;
        }

        var Lits = OverLappedRecored.ToList();
        var type = typeof(T);
        var predicate = PredicateBuilder.New<T>(true);
        foreach (var property in type.GetProperties())
        {
            var overlapAttribute = property.CustomAttributes
                .FirstOrDefault(i => i.AttributeType == typeof(IsEffectiveInDateOverLapChecking));
            if (overlapAttribute == null)
            {
                continue;
            }

            var isEffectiveArgument = overlapAttribute.NamedArguments
                .FirstOrDefault(i => i.MemberName == "IsEffective");
            if (isEffectiveArgument.TypedValue.Value is not true)
            {
                continue;
            }

            var value = Share.Helper.GetPropValue(Entity, property.Name);
            predicate = predicate.And(i => Helper.GetPropertyAsString(i, property.Name) == Convert.ToString(value));
        }

        if (Entity.Id > 0)
        {
            predicate = predicate.And(i => i.Id != Entity.Id);
        }

        return !Lits.Any(predicate);
    }
}
