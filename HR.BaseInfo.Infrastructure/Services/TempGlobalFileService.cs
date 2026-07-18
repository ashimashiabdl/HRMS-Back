using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.SharedKernel.Dapper;
using File = HR.BaseInfo.Core.Entities.File;
using HR.BaseInfo.infrastructure.Data;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services
{
    public class TempGlobalFileService : BaseService<TempGlobalFile, BaseInfoContext, TempGlobalFileDTO>, IScopedServices
    {
        public TempGlobalFileService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }
        public File? GetMainFile(long FileId)
        {
            return _unitOfWork.Context.Files.Find(FileId);
        }
        public bool Validate(TempGlobalFile entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
