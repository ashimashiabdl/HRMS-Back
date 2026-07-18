using System.Data;
using System.Text.RegularExpressions;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HR.BaseInfo.infrastructure.Import;

public class ImportEmployeeLookupService : IScopedServices
{
    private readonly ILogger<ImportEmployeeLookupService> _logger;
    private readonly string _connectionString;

    public ImportEmployeeLookupService(ILogger<ImportEmployeeLookupService> logger, IConfiguration configuration)
    {
        _logger = logger;
        var raw = configuration.GetConnectionString("HRMSConnection");
        var decrypted = HR.SharedKernel.Security.ConnectionStringProtector.TryUnprotect(raw);
        _connectionString = decrypted ?? raw ?? "";
    }

    public static string? NormalizeNationalNo(string? raw)
    {
        if (raw == null) return null;
        var digitsOnly = Regex.Replace(ExcelImportParser.NormalizeDigitsToEnglish(raw), @"\D", "");
        if (string.IsNullOrEmpty(digitsOnly)) return null;
        if (digitsOnly.Length > 10) digitsOnly = digitsOnly[..10];
        return digitsOnly.PadLeft(10, '0');
    }

    public long GetEmployeeIdFromNationalNo(string nationalNo)
    {
        try
        {
            using var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
            using var command = connection.CreateCommand();
            command.CommandType = CommandType.StoredProcedure;
            command.CommandText = "[emp].[GetEmployeeIdFromNationalNo]";
            command.Parameters.AddWithValue("@NationalNO", nationalNo);
            var returnValue = command.Parameters.Add("@RETURN_VALUE", SqlDbType.BigInt);
            returnValue.Direction = ParameterDirection.ReturnValue;
            connection.Open();
            command.ExecuteNonQuery();
            return Convert.ToInt64(returnValue.Value ?? 0);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "GetEmployeeIdFromNationalNo failed for {NationalNo}", nationalNo);
            return 0;
        }
    }

    public string? GetEmployeeFullName(long employeeId)
    {
        if (employeeId <= 0) return null;
        try
        {
            using var connection = new Microsoft.Data.SqlClient.SqlConnection(_connectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText =
                "SELECT TOP 1 LTRIM(RTRIM(ISNULL(FirstName, N'') + N' ' + ISNULL(LastName, N''))) FROM [emp].[Employee] WHERE Id = @Id AND IsDeleted = 0";
            command.Parameters.AddWithValue("@Id", employeeId);
            var result = command.ExecuteScalar();
            var name = result?.ToString()?.Trim();
            return string.IsNullOrWhiteSpace(name) ? null : name;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "GetEmployeeFullName failed for EmployeeId={EmployeeId}", employeeId);
            return null;
        }
    }
}
