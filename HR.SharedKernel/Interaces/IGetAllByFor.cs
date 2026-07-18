using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Interaces
{
    public interface IGetAllByFor<TResult, TKey> where TResult : class
    {
        IQueryable<TResult> GetAllByFor(TKey key);
    }
}
