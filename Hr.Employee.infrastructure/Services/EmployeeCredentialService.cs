using System.Security.Cryptography;
using System.Text;
using Hr.Employee.infrastructure.Data;
using HR.Employee.Core.DTOs;
using HR.SharedKernel;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Security;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

namespace Hr.Employee.infrastructure.Services;

public class EmployeeCredentialService(
    IUnitOfWork<EmployeeContext> unitOfWork,
    UserResolverService userResolverService) : IScopedServices
{
    private const int KeySize = 64;
    private const int Iterations = 350000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    private readonly IUnitOfWork<EmployeeContext> _unitOfWork = unitOfWork;
    private readonly UserResolverService _userResolverService = userResolverService;

    public bool VerifyHashedPassword(string password, string hash, byte[] salt)
    {
        var hashToCompare = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithm, KeySize);
        return CryptographicOperations.FixedTimeEquals(hashToCompare, Convert.FromHexString(hash));
    }

    public string HashPassword(string password, out byte[] salt)
    {
        salt = RandomNumberGenerator.GetBytes(KeySize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            salt,
            Iterations,
            HashAlgorithm,
            KeySize);

        return Convert.ToHexString(hash);
    }

    public async Task<OperationResult> UpdateCurrentUserPassword(UpdatePassCurrentEmployeeDTO entityToUpdate)
    {
        var user = await _unitOfWork.Context.Employees.FindAsync(entityToUpdate.EmployeeId);
        if (user == null)
        {
            return OperationResult.NotFound();
        }

        if (string.IsNullOrEmpty(entityToUpdate.newpass))
        {
            return OperationResult.Failed("کلمه عبور جدید وارد نشده");
        }
        if (string.IsNullOrEmpty(entityToUpdate.newpassconfirm))
        {
            return OperationResult.Failed("تکرار کلمه عبور جدید وارد نشده");
        }
        if (entityToUpdate.newpass != entityToUpdate.newpassconfirm)
        {
            return OperationResult.Failed("کلمه عبور جدید و تکرار آن مطابقت ندارد");
        }
        if (!PassWordRegex.IsMatch(entityToUpdate.newpass))
        {
            return OperationResult.Failed("کلمه عبور پیچیدگی لازم را ندارد ");
        }

        user.PasswordHash = HashPassword(entityToUpdate.newpass, out var salt);
        user.salt = salt;
        user.LastModifiedDate = DateTime.Now;
        user.IPAddress = _userResolverService.GetIP();
        _unitOfWork.Context.Employees.Update(user);

        if (await _unitOfWork.Save() > 0)
        {
            return OperationResult.Succeeded(payload: 1);
        }

        return OperationResult.Failed();
    }
}
