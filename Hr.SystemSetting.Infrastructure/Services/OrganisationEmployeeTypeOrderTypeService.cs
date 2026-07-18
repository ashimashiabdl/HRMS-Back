using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;

using Hr.SystemSetting.Infrastructure.Data;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using LinqKit;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace Hr.SystemSetting.Infrastructure.Services
{
    public class OrganisationEmployeeTypeOrderTypeService : BaseService<OrganisationEmployeeTypeOrderType, SystemSettingContext, OrganisationEmployeeTypeOrderTypeDTO>, IScopedServices
    {
        public OrganisationEmployeeTypeOrderTypeService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        public OperationResult GetAsTree(int? Id)
        {
            return GetAsTreeByOrganisationChartId(_currentUserDefaultOrganId, Id);
        }

        public OperationResult GetAsTreeByOrganisationChartId(long organisationChartId, int? employeeTypeId)
        {
            var activeRecords = All()
               .Include(i => i.OrderType)
               .Where(i => i.OrganisationChartId == organisationChartId
               && i.EmployeeTypeId == employeeTypeId
               ).Select(i => i.OrderTypeId).ToList();


            var list = _db.Set<OrganisationOrderType>()
                .Include(i => i.OrderTypeGroup)
                .Include(i => i.OrderType)
                .Where(DateValidityExtension<OrganisationOrderType>.GetDateValidationPredicate().And(i => i.OrganisationChartId == organisationChartId).And(j => activeRecords.Contains(j.OrderTypeId))).ToList();
            List<TreeNode> Tree = new List<TreeNode>();

            foreach (var item in list)
            {
                if (Tree.Any(i => i.Id == item.OrderTypeGroupId))
                {

                }
                else
                {
                    TreeNode toAdd = new TreeNode();
                    toAdd.Id = item.OrderTypeGroupId;
                    toAdd.Title = item.OrderTypeGroup?.title;
                    toAdd.Children = new List<TreeNode>();
                    var childList = list.Where(i => i.OrderTypeGroupId == item.OrderTypeGroupId).ToList();
                    if (childList != null)
                    {
                        if (childList.Any())
                        {
                            foreach (var innerItem in childList)
                            {
                                toAdd.Children.Add(new TreeNode()
                                {
                                    Id = innerItem.OrderTypeId,
                                    Title = innerItem.OrderType?.title
                                });
                            }
                        }
                    }
                    Tree.Add(toAdd);
                }
            }


            return OperationResult.Succeeded(payload: Tree);
        }

        public bool Validate(OrganisationEmployeeTypeOrderType entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
