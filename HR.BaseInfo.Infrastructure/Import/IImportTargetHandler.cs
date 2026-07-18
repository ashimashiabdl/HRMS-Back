using HR.BaseInfo.Core.Entities;

using HR.BaseInfo.infrastructure.Data;

using HR.SharedKernel.DTOs;



namespace HR.BaseInfo.infrastructure.Import;



public interface IImportTargetHandler

{

    bool CanHandle(string targetEntityName);



    Task<OperationResult?> ValidateUploadContextAsync(

        ImportProfile profile,

        string? contextJson,

        long organisationChartId) =>

        Task.FromResult<OperationResult?>(null);



    Task ValidateAndResolveRowsAsync(

        BaseInfoContext context,

        ImportProfile profile,

        List<ImportTempRow> tempRows);



    Task<int> FinalizeAsync(BaseInfoContext context, ImportBatch batch, string? ipAddress);



    Task<OperationResult?> RollbackFinalizedBatchAsync(BaseInfoContext context, ImportBatch batch) =>

        Task.FromResult<OperationResult?>(null);

}

