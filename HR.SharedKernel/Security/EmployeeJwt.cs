using Castle.Components.DictionaryAdapter.Xml;
using HR.SharedKernel.API;
using HR.SharedKernel.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Security;

public class EmployeeJwt
{
    public IConfiguration _Configuration { get; }
    private readonly RequestDelegate _next;
    protected readonly ILogger<EmployeeJwt> _logger;

    public EmployeeJwt(RequestDelegate next, IConfiguration configuration, ILogger<EmployeeJwt> Logger)
    {
        _next = next;
        _Configuration = configuration;
        _logger = Logger;
    }
    public async Task Invoke(HttpContext context)
    {
        try
        {
            var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (token != null)
            {
                _logger.LogInformation("EmployeeJwt Authorization header found");
                var userId = attachUserToContext(context, token);
                if (userId != null)
                {
                    _logger.LogInformation("userId  " + userId.Value + " attachedUserToContext ");
                }
                else
                {
                    _logger.LogInformation("user Not attachedUserToContext ");
                }
            }
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, ex.Message);
            throw;
        }
    }

    private long? attachUserToContext(HttpContext context, string token)
    {
        try
        {
            if (string.IsNullOrEmpty(token) || token == "null")
            {
                return null;
            }
            token = token.Replace("\"", "");
            var tokenHandler = new JwtSecurityTokenHandler();
            byte[] key = Encoding.UTF8.GetBytes(_Configuration.GetSection("Jwt:Key").Value);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidIssuer = _Configuration.GetSection("Jwt:JWTIssuer").Value,// ValidIssuer,
                ValidAudience = _Configuration.GetSection("Jwt:Audience").Value,
                // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;

            // Security: Username is not stored in JWT token - get userId from token instead
            var userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "userId");
            if (userIdClaim == null)
            {
                // Fallback to old claim name for backward compatibility during migration
                userIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "currentUserId");
            }
            
            if (userIdClaim == null || string.IsNullOrEmpty(userIdClaim.Value))
            {
                return null;
            }

            // Decrypt userId from token (supports both encrypted and plain format for backward compatibility)
            long currentUserIdLong;
            var userIdEncryptionService = context.RequestServices.GetService<UserIdEncryptionService>();
            var decryptedUserId = userIdEncryptionService?.DecryptUserId(userIdClaim.Value) ?? -1;
            if (decryptedUserId > 0)
            {
                currentUserIdLong = decryptedUserId;
            }
            else
            {
                // Fallback: Try to parse as plain number (backward compatibility with old tokens)
                if (!long.TryParse(userIdClaim.Value, out currentUserIdLong))
                {
                    return null;
                }
            }

            var currentUserId = currentUserIdLong.ToString();
            context.Items["currentUserId"] = currentUserId;

            var connectionString = _Configuration.GetSection("ConnectionStrings:HRMSConnection").Value;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                var sql = "SELECT [Id]\r\n,[salt]\r\n      ,[FirstName]\r\n ,[ExpiresOn]\r\n      ,[NationalNo]\r\n,[LastToken]\r\n      ,[LastName]\r\n      ,[ActiveName]\r\n      FROM [emp].[Employee]\r\n  where id = \r\n" + currentUserId;
                SqlCommand cmd = new SqlCommand(sql, con);
                cmd.CommandType = CommandType.Text;
                con.Open();
                SqlDataReader rdr = cmd.ExecuteReader();
                string userName = null;
                while (rdr.Read())
                {
                    var Id = rdr["Id"];
                    userName = rdr["ActiveName"]?.ToString();
                    var LastToken = rdr["LastToken"];
                    var ExpiresOn = rdr["ExpiresOn"];
                    if (LastToken == null)
                    {
                        con.Close();
                        context.Items["User"] = null;
                        context.Items["currentUserId"] = null;
                        return null;
                    }
                    else
                    {
                        if (token == LastToken.ToString())
                        {
                            con.Close();
                        }
                        else
                        {
                            con.Close();
                            context.Items["User"] = null;
                            context.Items["currentUserId"] = null;
                            return null;
                        }
                    }
                    break;
                }
                // Set username from database query result (not from token for security)
                if (!string.IsNullOrEmpty(userName))
                {
                    context.Items["User"] = userName;
                }
            }

            // Get fullname from token (claim name is "fullname")
            var fullnameClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "fullname" || x.Type == "CurrentUserFullName");
            if (fullnameClaim != null)
            {
                context.Items["CurrentUserFullName"] = fullnameClaim.Value;
            }

            // Get employee ID from token (optional claim)
            var currentUserEmployeeIdClaim = jwtToken.Claims.FirstOrDefault(x => x.Type == "currentUserEmployeeId");
            if (currentUserEmployeeIdClaim != null)
            {
                context.Items["currentUserEmployeeId"] = currentUserEmployeeIdClaim.Value;
            }

            // Populate HttpContext.User so UserResolverService and [Authorize] can read JWT claims
            context.User = new ClaimsPrincipal(new ClaimsIdentity(jwtToken.Claims, "Bearer"));

            return currentUserIdLong;

        }
        catch (Exception ex)
        {
            _logger.LogInformation(" Error in  attachUserToContext with Token : " + token);
            _logger.LogCritical(ex, ex.Message);
            return null;
            //throw new Exception("Error in attachUserToContext " + ex.Message);
        }
    }
}

