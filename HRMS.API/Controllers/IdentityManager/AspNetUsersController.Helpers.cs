using HR.Identity.Core.Entities;
using HRMS.API.Controllers.IdentityManager.Model;
using HRMS.API.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace HRMS.API.Controllers.IdentityManager;

public partial class AspNetUsersController
{
    private static readonly string[] PersianMonthNames =
    [
        string.Empty,
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    ];

    private const string UnknownLoginDateText = "نا مشخص";
    private const string UsersListVerificationPrefix = "UsersList_Verified";
    private const string UsersAddEditVerificationPrefix = "UsersAddEdit_Verified";

    private bool IsUsersListVerified() => IsSessionVerified(UsersListVerificationPrefix);

    private bool IsUsersAddEditVerified() => IsSessionVerified(UsersAddEditVerificationPrefix);

    private bool IsSessionVerified(string keyPrefix) =>
        HttpContext.Session.GetString($"{keyPrefix}_{currentUserId}") == "true";

    private bool TryValidateCaptcha(CaptchaService captchaService, out string? errorMessage)
    {
        var captchaId = Request.Headers["X-Captcha-Id"].FirstOrDefault();
        var captchaCode = Request.Headers["X-Captcha-Code"].FirstOrDefault();

        if (string.IsNullOrEmpty(captchaId) || string.IsNullOrEmpty(captchaCode))
        {
            errorMessage = "کد امنیتی الزامی است";
            return false;
        }

        if (!captchaService.Validate(captchaId, captchaCode))
        {
            errorMessage = "کد امنیتی صحیح نیست";
            return false;
        }

        errorMessage = null;
        return true;
    }

    private static bool TryDecryptRsaValue(string? value, RsaKeyService rsaKeyService, out string? plaintext)
    {
        if (string.IsNullOrEmpty(value))
        {
            plaintext = value;
            return true;
        }

        if (!value.StartsWith("enc::", StringComparison.Ordinal))
        {
            plaintext = value;
            return true;
        }

        var parts = value.Split(["::"], StringSplitOptions.None);
        if (parts.Length != 3)
        {
            plaintext = null;
            return false;
        }

        if (rsaKeyService.TryDecrypt(parts[1], parts[2], out var plain) && !string.IsNullOrEmpty(plain))
        {
            plaintext = plain;
            return true;
        }

        plaintext = null;
        return false;
    }

    private static string FormatPersianDateTime(DateTime dateTime)
    {
        var persianCalendar = new PersianCalendar();
        var month = persianCalendar.GetMonth(dateTime);
        var monthName = month >= 1 && month < PersianMonthNames.Length
            ? PersianMonthNames[month]
            : string.Empty;

        return $"{persianCalendar.GetDayOfMonth(dateTime):00}    {monthName} {persianCalendar.GetYear(dateTime)} ساعت {dateTime.Hour:00}:{dateTime.Minute:00}";
    }

    private static string FormatLastLoginDate(DateTime? lastLoginDate) =>
        lastLoginDate.HasValue ? FormatPersianDateTime(lastLoginDate.Value) : UnknownLoginDateText;

    private static List<Node> MapPermissionNodes(IReadOnlyList<Permission> permissions)
    {
        var nodes = new List<Node>(permissions.Count);
        foreach (var permission in permissions)
        {
            nodes.Add(new Node
            {
                Id = permission.Id,
                Name = permission.DisplayName,
                Key = permission.Name,
                ParentId = permission.ParentId,
            });
        }

        return nodes;
    }

    private async Task<HashSet<string>> BuildCurrentUserClaimSetAsync()
    {
        var userClaims = await _identityContext.UserClaims
            .AsNoTracking()
            .Where(i => i.UserId == currentUserId)
            .Select(i => i.ClaimType)
            .ToListAsync();

        var roleIds = await _identityContext.UserRoles
            .AsNoTracking()
            .Where(i => i.UserId == currentUserId)
            .Select(i => i.RoleId)
            .ToListAsync();

        if (roleIds.Count == 0)
        {
            return userClaims.ToHashSet(StringComparer.Ordinal);
        }

        var roleClaims = await _identityContext.RoleClaims
            .AsNoTracking()
            .Where(i => roleIds.Contains(i.RoleId))
            .Select(i => i.ClaimType)
            .ToListAsync();

        return userClaims.Concat(roleClaims).ToHashSet(StringComparer.Ordinal);
    }

    public static Node? FilterPermissionTree(Node node, HashSet<string> userPermissionIds)
    {
        var filteredChildren = new List<Node>(node.Children.Count);
        foreach (var child in node.Children)
        {
            var filteredChild = FilterPermissionTree(child, userPermissionIds);
            if (filteredChild != null)
            {
                filteredChildren.Add(filteredChild);
            }
        }

        if (userPermissionIds.Contains(node.Key) || filteredChildren.Count > 0)
        {
            return new Node
            {
                Id = node.Id,
                Name = node.Name,
                Key = node.Key,
                Children = filteredChildren
            };
        }

        return null;
    }
}
