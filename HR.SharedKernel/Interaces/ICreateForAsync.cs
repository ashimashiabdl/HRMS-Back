using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Interaces
{
    public interface ICreateForAsync<TInput> where TInput : BaseDTO
    {
        Task<OperationResult> CreateForAsync(TInput entityToCreate);
    }
}
