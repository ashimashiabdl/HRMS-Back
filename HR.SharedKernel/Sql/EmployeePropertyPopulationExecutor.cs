using System.Data;
using Microsoft.Data.SqlClient;

namespace HR.SharedKernel.Sql;

/// <summary>
/// Executes [rpt].[SP_Populate_EmployeeProperty] and reads the result set returned on success.
/// </summary>
public static class EmployeePropertyPopulationExecutor
{
    public const string ProcedureName = "[rpt].[SP_Populate_EmployeeProperty]";

    public sealed record ExecutionResult(int RowsAffected, string Message);

    public sealed record ExecutionOptions(int CommandTimeoutSeconds = 600);

    public static async Task<ExecutionResult> ExecuteAsync(
        string connectionString,
        ExecutionOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        options ??= new ExecutionOptions();

        await using var connection = new SqlConnection(connectionString);
        await using var command = CreateCommand(connection, options);

        await connection.OpenAsync(cancellationToken).ConfigureAwait(false);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);

        return await ReadResultAsync(reader, cancellationToken).ConfigureAwait(false);
    }

    public static ExecutionResult Execute(string connectionString, ExecutionOptions? options = null)
    {
        options ??= new ExecutionOptions();

        using var connection = new SqlConnection(connectionString);
        using var command = CreateCommand(connection, options);

        connection.Open();
        using var reader = command.ExecuteReader();

        return ReadResult(reader);
    }

    public static string FormatSqlException(SqlException ex)
    {
        var detail = ex.Errors.Count > 0
            ? ex.Errors[0]
            : null;

        return
            $"SQL {ex.Number}: {ex.Message}" +
            (detail?.LineNumber > 0 ? $" (Line {detail.LineNumber})" : string.Empty) +
            (!string.IsNullOrWhiteSpace(detail?.Procedure) ? $" [{detail.Procedure}]" : string.Empty);
    }

    private static SqlCommand CreateCommand(SqlConnection connection, ExecutionOptions options)
    {
        var command = connection.CreateCommand();
        command.CommandType = CommandType.StoredProcedure;
        command.CommandText = ProcedureName;
        command.CommandTimeout = Math.Max(30, options.CommandTimeoutSeconds);
        return command;
    }

    private static ExecutionResult ReadResult(SqlDataReader reader)
    {
        if (!reader.Read())
        {
            return new ExecutionResult(0, "Stored procedure completed without result set.");
        }

        return MapResult(reader);
    }

    private static async Task<ExecutionResult> ReadResultAsync(SqlDataReader reader, CancellationToken cancellationToken)
    {
        if (!await reader.ReadAsync(cancellationToken).ConfigureAwait(false))
        {
            return new ExecutionResult(0, "Stored procedure completed without result set.");
        }

        return MapResult(reader);
    }

    private static ExecutionResult MapResult(SqlDataReader reader)
    {
        var rowsAffected = reader.GetInt32(reader.GetOrdinal("RowsAffected"));
        var message = reader.IsDBNull(reader.GetOrdinal("Message"))
            ? string.Empty
            : reader.GetString(reader.GetOrdinal("Message"));

        return new ExecutionResult(rowsAffected, message);
    }
}
