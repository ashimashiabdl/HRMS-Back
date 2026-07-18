using HR.SharedKernel.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Interaces
{
    public interface ISimpleCRUD<TResult> where TResult : class
    {
        Task<OperationResult> Get(int id);
        Task<IQueryable<TResult>> GetAllAsync();
    }

}
