using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Interaces
{
    public interface IFindForAsync<TResult, TKey> where TResult : class
    {
        Task<TResult> FindForAsync(TKey key);
    }
}
