using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using HR.BaseInfo.infrastructure.Data;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services
{
    public class BaseTableValueService : BaseService<BaseTableValue, BaseInfoContext, BaseTableValueDTO>, IScopedServices
    {
        public BaseTableValueService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? SelectedEmployeeTypeId = null, long? EmployeeId = null, IQueryable<BaseTableValue> CustomDataSource = null, bool IgnoreDefaultOrganId = false, long? BasetableId = null)
        {
            var Properties = typeof(BaseTableValue).GetProperties();
            var all = All(IgnoreExpired);
            if (CustomDataSource != null)
            {
                all = CustomDataSource;
            }
            else
            {
                foreach (var Propertiy in Properties)
                {
                    if (typeof(BaseTableValue).Name == "OrganisationMRT")
                    {
                        break;
                    }
                    if (Propertiy.PropertyType.BaseType == typeof(BaseTableValue).BaseType)
                    {
                        all = all.Include(Propertiy.Name);
                    }
                }
            }

            if (BasetableId.HasValue && BasetableId.Value > 0)
            {
                var xParam = Expression.Parameter(typeof(BaseTableValue), "x");
                var colExpr = Expression.Property(xParam, "BaseTableId");
                var constExpr = Expression.Constant(BasetableId.Value);
                var lambdaBody = Expression.MakeBinary(ExpressionType.Equal, colExpr, constExpr);
                var lambda = Expression.Lambda<Func<BaseTableValue, bool>>(lambdaBody, xParam);
                all = all.Where(lambda);
            }

            int rowCount = 0;
            var FlatList = _mapper.Map<List<BaseTableValueDTO>>(PagerUtility<BaseTableValue>.GetPagedData(all, out rowCount, currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection));
            return OperationResult.Succeeded(payload: FlatList, rowCount: rowCount);
        }
        public OperationResult GetAsKeyValuePairValue(int id)
        {
            return OperationResult.Succeeded(payload: All().Include(i => i.BaseTable).Where(i => i.BaseTableId == id || id == -1).OrderByDescending(i=>i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.Id,
                value = i.title + (id == -1 ? " ( " + i.BaseTable.title + " ) " : "")
            }));
        }
        
        public OperationResult GetAsKeyValuePairValueIncluded(int id)
        {
            return OperationResult.Succeeded(payload: All().Include(i => i.BaseTable).Where(i => i.BaseTableId == id || id == -1).OrderByDescending(i=>i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.Id,
                value = i.Value + " - " + i.title + (id == -1 ? " ( " + i.BaseTable.title + " ) " : "")
            }));
        }
        public OperationResult GetAsKeyValuePairValueBatch(BatchTableValueRequest ids)
        {
            return OperationResult.Succeeded(payload: All().Where(i => ids.BaseTableIds.Contains(i.BaseTableId)).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                id = i.BaseTableId,
                key = i.Id,
                value = i.title
            }));
        }
     
    }
}
