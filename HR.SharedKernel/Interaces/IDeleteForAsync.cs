using HR.SharedKernel.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Interaces
{
    public interface IDeleteForAsync<TKey, TUserKey>
    {
        Task<OperationResult> DeleteForAsync(TKey key, TUserKey userKey);
    }
}
