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
    public class OrganisationEmployeeTypeService : BaseService<OrganisationEmployeeType, SystemSettingContext, OrganisationEmployeeTypeDTO>, IScopedServices
    {
        public OrganisationEmployeeTypeService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        public OperationResult GetAsTree(int? ParentId)
        {
            return GetAsTreeByOrganisationChartId(_currentUserDefaultOrganId);
        }

        public OperationResult GetAsTreeByOrganisationChartId(long organisationChartId)
        {
            var list = All()
                .Include(i => i.EmployeeType)
                .Include(i => i.EmployeeTypeGroup)
                .Where(i => i.OrganisationChartId == organisationChartId).ToList();

            List<TreeNode> Tree = new List<TreeNode>();

            foreach (var item in list)
            {
                if (Tree.Any(i => i.Id == item.EmployeeTypeGroupId))
                {

                }
                else
                {
                    TreeNode toAdd = new TreeNode();
                    toAdd.Id = item.EmployeeTypeGroupId;
                    toAdd.Title = item.EmployeeTypeGroup?.title;
                    toAdd.Children = new List<TreeNode>();
                    var childList = list.Where(i => i.EmployeeTypeGroupId == item.EmployeeTypeGroupId);
                    if (childList != null)
                    {
                        if (childList.Any())
                        {
                            foreach (var innerItem in childList)
                            {
                                toAdd.Children.Add(new TreeNode()
                                {
                                    Id = innerItem.EmployeeTypeId,
                                    Title = innerItem.EmployeeType?.title
                                });
                            }
                        }
                    }
                    Tree.Add(toAdd);
                }
            }


            return OperationResult.Succeeded(payload: Tree);
        }
        public new OperationResult GetAsKeyValuePair()
        {
            return OperationResult.Succeeded(payload: All().Include(i => i.EmployeeType).Where(i => i.OrganisationChartId == _currentUserDefaultOrganId).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.EmployeeType.Id,
                value = i.EmployeeType.title
            }));
        }
        public bool Validate(OrganisationEmployeeType entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
