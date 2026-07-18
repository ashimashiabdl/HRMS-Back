using HR.Identity.Core.DTOs;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Interaces;
using HR.SharedKernel.Security;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.Core.Interfaces
{
    /// <summary>
    /// در این سرویس متد های احراز هویت و ورود به سیستم و امنیت سیستم قرار دارد
    /// </summary>
    public interface IHRAuthenticationService : ICreateForAsync<RegisterUserDTO>
    {
        ClientStorageDTO GenerateToken(string username);
        string HashPassword(RegisterUserDTO user, string password, out byte[] salt);
        /// <summary>
        /// متد احراز هویت و ورود به سیستم
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task<OperationResult> Login(LoginDTO user);
        bool VerifyHashedPassword(string password, string hash, byte[] salt);
    }
}
