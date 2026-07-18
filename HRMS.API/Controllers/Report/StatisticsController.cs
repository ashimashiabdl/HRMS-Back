using AutoMapper;
using Hr.Employee.infrastructure.Services;
using HR.Organisation.Infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using Microsoft.AspNetCore.Mvc;
using HRMS.API.Cache;
using System.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Dapper;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace HRMS.API.Controllers.Report;

[Route("api/Statistics")]
[ControllerGroup("Report", "گزارشات")] 
[DisplayName("آمار سازمانی")]
public class StatisticsController : AppBaseController
{
    private readonly EmployeeService _employeeService;
    private readonly OrganisationChartService _organisationChartService;
    private readonly IDapper _dapper;

    public StatisticsController(EmployeeService employeeService, OrganisationChartService organisationChartService, ILogger<StatisticsController> logger, IHttpContextAccessor accessor, IMapper mapper, IDapper dapper, UserResolverService UserResolverService)
        : base(UserResolverService, logger, accessor, mapper, dapper)
    {
        _employeeService = employeeService;
        _employeeService._currentUserDefaultOrganId = currentUserDefaultOrganId;

        _organisationChartService = organisationChartService;
        _organisationChartService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _dapper = dapper;
    }

        [HttpGet, Route("TotalEmployees")]
        [CustomAccessKey(AccessKey: "TotalEmployees")]
        public IActionResult GetTotalEmployees()
        {
            var count = _employeeService.GetAccessibleEmployeesQueryable(currentUserId).Count();
            return this.AppOk(new { total = count });
        }

    [HttpGet, Route("EmployeesByBaseOrganisation")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetEmployeesByBaseOrganisation()
    {
        // Get only employees current user can access
        var query = _employeeService.GetAccessibleEmployeesQueryable(currentUserId);

        var grouped = query
            .GroupBy(e => e.BaseOrganisationId)
            .Select(g => new { BaseOrganisationId = g.Key, Count = g.Count() })
            .ToList();

        var orgIds = grouped.Where(x => x.BaseOrganisationId.HasValue).Select(x => x.BaseOrganisationId.Value).ToList();
        var orgTitles = _organisationChartService.All()
            .Where(o => orgIds.Contains(o.Id))
            .Select(o => new { o.Id, Title = o.title })
            .ToList();

        var result = grouped.Select(x => new
        {
            BaseOrganisationId = x.BaseOrganisationId,
            Title = x.BaseOrganisationId.HasValue ? orgTitles.FirstOrDefault(o => o.Id == x.BaseOrganisationId.Value)?.Title ?? "نامشخص" : "نامشخص",
            Count = x.Count
        })
        .OrderByDescending(x => x.Count)
        .ToList();

        return this.AppOk(result);
    }

    [HttpGet, Route("EmployeesByGender")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetEmployeesByGender()
    {
        // Get only employees current user can access
        var query = _employeeService.GetAccessibleEmployeesQueryable(currentUserId);

        var grouped = query
            .GroupBy(e => e.GenderId)
            .Select(g => new { GenderId = g.Key, Count = g.Count() })
            .ToList();

        var genderIds = grouped.Where(x => x.GenderId.HasValue).Select(x => x.GenderId.Value).ToList();
        
        // Get gender titles from BaseTableValue using Dapper
        var titleLookup = new Dictionary<long, string>();
        if (genderIds.Any())
        {
            var sqlWithIn = $"SELECT Id, title FROM bas.Base_Table_Value WHERE Id IN ({string.Join(",", genderIds)}) AND IsDeleted = 0";
            var titles = _dapper.GetAll<dynamic>(sqlWithIn, new DynamicParameters(), CommandType.Text);
            titleLookup = titles.ToDictionary(x => (long)x.Id, x => (string)x.title);
        }

        var result = grouped.Select(x => new
        {
            GenderId = x.GenderId,
            Gender = x.GenderId.HasValue && titleLookup.ContainsKey(x.GenderId.Value)
                ? titleLookup[x.GenderId.Value]
                : "نامشخص",
            Count = x.Count
        })
        .OrderByDescending(x => x.Count)
        .ToList();

        return this.AppOk(result);
    }

    [HttpGet, Route("EmployeesByMaritalStatus")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetEmployeesByMaritalStatus()
    {
        // Get only employees current user can access
        var query = _employeeService.GetAccessibleEmployeesQueryable(currentUserId);

        var grouped = query
            .GroupBy(e => e.MaritalStatusId)
            .Select(g => new { MaritalStatusId = g.Key, Count = g.Count() })
            .ToList();

        var maritalStatusIds = grouped.Where(x => x.MaritalStatusId.HasValue).Select(x => x.MaritalStatusId.Value).ToList();
        
        // Get marital status titles from BaseTableValue using Dapper
        var titleLookup = new Dictionary<long, string>();
        if (maritalStatusIds.Any())
        {
            var sqlWithIn = $"SELECT Id, title FROM bas.Base_Table_Value WHERE Id IN ({string.Join(",", maritalStatusIds)}) AND IsDeleted = 0";
            var titles = _dapper.GetAll<dynamic>(sqlWithIn, new DynamicParameters(), CommandType.Text);
            titleLookup = titles.ToDictionary(x => (long)x.Id, x => (string)x.title);
        }

        var result = grouped.Select(x => new
        {
            MaritalStatusId = x.MaritalStatusId,
            MaritalStatus = x.MaritalStatusId.HasValue && titleLookup.ContainsKey(x.MaritalStatusId.Value)
                ? titleLookup[x.MaritalStatusId.Value]
                : "نامشخص",
            Count = x.Count
        })
        .OrderByDescending(x => x.Count)
        .ToList();

        return this.AppOk(result);
    }
}


