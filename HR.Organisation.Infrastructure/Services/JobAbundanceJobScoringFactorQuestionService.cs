using AutoMapper;
using AutoMapper.Configuration.Annotations;
using HR.Organisation.Core.DTOs;
using HR.Organisation.Core.Entities;
using HR.Organisation.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;

using Microsoft.Extensions.Configuration;

namespace HR.Organisation.Infrastructure.Services
{
    public class JobAbundanceJobScoringFactorQuestionService : BaseService<JobAbundanceJobScoringFactorQuestion, OrganisationContext, JobAbundanceJobScoringFactorQuestionDTO>, IScopedServices
    {
        private AbundanceService _abundanceService;
        public JobAbundanceJobScoringFactorQuestionService(IMapper mapper, AbundanceService AbundanceService, IUnitOfWork<OrganisationContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {
            _abundanceService = AbundanceService;
        }
        public OperationResult GetAbundanceQuestionsAsKeyValuePair(long id, long id1)
        {
            var Abundances = _abundanceService.All().ToList();
            var questions = All().Where(i => i.OrganizationJobId == id && i.JobScoringFactorId == id1).ToList();
            foreach (var complexity in Abundances)
            {
                if (questions.Any(i => i.AbundanceId == complexity.Id))
                {
                    complexity.title = questions.Single(i => i.AbundanceId == complexity.Id).Question;
                }
            }
            return OperationResult.Succeeded(payload: Abundances.OrderBy(i => i.Level).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                id = i.Level,
                key = i.Id,
                value = i.title
            }));
        }
        public bool Validate(JobAbundanceJobScoringFactorQuestion entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
