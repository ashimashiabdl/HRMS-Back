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
    public class JobComplexityJobScoringFactorQuestionService : BaseService<JobComplexityJobScoringFactorQuestion, OrganisationContext, JobComplexityJobScoringFactorQuestionDTO>, IScopedServices
    {
        private ComplexityService _complexityService;
        public JobComplexityJobScoringFactorQuestionService(IMapper mapper, ComplexityService ComplexityService, IUnitOfWork<OrganisationContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {
            _complexityService = ComplexityService;
        }
        public OperationResult GetComplexityQuestionsAsKeyValuePair(long id, long id1)
        {
            var complexities = _complexityService.All().ToList();
            var questions = All().Where(i => i.OrganizationJobId == id && i.JobScoringFactorId == id1).ToList();
            foreach (var complexity in complexities)
            {
                if (questions.Any(i => i.ComplexityId == complexity.Id))
                {
                    complexity.title = questions.Single(i => i.ComplexityId == complexity.Id).Question;
                }
            }
            return OperationResult.Succeeded(payload: complexities.OrderBy(i => i.Level).Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                id = i.Id,
                key = i.Level,
                value = i.title
            }));
        }
        public bool Validate(JobComplexityJobScoringFactorQuestion entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
