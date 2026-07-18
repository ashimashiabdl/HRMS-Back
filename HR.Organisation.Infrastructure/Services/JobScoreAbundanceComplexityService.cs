using AutoMapper;
using Castle.Components.DictionaryAdapter.Xml;
using HR.Organisation.Core.DTOs;
using HR.Organisation.Core.Entities;
using HR.Organisation.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace HR.Organisation.Infrastructure.Services
{
    public class JobScoreAbundanceComplexityService : BaseService<JobScoreAbundanceComplexity, OrganisationContext, JobScoreAbundanceComplexityDTO>, IScopedServices
    {
        private ComplexityService _complexityService;
        private G20ScoreDomainJobDegreeService _g20ScoreDomainJobDegreeService;
        public JobScoreAbundanceComplexityService(IMapper mapper, G20ScoreDomainJobDegreeService G20ScoreDomainJobDegreeService, ComplexityService ComplexityService, IUnitOfWork<OrganisationContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {
            _complexityService = ComplexityService;
            _g20ScoreDomainJobDegreeService = G20ScoreDomainJobDegreeService;
        }
        public OperationResult GetJobScoreSummation(long organizationJobId, bool ignoreExpired = true)
        {
            try
            {
                var query = _unitOfWork.Context.Set<JobScoreAbundanceComplexity>()
                    .Include(i => i.OrganizationJob)
                    .Where(DateValidityExtension<JobScoreAbundanceComplexity>.GetDateValidationPredicate(ignoreExpired))
                    .Where(x => x.OrganizationJobId == organizationJobId && x.JobScoringFactorId != null);

                if (_currentUserDefaultOrganId > 0)
                {
                    query = query.Where(x => x.OrganizationJob!.OrganisationChartId == _currentUserDefaultOrganId);
                }

                var selections = query
                    .Where(x => x.Selected || x.SelectedFromQuestion)
                    .ToList();

                var totalScore = selections

                    .Sum(s => s.Score);

                return OperationResult.Succeeded(payload: totalScore);
            }
            catch (Exception)
            {
                return OperationResult.Failed();
            }
        }
        public OperationResult GroupPut(GroupPutDTO data)
        {
            if (data.DetailList == null || !data.DetailList.Any())
            {
                return OperationResult.Failed("لیست امتیازها خالی است");
            }

            var organizationJobId = data.DetailList.First().OrganizationJobId;
            if (organizationJobId == null)
            {
                return OperationResult.Failed("شناسه شغل سازمانی نامعتبر است");
            }

            var factorIds = data.DetailList
                .Where(x => x.JobScoringFactorId.HasValue)
                .Select(x => x.JobScoringFactorId!.Value)
                .Distinct()
                .ToList();

            var all = All(false).Where(i => i.OrganizationJobId == organizationJobId).ToList();
            _unitOfWork.CreateTransaction();

            try
            {
                foreach (var item in data.DetailList)
                {
                    var existing = all.FirstOrDefault(i =>
                        i.JobScoringFactorId == item.JobScoringFactorId
                        && i.OrganizationJobId == item.OrganizationJobId
                        && i.ComplexityId == item.ComplexityId
                        && i.AbundanceId == item.AbundanceId);

                    if (existing != null)
                    {
                        existing.Score = item.Score ?? 0;
                        existing.StartDate = DateTime.Now.AddMonths(-3);
                        existing.EndDate = null;
                        Update(existing);
                    }
                    else if (item.Score > 0)
                    {
                        var newEntity = new JobScoreAbundanceComplexity()
                        {
                            IPAddress = "",
                            CreateDate = DateTime.Now,
                            AbundanceId = item.AbundanceId,
                            ComplexityId = item.ComplexityId,
                            Score = item.Score ?? 0,
                            IsDeleted = false,
                            StartDate = DateTime.Now.AddMonths(-12),
                            OrganizationJobId = item.OrganizationJobId,
                            JobScoringFactorId = item.JobScoringFactorId,
                            Selected = true,
                            SelectedFromQuestion = false,
                        };
                        Add(newEntity);
                        all.Add(newEntity);
                    }
                }

                foreach (var factorId in factorIds)
                {
                    var factorRecords = all
                        .Where(i => i.JobScoringFactorId == factorId)
                        .ToList();
                    ApplyTableTabSelection(
                        factorRecords,
                        data.PrimaryComplexityId,
                        data.PrimaryAbundanceId);
                }

                if (_unitOfWork.Save().Result <= 0)
                {
                    _unitOfWork.Rollback();
                    return OperationResult.Failed("خطا در ذخیره امتیازها");
                }

                _unitOfWork.Commit();
                return OperationResult.Succeeded();
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("خطا در تراکنش");
            }
        }

        private static int ResolveFactorGroupScore(List<JobScoreAbundanceComplexity> factorRecords)
        {
            var userSelections = factorRecords.Where(x => x.Selected).ToList();
            if (userSelections.Any())
            {
                return ResolveFactorScore(userSelections);
            }

            var questionSelections = factorRecords.Where(x => x.SelectedFromQuestion).ToList();
            if (questionSelections.Any())
            {
                return ResolveFactorScore(questionSelections);
            }

            return 0;
        }

        private static int ResolveFactorScore(List<JobScoreAbundanceComplexity> selections)
        {
            if (!selections.Any())
            {
                return 0;
            }

            if (selections.Count == 1)
            {
                return selections[0].Score;
            }

            var latestModified = selections.Max(x => x.LastModifiedDate ?? DateTime.MinValue);
            var primaryCandidates = selections
                .Where(x => (x.LastModifiedDate ?? DateTime.MinValue) == latestModified)
                .ToList();

            if (primaryCandidates.Count == 1)
            {
                return primaryCandidates[0].Score;
            }

            // Fallback when primary cell was not marked (same LastModifiedDate on all rows).
            return primaryCandidates
                .OrderByDescending(x => x.Score)
                .ThenByDescending(x => x.Id)
                .First()
                .Score;
        }

        private void ApplyTableTabSelection(
            List<JobScoreAbundanceComplexity> records,
            long? primaryComplexityId = null,
            long? primaryAbundanceId = null)
        {
            foreach (var record in records)
            {
                record.Selected = true;
                record.SelectedFromQuestion = false;
            }

            PersistSelectionFlags(records);
            FinalizePrimarySelection(records, primaryComplexityId, primaryAbundanceId);
        }

        private void ApplyQuestionTabSelection(
            List<JobScoreAbundanceComplexity> records,
            long? primaryComplexityId = null,
            long? primaryAbundanceId = null)
        {
            foreach (var record in records)
            {
                record.Selected = false;
                record.SelectedFromQuestion = true;
            }

            PersistSelectionFlags(records);
            FinalizePrimarySelection(records, primaryComplexityId, primaryAbundanceId);
        }

        private void FinalizePrimarySelection(
            List<JobScoreAbundanceComplexity> records,
            long? primaryComplexityId,
            long? primaryAbundanceId)
        {
            if (!primaryComplexityId.HasValue || !primaryAbundanceId.HasValue)
            {
                return;
            }

            var target = records.FirstOrDefault(record =>
                record.ComplexityId == primaryComplexityId
                && record.AbundanceId == primaryAbundanceId);

            if (target == null)
            {
                return;
            }

            target.LastModifiedDate = DateTime.UtcNow.AddSeconds(2);
            if (target.Id > 0)
            {
                Update(target);
            }
        }

        private void PersistSelectionFlags(List<JobScoreAbundanceComplexity> records)
        {
            foreach (var record in records.Where(record => record.Id > 0))
            {
                Update(record);
            }
        }

        public OperationResult UpdateSelectedItem(JobScoreAbundanceComplexityDTO toUpdateDTO, bool IsForQuestion)
        {
            var relatedRecords = All(false).Where(i =>
                i.JobScoringFactorId == toUpdateDTO.JobScoringFactorId
                && i.OrganizationJobId == toUpdateDTO.OrganizationJobId).ToList();

            var targetRecord = relatedRecords.FirstOrDefault(i =>
                i.ComplexityId == toUpdateDTO.ComplexityId
                && i.AbundanceId == toUpdateDTO.AbundanceId);

            if (targetRecord == null)
            {
                return OperationResult.NotFound("تنظیمات امتیاز انتخابی یافت نشد از تب اول ابتدا امتیاز مورد نظر را ثبت بفرمایید");
            }

            _unitOfWork.CreateTransaction();
            try
            {
                if (IsForQuestion)
                {
                    ApplyQuestionTabSelection(
                        relatedRecords,
                        toUpdateDTO.ComplexityId,
                        toUpdateDTO.AbundanceId);
                }
                else
                {
                    ApplyTableTabSelection(
                        relatedRecords,
                        toUpdateDTO.ComplexityId,
                        toUpdateDTO.AbundanceId);
                }

                if (_unitOfWork.Save().Result <= 0)
                {
                    _unitOfWork.Rollback();
                    return OperationResult.Failed("خطا در ذخیره انتخاب");
                }

                _unitOfWork.Commit();
                return OperationResult.Succeeded(payload: _mapper.Map<JobScoreAbundanceComplexityDTO>(targetRecord));
            }
            catch (Exception)
            {
                _unitOfWork.Rollback();
                throw;
            }
        }
        public OperationResult GetJobScoreRange(long organisationJobId, bool ignoreExpired = true)
        {

            var summationResult = GetJobScoreSummation(organisationJobId, ignoreExpired);
            if (!summationResult.Success)
            {
                return summationResult;
            }

            int totalScore = summationResult.Payload is int scoreValue
                ? scoreValue
                : Convert.ToInt32(summationResult.Payload);
            var matchedRecords = _g20ScoreDomainJobDegreeService.All()
                .AsEnumerable()
                .Where(i => i.LowerLimit <= totalScore && totalScore <= i.UpperLimit)
                .ToList();

            if (matchedRecords.Count > 1)
            {
                return OperationResult.NotFound("بیش از یک ردیف تنظیمات دامنه امتیازی یافت شد");
            }

            if (matchedRecords.Count == 0)
            {
                return OperationResult.Succeeded(payload: new
                {
                    TotalScore = totalScore,
                    JobDegree = (int?)null,
                    LowerLimit = (int?)null,
                    UpperLimit = (int?)null,
                });
            }

            return OperationResult.Succeeded(payload: new
            {
                TotalScore = totalScore,
                JobDegree = matchedRecords[0].JobDegree,
                LowerLimit = matchedRecords[0].LowerLimit,
                UpperLimit = matchedRecords[0].UpperLimit,
            });

        }
        public OperationResult GetRowForQuestionScoreDisplay(long OrganizationJobId, long JobScoringFactorId, long ComplexityId, long AbundanceId)
        {
            var ret = All().Where(i => i.OrganizationJobId == OrganizationJobId
              && i.JobScoringFactorId == JobScoringFactorId
              && i.ComplexityId == ComplexityId
              && i.AbundanceId == AbundanceId
                                 ).ToList();
            if (ret.Count() > 0)
            {
                return OperationResult.Succeeded(payload: ret.Single());
            }
            else { return OperationResult.NotFound(); }
        }
        public OperationResult Get(long id, long id1)
        {
            try
            {
                var ret = All()
                    .Include(i => i.Complexity)
                    .Include(i => i.Abundance)
                    .Include(i => i.JobScoringFactor)
                    .Include(i => i.OrganizationJob.Job)
                    .Where(i => i.OrganizationJobId == id && i.JobScoringFactorId == id1).ToList();

                var complexList = _complexityService.All().ToList();

                foreach (var complex in complexList)
                {

                    if (ret.Any(i => i.ComplexityId == complex.Id))
                    {
                        continue;
                    }
                    else
                    {
                        ret.Add(new JobScoreAbundanceComplexity()
                        {
                            ComplexityId = complex.Id,
                            Complexity = new Complexity()
                            {
                                Level = complex.Level,
                            }
                        });
                    }


                }

                return OperationResult.Succeeded(payload: _mapper.Map<List<JobScoreAbundanceComplexityDTO>>(ret).OrderBy(i => i.ComplexityLevel));
            }
            catch (Exception ex)
            {

                throw;
            }

        }
        public bool Validate(JobScoreAbundanceComplexity entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
