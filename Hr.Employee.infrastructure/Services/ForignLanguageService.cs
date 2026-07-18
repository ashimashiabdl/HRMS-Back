using AutoMapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.DTOs;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using HR.SharedKernel;


using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

// Helper class for Dapper query
internal class BaseTableValueTitle
{
    public long Id { get; set; }
    public string title { get; set; }
}

public class ForignLanguageService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<HR.Employee.Core.Entities.ForeignLanguage, EmployeeContext, ForignLanguageDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public new OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? SelectedEmployeeTypeId = null, long? EmployeeId = null, IQueryable<HR.Employee.Core.Entities.ForeignLanguage> CustomDataSource = null, bool IgnoreDefaultOrganId = false)
    {
        var all = All(IgnoreExpired);
        
        if (CustomDataSource != null)
        {
            all = CustomDataSource;
        }

        // Filter by EmployeeId if provided
        if (EmployeeId > 0)
        {
            var xParam = Expression.Parameter(typeof(HR.Employee.Core.Entities.ForeignLanguage), "x");
            var colExpr = Expression.Property(xParam, "EmployeeId");
            var constExpr = Expression.Constant(EmployeeId);
            var lambdaBody = Expression.MakeBinary(ExpressionType.Equal, colExpr, constExpr);
            var lambda = Expression.Lambda<Func<HR.Employee.Core.Entities.ForeignLanguage, bool>>(lambdaBody, xParam);
            all = all.Where(lambda);
        }

        int rowCount = 0;
        var pagedData = PagerUtility<HR.Employee.Core.Entities.ForeignLanguage>.GetPagedData(all, out rowCount, currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection);
        
        // Map to DTO
        var result = _mapper.Map<List<ForignLanguageDTO>>(pagedData);
        
        // Get the IDs for lookup
        var languageIds = result.Where(r => r.LanguageId.HasValue).Select(r => r.LanguageId.Value).Distinct().ToList();
        var languageSkillIds = result.Where(r => r.LanguageskillId.HasValue).Select(r => r.LanguageskillId.Value).Distinct().ToList();
        var levelIds = result.Where(r => r.LevelId.HasValue).Select(r => r.LevelId.Value).Distinct().ToList();
        
        // Combine all IDs
        var allIds = languageIds.Union(languageSkillIds).Union(levelIds).ToList();
        
        if (allIds.Any())
        {
            // Use Dapper to get titles from BaseTableValue
            var sql = "SELECT Id, title FROM bas.Base_Table_Value WHERE Id IN @Ids AND IsDeleted = 0";
            var parms = new DynamicParameters();
            parms.Add("@Ids", string.Join(",", allIds));
            
            var sqlWithIn = $"SELECT Id, title FROM bas.Base_Table_Value WHERE Id IN ({string.Join(",", allIds)}) AND IsDeleted = 0";
            var titles = dapper.GetAll<BaseTableValueTitle>(sqlWithIn, new DynamicParameters(), CommandType.Text);
            var titleLookup = titles.ToDictionary(x => x.Id, x => x.title);
            
            // Set titles on DTOs
            foreach (var dto in result)
            {
                if (dto.LanguageId.HasValue && titleLookup.ContainsKey(dto.LanguageId.Value))
                {
                    dto.LanguageTitle = titleLookup[dto.LanguageId.Value];
                }
                
                if (dto.LanguageskillId.HasValue && titleLookup.ContainsKey(dto.LanguageskillId.Value))
                {
                    dto.LanguageskillTitle = titleLookup[dto.LanguageskillId.Value];
                }
                
                if (dto.LevelId.HasValue && titleLookup.ContainsKey(dto.LevelId.Value))
                {
                    dto.LevelTitle = titleLookup[dto.LevelId.Value];
                }
            }
        }

        return OperationResult.Succeeded(payload: result, rowCount: rowCount);
    }

    public new OperationResult Get(long id)
    {
        var all = All(false);
        var row = all.SingleOrDefault(i => i.Id == id);
        var record = _mapper.Map<ForignLanguageDTO>(row);
        
        if (record == null)
        {
            return OperationResult.NotFound();
        }
        
        // Get titles for the single record
        var allIds = new List<long>();
        if (record.LanguageId.HasValue) allIds.Add(record.LanguageId.Value);
        if (record.LanguageskillId.HasValue) allIds.Add(record.LanguageskillId.Value);
        if (record.LevelId.HasValue) allIds.Add(record.LevelId.Value);
        
        if (allIds.Any())
        {
            // Use Dapper to get titles from BaseTableValue
            var sqlWithIn = $"SELECT Id, title FROM bas.Base_Table_Value WHERE Id IN ({string.Join(",", allIds)}) AND IsDeleted = 0";
            var titles = dapper.GetAll<BaseTableValueTitle>(sqlWithIn, new DynamicParameters(), CommandType.Text);
            var titleLookup = titles.ToDictionary(x => x.Id, x => x.title);
            
            // Set titles on DTO
            if (record.LanguageId.HasValue && titleLookup.ContainsKey(record.LanguageId.Value))
            {
                record.LanguageTitle = titleLookup[record.LanguageId.Value];
            }
            
            if (record.LanguageskillId.HasValue && titleLookup.ContainsKey(record.LanguageskillId.Value))
            {
                record.LanguageskillTitle = titleLookup[record.LanguageskillId.Value];
            }
            
            if (record.LevelId.HasValue && titleLookup.ContainsKey(record.LevelId.Value))
            {
                record.LevelTitle = titleLookup[record.LevelId.Value];
            }
        }
        
        return OperationResult.Succeeded(payload: record);
    }
}
