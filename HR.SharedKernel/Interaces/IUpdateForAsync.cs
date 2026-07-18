using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Interaces
{
    public interface IUpdateForAsync<TInput, TKey> where TInput : BaseDTO
    {
        Task<OperationResult> UpdateForAsync(TKey key, TInput entityToUpdate);
    }
}
