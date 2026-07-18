using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Interaces
{
    public interface IGetAllFor<TResult> where TResult : class
    {
        IQueryable<TResult> GetAllFor();
    }
}
