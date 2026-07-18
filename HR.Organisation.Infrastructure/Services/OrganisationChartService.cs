using AutoMapper;
using HR.Organisation.Core.DTOs;
using HR.Organisation.Core.Entities;
using HR.Organisation.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using Microsoft.EntityFrameworkCore;
using System.Drawing;
using System.Net;
using System.Reflection;

using Microsoft.Extensions.Configuration;

namespace HR.Organisation.Infrastructure.Services;

public class OrganisationChartService(IMapper mapper, IUnitOfWork<OrganisationContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<OrganisationChart, OrganisationContext, OrganisationChartDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public new async Task<OperationResult> CreateForAsync(OrganisationChartDTO entityToCreate)
    {
        _unitOfWork.CreateTransaction();
        try
        {
            var mappedTodo = _mapper.Map<OrganisationChart>(entityToCreate);
            if (typeof(OrganisationChart).GetInterfaces().Contains(typeof(IOrganisationChartId)))
            {
                PropertyInfo propertyInfo = mappedTodo.GetType().GetProperty("OrganisationChartId");
                propertyInfo.SetValue(mappedTodo, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
            }

            if (CheckDateRangeNoOverLap(mappedTodo) || typeof(OrganisationChart).GetInterfaces().Contains(typeof(IignoreDateRangeValidation)))
            {
                Add(mappedTodo);
                _unitOfWork.Context.SaveChanges();

                if (entityToCreate.tempFileId > 0)
                {
                    var tempfile = _unitOfWork.Context.Set<TashkilatTempFile>().Find(entityToCreate.tempFileId);
                    _unitOfWork.Context.Set<OrganisationChartImage>().Add(new OrganisationChartImage()
                    {
                        Content = tempfile.Content,
                        CreateDate = DateTime.Now,
                        MimeType = tempfile.MimeType,
                        OrganisationChartId = mappedTodo.Id,
                        Size = tempfile.Size,
                        UniqueId = Guid.NewGuid(),
                        StartDate = mappedTodo.StartDate,
                        IsDeleted = false,
                        IPAddress = "",
                        title = tempfile.title
                    });
                }
                _unitOfWork.Context.SaveChanges();

                _unitOfWork.Commit();
                return OperationResult.Succeeded(payload: mappedTodo.Id);

            }
            else
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            throw;
        }

    }
    public new async Task<OperationResult> UpdateForAsync(OrganisationChartDTO entityToUpdate)
    {
        _unitOfWork.CreateTransaction();
        try
        {
            var mappedTodo = _mapper.Map<OrganisationChart>(entityToUpdate);

            if (entityToUpdate.tempFileId > 0)
            {
                var tempfile = _unitOfWork.Context.Set<TashkilatTempFile>().Find(entityToUpdate.tempFileId);

                var existingImage = _unitOfWork.Context.Set<OrganisationChartImage>().Where(i => i.OrganisationChartId == entityToUpdate.Id);

                if (existingImage == null)
                {
                    _unitOfWork.Context.Set<OrganisationChartImage>().Add(new OrganisationChartImage()
                    {

                        Content = tempfile.Content,
                        CreateDate = DateTime.Now,
                        MimeType = tempfile.MimeType,
                        OrganisationChartId = entityToUpdate.Id.Value,
                        Size = tempfile.Size,
                        UniqueId = Guid.NewGuid(),
                        StartDate = entityToUpdate.StartDate,
                        IsDeleted = false,
                        IPAddress = "",
                        title = tempfile.title
                    });
                }
                else
                {
                    if (existingImage.Any())
                    {
                        var Exist = _unitOfWork.Context.Set<OrganisationChartImage>().Find(existingImage.Single().Id);
                        Exist.Content = tempfile.Content;
                        Exist.CreateDate = DateTime.Now;
                        Exist.MimeType = tempfile.MimeType;
                        Exist.OrganisationChartId = entityToUpdate.Id.Value;
                        Exist.Size = tempfile.Size;
                        Exist.UniqueId = Guid.NewGuid();
                        Exist.StartDate = entityToUpdate.StartDate;
                        Exist.IsDeleted = false;
                        Exist.IPAddress = "";
                        Exist.title = tempfile.title;
                        _unitOfWork.Context.Update(Exist);
                    }
                    else
                    {
                        _unitOfWork.Context.Set<OrganisationChartImage>().Add(new OrganisationChartImage()
                        {
                            Content = tempfile.Content,
                            CreateDate = DateTime.Now,
                            MimeType = tempfile.MimeType,
                            OrganisationChartId = entityToUpdate.Id.Value,
                            Size = tempfile.Size,
                            UniqueId = Guid.NewGuid(),
                            StartDate = entityToUpdate.StartDate,
                            IsDeleted = false,
                            IPAddress = "",
                            title = tempfile.title
                        });
                    }
                }

            }
            _unitOfWork.Context.SaveChanges();
            Update(mappedTodo);
            if (CheckDateRangeNoOverLap(mappedTodo))
            {
                if (await _unitOfWork.Save() > 0)
                {
                    _unitOfWork.Commit();
                    return OperationResult.Succeeded(payload: 1);
                }
                else
                {
                    _unitOfWork.Rollback();
                    return OperationResult.Failed();

                }
            }
            else
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
            }
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            throw;
        }


    }
    public string GetCurrentNodePath(long? NodeId)
    {
        var Current = All().SingleOrDefault(i => i.Id == NodeId);
        if (Current.ParentOrganisationChartId == null)
        {
            return "ریشه";
        }
        else
        {
            var parent = All().SingleOrDefault(i => i.Id == NodeId);
            var path = GetCurrentNodePath(Current.ParentOrganisationChartId.Value) + " / " + parent.title;
            return path;
        }
    }

    public async Task<OperationResult> UpdateNodeParent(OrganisationChartDTO Node)
    {
        var mappedTodo = GetIdAsync(Node.Id.Value).Result;
        mappedTodo.ParentOrganisationChartId = Node.ParentOrganisationChartId;
        Update(mappedTodo);
        if (CheckDateRangeNoOverLap(mappedTodo))
        {
            if (await _unitOfWork.Save() > 0)
            {
                return OperationResult.Succeeded(payload: 1);
            }
            else { return OperationResult.Failed(); }
        }
        else
        {
            return OperationResult.Failed("در بازه زمانی انتخابی ردیف تکراری وجود دارد");
        }
    }
    public OperationResult GetAsTree(double? ParentId)
    {
        try
        {
            if (ParentId == 0)
            {
                ParentId = All().SingleOrDefault(i => i.IsRoot == true).Id;
            }

            var list = All()
      .Include(i => i.OrganizationType)
      .Include(i => i.ParentOrganisationChart)

      .Where(i => i.ParentOrganisationChartId == ParentId).ToList();

            var positionRelatedNodeIds = _unitOfWork.Context.OrganisationPositions
                .AsNoTracking()
                .Where(p => p.OrganisationChartId == _currentUserDefaultOrganId)
                .Select(p => p.RelatedNodeId)
                .Distinct()
                .ToHashSet();

            List<TreeNode> Tree = new List<TreeNode>();

            foreach (var item in list)
            {
                if (Tree.Any(i => i.Id == item.ParentOrganisationChartId))
                {

                }
                else
                {
                    TreeNode toAdd = new TreeNode();
                    toAdd.Id = item.ParentOrganisationChartId;
                    toAdd.Title = item.ParentOrganisationChart?.title;//+ (innerItem.OrganizationType == null ? "" : " ( " + innerItem.OrganizationType.title + " ) ")
                    toAdd.HasOrganisationPosition = item.ParentOrganisationChartId.HasValue
                        && positionRelatedNodeIds.Contains(item.ParentOrganisationChartId.Value);
                    toAdd.Children = new List<TreeNode>();
                    var childList = list.Where(i => i.ParentOrganisationChartId == item.ParentOrganisationChartId);
                    if (childList != null)
                    {
                        if (childList.Any())
                        {
                            foreach (var innerItem in childList)
                            {
                                toAdd.Children.Add(new TreeNode()
                                {
                                    Id = innerItem.Id,
                                    Title = innerItem.title + (innerItem.OrganizationType == null ? "" : " ( " + innerItem.OrganizationType.title + " ) "),
                                    HasOrganisationPosition = positionRelatedNodeIds.Contains(innerItem.Id)
                                });
                            }
                        }
                    }
                    Tree.Add(toAdd);
                }
            }
            if (ParentId == null)
            {
                Tree.Add(new TreeNode() { });
            }

            return OperationResult.Succeeded(payload: Tree);
        }
        catch (Exception ex)
        {

            throw;
        }



    }
    public OperationResult GetAsTreePrimeng(double? ParentId)
    {
        try
        {
            if (ParentId == 0)
            {
                ParentId = All().SingleOrDefault(i => i.IsRoot == true).Id;
            }

            var list = All()
      .Include(i => i.OrganizationType)
      .Include(i => i.ParentOrganisationChart)
      .Include(i => i.Place);

            //.Where(i => i.ParentOrganisationChartId == ParentId).ToList();

            List<PrimeTreeNode<OrganisationChart>> Tree = new List<PrimeTreeNode<OrganisationChart>>();

            foreach (var item in list)
            {
                Tree.Add(new PrimeTreeNode<OrganisationChart>()
                {
                    Id = item.Id,
                    pid = item.ParentOrganisationChartId,
                    OrganizationType = item.OrganizationType?.title,
                    Place = item.Place?.title,
                    name = item.title,
                    isPayLocation = item.IsPayLocation
                });
            }
            return OperationResult.Succeeded(payload: Tree);
        }
        catch (Exception ex)
        {

            throw;
        }



    }
    public OperationResult GetAllPayLocationsAsKeyValuePair(DateTime dt)
    {
        return OperationResult.Succeeded(payload: All(ImpleDate: dt).Where(i => i.IsPayLocation == true).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.Id,
            value = i.title
        }));
    }


    public OperationResult GetAllAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: All().Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.Id,
            value = i.title
        }));
    }

    public OperationResult GetAsTreeFromRoot(long rootId)
    {
        try
        {
            if (rootId <= 0)
            {
                return OperationResult.Failed("Root ID is invalid");
            }

            // Get the root node
            var rootNode = All()
                .Include(i => i.OrganizationType)
                .Include(i => i.ParentOrganisationChart)
                .FirstOrDefault(i => i.Id == rootId);

            if (rootNode == null)
            {
                return OperationResult.Failed("Root node not found");
            }

            // Get all nodes that are descendants of the root (including the root itself)
            var allNodes = All()
                .Include(i => i.OrganizationType)
                .Include(i => i.ParentOrganisationChart)
                .ToList();

            // Build a map of all nodes by ID for quick lookup
            var nodeMap = allNodes.ToDictionary(n => n.Id);

            // Find all descendants of the root
            var descendants = new HashSet<long> { rootId };
            var queue = new Queue<long>();
            queue.Enqueue(rootId);

            while (queue.Count > 0)
            {
                var currentId = queue.Dequeue();
                var children = allNodes.Where(n => n.ParentOrganisationChartId == currentId).ToList();
                foreach (var child in children)
                {
                    if (!descendants.Contains(child.Id))
                    {
                        descendants.Add(child.Id);
                        queue.Enqueue(child.Id);
                    }
                }
            }

            // Filter to only include descendants
            var filteredNodes = allNodes.Where(n => descendants.Contains(n.Id)).ToList();

            // Build tree structure
            List<TreeNode> tree = new List<TreeNode>();

            // Create root node
            var rootTreeNode = new TreeNode
            {
                Id = rootNode.Id,
                Title = rootNode.title + (rootNode.OrganizationType == null ? "" : " ( " + rootNode.OrganizationType.title + " ) "),
                Children = new List<TreeNode>()
            };

            // Build children recursively
            BuildTreeRecursive(rootTreeNode, filteredNodes, rootId);

            tree.Add(rootTreeNode);

            return OperationResult.Succeeded(payload: tree);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"Error building tree: {ex.Message}");
        }
    }

    private void BuildTreeRecursive(TreeNode parentNode, List<OrganisationChart> allNodes, long parentId)
    {
        var children = allNodes.Where(n => n.ParentOrganisationChartId == parentId).ToList();

        foreach (var child in children)
        {
            var childTreeNode = new TreeNode
            {
                Id = child.Id,
                Title = child.title + (child.OrganizationType == null ? "" : " ( " + child.OrganizationType.title + " ) "),
                Children = new List<TreeNode>()
            };

            parentNode.Children.Add(childTreeNode);

            // Recursively build children
            BuildTreeRecursive(childTreeNode, allNodes, child.Id);
        }
    }

    public bool Validate(OrganisationChart entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
