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
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Data;

namespace HR.Organisation.Infrastructure.Services;

public class OrganisationPositionService : BaseService<OrganisationPosition, OrganisationContext, OrganisationPositionDTO>, IScopedServices
{
    public OrganisationPositionService(IMapper mapper, IUnitOfWork<OrganisationContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
        : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
    }

    public new OperationResult GetAsKeyValuePair()
    {
        var query = _unitOfWork.Context.Set<OrganisationPosition>()
            .AsNoTracking()
            .Where(i => i.IsDeleted != true);

        if (_currentUserDefaultOrganId > 0)
        {
            query = query.Where(i => i.OrganisationChartId == _currentUserDefaultOrganId);
        }

        var payload = query
            .OrderByDescending(i => i.Id)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair
            {
                id = i.Id,
                key = i.Id,
                value = (i.Position != null ? i.Position.title : "") + " ( " + (i.PositionCode ?? "") + " ) "
            })
            .ToList();

        return OperationResult.Succeeded(payload: payload);
    }

    public OperationResult GetChartNodePositions(long Chart_Node, DateTime ImpleDate)
    {
        var relatedPositions = new List<GetChartNodePositions_Result>();
        using (var con = new SqlConnection(_connectionString))
        {
            var cmd = new SqlCommand("[Organisation].[GetChartNodePositions]", con);
            cmd.Parameters.Add("@Chart_Node", SqlDbType.BigInt).Value = Chart_Node;
            cmd.Parameters.Add("@OrganisationChartId", SqlDbType.BigInt).Value = _currentUserDefaultOrganId;
            cmd.Parameters.Add("@ImpleDate", SqlDbType.Date).Value = ImpleDate.Date;
            cmd.CommandType = CommandType.StoredProcedure;
            con.Open();
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
            {
                relatedPositions.Add(rdr.ConvertToObject<GetChartNodePositions_Result>());
            }
        }
        return OperationResult.Succeeded(payload: relatedPositions);
    }

    /// <summary>
    /// Returns distinct RelatedNodeId values that have at least one organisation position.
    /// </summary>
    public OperationResult GetRelatedNodeIds()
    {
        var ids = All()
            .AsNoTracking()
            .Select(i => i.RelatedNodeId)
            .Distinct()
            .ToList();
        return OperationResult.Succeeded(payload: ids);
    }

    /// <summary>
    /// پست‌های مرتبط با گره چارت از طریق RelatedNodeId.
    /// از All() تاریخ‌محور استفاده نمی‌شود تا رکورد موجود در جدول به‌خاطر Start/EndDate از قلم نیفتد.
    /// </summary>
    public OperationResult GetByRelatedNodeId(long relatedNodeId, DateTime? impleDate = null)
    {
        List<OrganisationPosition> Load(bool restrictToCurrentOrgan, bool applyDateFilter)
        {
            var query = _unitOfWork.Context.Set<OrganisationPosition>()
                .AsNoTracking()
                .Include(i => i.Position)
                .Include(i => i.PositionType)
                .Include(i => i.InsurancePosition)
                .Include(i => i.RelatedNode)
                .Include(i => i.OrganisationChart)
                .Where(i => i.RelatedNodeId == relatedNodeId && i.IsDeleted != true);

            if (restrictToCurrentOrgan && _currentUserDefaultOrganId > 0)
            {
                query = query.Where(i => i.OrganisationChartId == _currentUserDefaultOrganId);
            }

            if (applyDateFilter && impleDate.HasValue)
            {
                var d = impleDate.Value.Date;
                query = query.Where(a =>
                    ((a.StartDate != null && a.StartDate.Value.Date <= d) || a.StartDate == null)
                    && (a.EndDate == null || a.EndDate > d));
            }

            return query.OrderByDescending(i => i.Id).ToList();
        }

        var rows = Load(restrictToCurrentOrgan: true, applyDateFilter: impleDate.HasValue);
        if (rows.Count == 0 && impleDate.HasValue)
        {
            rows = Load(restrictToCurrentOrgan: true, applyDateFilter: false);
        }
        if (rows.Count == 0)
        {
            // رکورد در جدول هست ولی OrganisationChartId با سازمان جاری یکی نیست
            rows = Load(restrictToCurrentOrgan: false, applyDateFilter: false);
        }

        return OperationResult.Succeeded(payload: _mapper.Map<List<OrganisationPositionDTO>>(rows));
    }

    public bool Validate(OrganisationPosition entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
