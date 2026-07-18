using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Security
{
    public class Identitysetting
    {
        public string Key { get; set; }
        public string JWTIssuer { get; set; }
        public string Audience { get; set; }
        // Access token lifetime in minutes (keep short)
        public int AccessTokenMinutes { get; set; } = 20;
        // Refresh token lifetime in days
        public int RefreshTokenDays { get; set; } = 7;
        // Maximum number of simultaneously active refresh tokens per user
        public int MaxActiveRefreshTokensPerUser { get; set; } = 1;
        // Minimum number of character differences required between old and new password
        public int PasswordMinDiffChars { get; set; } = 4;
        // Number of previous passwords that are disallowed for reuse
        public int PasswordHistoryDisallowCount { get; set; } = 5;
    }
}
