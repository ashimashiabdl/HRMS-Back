using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers.IdentityManager;

public partial class UserDefaultSettingController
{
    private IQueryable<UserDefaultSetting> BuildFilteredQuery(bool ignoreExpired, long? userId) =>
        _context.Set<UserDefaultSetting>()
            .AsNoTracking()
            .Include(i => i.User)
            .Include(i => i.DefaultOrgan)
            .Where(DateValidityExtension<UserDefaultSetting>.GetDateValidationPredicate(ignoreExpired)
                .And(i => userId == null || i.UserId == userId));

    private async Task EnrichPagedPayloadWithOrganTitlesAsync(
        OperationResult result,
        IQueryable<UserDefaultSetting> filteredQuery)
    {
        if (result is not { Success: true } || result.Payload is not System.Collections.IEnumerable payloadEnumerable)
        {
            return;
        }

        var payloadList = payloadEnumerable.Cast<UserDefaultSettingDTO>().ToList();
        if (payloadList.Count == 0)
        {
            result.Payload = payloadList;
            return;
        }

        var dtoIdSet = payloadList
            .Where(d => d.Id.HasValue)
            .Select(d => d.Id!.Value)
            .ToHashSet();

        if (dtoIdSet.Count == 0)
        {
            result.Payload = payloadList;
            return;
        }

        var organTitlesById = await filteredQuery
            .Where(e => dtoIdSet.Contains(e.Id))
            .Select(e => new { e.Id, Title = e.DefaultOrgan != null ? e.DefaultOrgan.title : null })
            .ToDictionaryAsync(e => e.Id, e => e.Title);

        foreach (var dto in payloadList)
        {
            if (dto.Id.HasValue && organTitlesById.TryGetValue(dto.Id.Value, out var organTitle))
            {
                dto.DefaultOrganTitle = organTitle;
            }
        }

        result.Payload = payloadList;
    }
}
