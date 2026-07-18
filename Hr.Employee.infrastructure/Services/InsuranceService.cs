using AutoMapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.DTOs;
using HR.BaseInfo.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Linq;

using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services
{
    public class InsuranceService : BaseService<HR.Employee.Core.Entities.Insurance, EmployeeContext, InsuranceDTO>, IScopedServices
    {
        public InsuranceService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        public new async Task<OperationResult> CreateForAsync(InsuranceDTO entityToCreate)
        {
            try
            {
                var mapped = _mapper.Map<HR.Employee.Core.Entities.Insurance>(entityToCreate);
                if (string.IsNullOrEmpty(mapped.title))
                {
                    mapped.title = "";
                }
                if (typeof(HR.Employee.Core.Entities.Insurance).GetInterfaces().Contains(typeof(IOrganisationChartId)))
                {
                    if (_currentUserDefaultOrganId > 0)
                    {
                        var propertyInfo = mapped.GetType().GetProperty("OrganisationChartId");
                        propertyInfo.SetValue(mapped, Convert.ChangeType(_currentUserDefaultOrganId, propertyInfo.PropertyType), null);
                    }
                    else
                    {
                        throw new Exception("سازمان پیش فرض مشخض نشده است");
                    }
                }

                _unitOfWork.CreateTransaction();
                try
                {
                    if (mapped.IsLast == true)
                    {
                        var others = _unitOfWork.Context.Insurances
                            .Where(x => x.EmployeeId == mapped.EmployeeId && x.IsDeleted != true && x.IsLast == true);
                        foreach (var item in others)
                        {
                            item.IsLast = false;
                            _unitOfWork.Context.Entry(item).State = EntityState.Modified;
                        }
                    }

                    Add(mapped);
                    await _unitOfWork.Save();
                    _unitOfWork.Commit();
                    return OperationResult.Succeeded(payload: mapped.Id);
                }
                catch (Exception)
                {
                    _unitOfWork.Rollback();
                    return OperationResult.Failed();
                }
            }
            catch (Exception)
            {
                return OperationResult.Failed();
            }
        }

        public new async Task<OperationResult> UpdateForAsync(InsuranceDTO entityToUpdate)
        {
            try
            {
                var mapped = _mapper.Map<HR.Employee.Core.Entities.Insurance>(entityToUpdate);

                _unitOfWork.CreateTransaction();
                try
                {
                    if (mapped.IsLast == true)
                    {
                        var others = _unitOfWork.Context.Insurances
                            .Where(x => x.EmployeeId == mapped.EmployeeId && x.Id != mapped.Id && x.IsDeleted != true && x.IsLast == true);
                        foreach (var item in others)
                        {
                            item.IsLast = false;
                            _unitOfWork.Context.Entry(item).State = EntityState.Modified;
                        }
                    }

                    Update(mapped);
                    await _unitOfWork.Save();
                    _unitOfWork.Commit();
                    return OperationResult.Succeeded(payload: 1);
                }
                catch (Exception)
                {
                    _unitOfWork.Rollback();
                    return OperationResult.Failed();
                }
            }
            catch (Exception)
            {
                return OperationResult.Failed();
            }
        }
        public new OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? SelectedEmployeeTypeId = null, long? EmployeeId = null, IQueryable<HR.Employee.Core.Entities.Insurance>? CustomDataSource = null, bool IgnoreDefaultOrganId = false)
        {
            var result = base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, SelectedEmployeeTypeId, EmployeeId, CustomDataSource, IgnoreDefaultOrganId);

            if (result.Success && result.Payload is List<InsuranceDTO> list)
            {
                var insWorkShopTypeIds = list.Where(x => x.InsWorkShopTypeId.HasValue)
                                             .Select(x => x.InsWorkShopTypeId!.Value)
                                             .Distinct()
                                             .ToList();

                if (insWorkShopTypeIds.Any())
                {
                    // BaseTableId = 68 corresponds to InsWorkShopType values
                    var values = _unitOfWork.Context.Set<BaseTableValue>()
                        .Where(b => b.BaseTableId == 68 && insWorkShopTypeIds.Contains(b.Id))
                        .Select(b => new { b.Id, b.title })
                        .ToList();

                    var dict = values.ToDictionary(k => k.Id, v => v.title);

                    foreach (var dto in list)
                    {
                        if (dto.InsWorkShopTypeId.HasValue && dict.TryGetValue(dto.InsWorkShopTypeId.Value, out var title))
                        {
                            dto.InsWorkShopType = title;
                        }
                    }
                }

                // Supplementary insurance type (BaseTableId = 79)
                var supplementaryTypeIds = list.Where(x => x.SupplementaryInsuranceTypeId.HasValue)
                                               .Select(x => x.SupplementaryInsuranceTypeId!.Value)
                                               .Distinct()
                                               .ToList();

                if (supplementaryTypeIds.Any())
                {
                    var suppValues = _unitOfWork.Context.Set<BaseTableValue>()
                        .Where(b => b.BaseTableId == 79 && supplementaryTypeIds.Contains(b.Id))
                        .Select(b => new { b.Id, b.title })
                        .ToList();

                    var suppDict = suppValues.ToDictionary(k => k.Id, v => v.title);

                    foreach (var dto in list)
                    {
                        if (dto.SupplementaryInsuranceTypeId.HasValue && suppDict.TryGetValue(dto.SupplementaryInsuranceTypeId.Value, out var title))
                        {
                            dto.SupplementaryInsuranceType = title;
                        }
                    }
                }

                // InsuranceType from Payroll schema
                var insuranceTypeIds = list.Where(x => x.InsuranceTypeId.HasValue)
                                         .Select(x => x.InsuranceTypeId!.Value)
                                         .Distinct()
                                         .ToList();

                if (insuranceTypeIds.Any())
                {
                    // Create a simple class for the result
                    var insuranceTypeResults = new List<(long Id, string Title)>();
                    
                    // Use Dapper with parameterized query for safety
                    using (var connection = _dapper.GetDbconnection())
                    {
                        connection.Open();
                        var sql = @"
                            SELECT Id, title as Title
                            FROM [Payroll].[Insurance_Type] 
                            WHERE Id IN @Ids AND IsDeleted = 0";
                        
                        var parameters = new Dapper.DynamicParameters();
                        parameters.Add("Ids", insuranceTypeIds);
                        
                        insuranceTypeResults = connection.Query<(long Id, string Title)>(sql, new { Ids = insuranceTypeIds }).ToList();
                        connection.Close();
                    }

                    var insuranceTypeDict = insuranceTypeResults.ToDictionary(k => k.Id, v => v.Title);

                    foreach (var dto in list)
                    {
                        if (dto.InsuranceTypeId.HasValue && insuranceTypeDict.TryGetValue(dto.InsuranceTypeId.Value, out var title))
                        {
                            dto.InsuranceType = title;
                        }
                    }
                }
            }

            return result;
        }
        public OperationResult GetAsKeyValuePair(long id)
        {
            return OperationResult.Succeeded(payload: All(false)
                .Include(i => i.InsWorkShopType)
                .Where(i => i.EmployeeId == id)
                .Select(i => new HR.SharedKernel.Data.KeyValuePair()
                {
                    key = i.Id,
                    value = (i.InsWorkShopType == null ? "" : i.InsWorkShopType.title + " ( " + i.Id + " ) ") + " - " + " - " + (i.InsuranceNumber == null ? "" : i.InsuranceNumber)
                }));
        }

        public OperationResult GetComputableAccDaySum(long organisationChartId, long employeeId)
        {
            var sum = _unitOfWork.Context.Insurances
                .Where(x => x.OrganisationChartId == organisationChartId
                    && x.EmployeeId == employeeId
                    && x.IsComputable == true
                    && x.IsDeleted != true)
                .Sum(x => x.AccDay ?? 0);

            return OperationResult.Succeeded(payload: sum);
        }
        public bool Validate(HR.Employee.Core.Entities.Insurance entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
