using AutoMapper;
using DynamicExpressions.Mapping;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HR.Identity.infrastructure.Services;

public class PermissionsService(IMapper _mapper, IdentityContext _context, IDapper dapper, UserResolverService userService)
{
    public List<Permission> GetAll()
    {
        var all = _context.Permissions;
        return  all.ToList();
    }
}
