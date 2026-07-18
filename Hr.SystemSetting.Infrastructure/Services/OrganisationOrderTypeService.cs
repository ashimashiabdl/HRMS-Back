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
    public class OrganisationOrderTypeService : BaseService<OrganisationOrderType, SystemSettingContext, OrganisationOrderTypeDTO>, IScopedServices
    {
        public OrganisationOrderTypeService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        public OperationResult GetAsTree(int? Id)
        {

            var list = All()
                .Include(i => i.OrderType)
                .Include(i => i.OrderTypeGroup)
                .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId).ToList();

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
                    var childList = list.Where(i => i.OrderTypeGroupId == item.OrderTypeGroupId);
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
        public new OperationResult GetAsKeyValuePair()
        {
            return OperationResult.Succeeded(payload: All().Include(i => i.OrderType).Where(i => i.OrganisationChartId == _currentUserDefaultOrganId).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.OrderTypeId,
                value = i.OrderType.title
            }));
        }

        public  OperationResult GetAllBatchAblesAsKeyValuePair()
        {
            return OperationResult.Succeeded(payload: All().Include(i => i.OrderType).Where(i => i.IsBatch == true && i.OrganisationChartId == _currentUserDefaultOrganId).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.OrderTypeId,
                value = i.OrderType.title
            }));
        }
        public bool Validate(OrganisationOrderType entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
