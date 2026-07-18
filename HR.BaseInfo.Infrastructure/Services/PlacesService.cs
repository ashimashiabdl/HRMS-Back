using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel;
using HR.SharedKernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Dapper;
using Microsoft.EntityFrameworkCore;
using HR.SharedKernel.Share;
using HR.BaseInfo.infrastructure.Data;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services
{


    public class PlacesService : BaseService<Places, BaseInfoContext, PlacesDTO>, IScopedServices
    {
        public PlacesService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }


        public OperationResult GetAsKeyValuePair(string filter)
        {
            if (string.IsNullOrEmpty(filter))
            {
                return OperationResult.Succeeded(payload: new List<SharedKernel.Data.KeyValuePair> { });
            }
            else
            {
                var isNumeric = int.TryParse(filter, out int n);
                if (isNumeric)
                {
                    return OperationResult.Succeeded(payload: new List<SharedKernel.Data.KeyValuePair> { });
                }

                if (filter.Length > 2)
                {

                }
                else
                {
                    return OperationResult.Succeeded(payload: new List<SharedKernel.Data.KeyValuePair> { });
                }
            }
            return OperationResult.Succeeded(payload: All()
                .Include(i => i.PlaceType)
                .Where(i => i.title.ToLower().Trim().Contains(filter.ToLower().Trim()))
                .OrderByDescending(i => i.Id)
                .Select(i => new HR.SharedKernel.Data.KeyValuePair()
                {
                    key = i.Id,
                    value = i.title + (i.PlaceType == null ? "" : (" ( " + i.PlaceType.title + " ) "))
                }));
        }




        /// <summary>
        /// گرفتن کشور مربوطه به صورت بازگشتی
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Places GetRelatedCountryRecursive(long id)
        {
            var place = GetIdAsync(id).Result;
            if (place.PlaceTypeId == (long)Enums.PlaceType.Country)
            {
                return place;
            }
            if (place.ParentPlaceId > 0)
            {
                return GetRelatedCountryRecursive(place.ParentPlaceId.Value);
            }
            else
            {
                if (place.ParentPlaceId == null)
                {
                    return GetIdAsync(1).Result;
                }
            }
            return GetIdAsync(1).Result;
        }

        public new OperationResult GetAsKeyValuePair()
        {
            return OperationResult.Succeeded(payload: All().Include(i => i.PlaceType).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.Id,
                value = i.title + (i.PlaceType == null ? "" : (" ( " + i.PlaceType.title + " ) "))
            }));
        }

        private string GetPath(long Id, bool isRoot)
        {
            if (Id == 0)
            {
                return "";
            }
            string path = string.Empty;
            if (isRoot)
            {
                var Node = _unitOfWork.Context.Places.Include(i => i.PlaceType).Single(i => i.Id == Id);
                if (Node.ParentPlaceId == null)
                {
                    return Node.title;
                }
                else
                {
                    if (Node.ParentPlaceId > 0)
                    {
                        path = path + " " + Node.PlaceType.title + " " + Node.title + " / " + GetPath(Node.ParentPlaceId.Value, false);
                    }
                }
            }
            else
            {
                var Node = _unitOfWork.Context.Places.Include(i => i.PlaceType).Single(i => i.Id == Id);
                if (Node.ParentPlaceId == null)
                {
                    path = path + " " + Node.title;
                }
                else
                {
                    if (Node.ParentPlaceId > 0)
                    {
                        path = path + " " + Node.PlaceType.title + " " + Node.title + " / " + GetPath(Node.ParentPlaceId.Value, false);
                    }
                }
            }
            return path;
        }
        public OperationResult getFullPath(long Id)
        {
            return OperationResult.Succeeded(payload: GetPath(Id, true));
        }
        public OperationResult GetAsTree(long? ParentId)
        {

            if (ParentId == 0)
            {
                var list = All().Where(i => i.ParentPlaceId == null).Include(
     i => i.PlaceType
     )
     .ToList();

                List<TreeNode> Tree = new List<TreeNode>();
                foreach (var item in list)
                {
                    Tree.Add(
                        new TreeNode()
                        {
                            Id = item.Id,
                            ParentId = item.ParentPlaceId,
                            Title = " ( " + item.PlaceType.title + " ) " + item.title,
                            Children = new List<TreeNode>()
                        }
                        );
                }
                var root = Tree.GenerateTree(c => c.Id, c => c.ParentId).ToList();
                return OperationResult.Succeeded(payload: root);


            }
            else
            {
                var list = All().Where(i => i.ParentPlaceId == ParentId).Include(i => i.PlaceType).ToList();
                List<TreeNode> Tree = new List<TreeNode>();
                foreach (var item in list)
                {

                    Tree.Add(
             new TreeNode()
             {
                 Id = item.Id,
                 ParentId = item.ParentPlaceId,
                 Title = " ( " + item.PlaceType.title + " ) " + item.title,
                 Children = new List<TreeNode>()
             }
             );


                }
                List<TreeNode> Tree2 = new List<TreeNode>();
                var parent = GetIdAsync(ParentId.Value).Result;
                TreeNode Root = new TreeNode()
                {
                    Id = ParentId,
                    Children = Tree,
                    Title = parent.title
                };
                Tree2.Add(Root);

                var root = Tree2.GenerateTree(c => c.Id, c => c.ParentId).ToList();
                return OperationResult.Succeeded(payload: root);
            }


        }

        public bool Validate(Places entity, object etc = null)
        {
            throw new NotImplementedException();
        }

    }
}
