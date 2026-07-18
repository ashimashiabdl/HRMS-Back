using AutoMapper;
using DynamicExpressions.Mapping;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using LinqKit;
using Microsoft.EntityFrameworkCore;
using NLog.LayoutRenderers;

using Microsoft.Extensions.Configuration;

namespace Hr.SystemSetting.Infrastructure.Services
{
    public class OrganisationEmployeeStatusService : BaseService<OrganisationEmployeeStatus, SystemSettingContext, OrganisationEmployeeStatusDTO>, IScopedServices
    {
        public OrganisationEmployeeStatusService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public new OperationResult GetAsKeyValuePair()
        {
            return OperationResult.Succeeded(payload: All().Where(i => i.OrganisationChartId == _currentUserDefaultOrganId).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.EmployeeStatus.Id,
                value =  i.EmployeeStatus.title
            }));
        }
        public  OperationResult GetAsKeyValuePair(DateTime dt)
        {
            return GetAsKeyValuePairByOrganisationChartId(_currentUserDefaultOrganId, dt);
        }

        public OperationResult GetAsKeyValuePairByOrganisationChartId(long organisationChartId, DateTime dt)
        {
            return OperationResult.Succeeded(payload: All(ImpleDate: dt).Include(i => i.EmployeeStatusGroup).Where(i => i.OrganisationChartId == organisationChartId).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.EmployeeStatus.Id,
                value = "( " + i.EmployeeStatusGroup.title + " ) " + i.EmployeeStatus.title
            }));
        }

        public OperationResult GetAsTree(int? ParentId)
        {

            var list = All()
                .Include(i => i.EmployeeStatus)
                .Include(i => i.EmployeeStatusGroup)
                .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId).ToList();

            List<TreeNode> Tree = new List<TreeNode>();

            foreach (var item in list)
            {
                if (Tree.Any(i => i.Id == item.EmployeeStatusGroupId))
                {

                }
                else
                {
                    TreeNode toAdd = new TreeNode();
                    toAdd.Id = item.EmployeeStatusGroupId;
                    toAdd.Title = item.EmployeeStatusGroup?.title;
                    toAdd.Children = new List<TreeNode>();
                    var childList = list.Where(i => i.EmployeeStatusGroupId == item.EmployeeStatusGroupId);
                    if (childList != null)
                    {
                        if (childList.Any())
                        {
                            foreach (var innerItem in childList)
                            {
                                toAdd.Children.Add(new TreeNode()
                                {
                                    Id = innerItem.EmployeeStatusId,
                                    Title = innerItem.EmployeeStatus?.title
                                });
                            }
                        }
                    }
                    Tree.Add(toAdd);
                }
            }


            return OperationResult.Succeeded(payload: Tree);
        }
        public bool Validate(OrganisationEmployeeStatus entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
