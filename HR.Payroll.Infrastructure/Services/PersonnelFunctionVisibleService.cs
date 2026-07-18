using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services
{
    public class PersonnelFunctionVisibleService : BaseService<PersonnelFunctionVisible, PayrollContext, PersonnelFunctionVisibleDTO>, IScopedServices
    {
        public PersonnelFunctionVisibleService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
            : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {
        }

        public OperationResult GetVisibilitySettings()
        {
            try
            {
                // Find the visibility settings for the current organization
                var visibilityRecord = _unitOfWork.Context.PersonnelFunctionVisibles
                    .FirstOrDefault(x => x.OrganisationChartId == _currentUserDefaultOrganId);

                if (visibilityRecord == null)
                {
                    // If no record exists, create a default one with all fields visible
                    var defaultRecord = new PersonnelFunctionVisible
                    {
                        OrganisationChartId = _currentUserDefaultOrganId,
                        FunctionDay = true,
                        PersonnelFunctionDay = true,
                        PersonnelHourPresent = true,
                        PersonnelNoEnter = true,
                        PersonnelAbsenceDay = true,
                        PersonnelIllnessDay = true,
                        PersonnelMissionHours = true,
                        PersonnelOverTime = true,
                        PersonnelOverTimeMinutes = true,
                        PersonnelNightWork = true,
                        PersonnelWorkingHolidayHours = true,
                        Year = true,
                        Month = true,
                        RemoteWorkHours = true,
                        IsConfirmed = true,
                        RealFunctionDay = true,
                        HolidayFunctionDay = true,
                        PersonnelMissionDay = true,
                        PaylessDay = true,
                        PaylessHour = true,
                        ShiftWork10Percent = true,
                        ShiftWork15Percent = true,
                        ShiftWork22Point5Percent = true,
                        RewardsDay = true,
                        PostType = true,
                        DeservedFunctionInHoliday = true,
                        DeservedFunctionOutHoliday = true,
                        PersonnelNightWorkDay = true,
                        PersonnelWorkingHolidaysDay = true,
                        LinearFunctionDay = true,
                        IsModir = true,
                        PersonnelCeillingOvertime = true,
                        PersonnelOverTimeFixed = true,
                        CarServiceDeduction = true,
                        AttendanceId = true,
                        ShiftWorkAllowance = true,
                        ShiftCount = true,
                        Food = true,
                        AccordReward = true,
                        Reward = true,
                        Arear1 = true,
                        Arear2 = true,
                        Description = true,
                        PersonnelHourlyWork = true,
                        PersonnelHourlyWorkMinutes = true,
                        PaylessMinutes = true,
                        Karaneh = true,
                        PersonnelNightWorkMinutes = true,
                        BasijOverTime = true,
                        TravelExpenses = true,
                        MissionExpenses = true,
                        RequestForAdditionalInsuranceForEntry = true,
                        MaximumAmountOfAllowancePayable = true,
                        OtherDeductions = true,
                        HekmatDeductions = true,
                        PersonnelWorkingHolidayMinutes = true,
                        DebtToTheCompany = true,
                        LastMonthDemand = true,
                        BonusCeiling = true,
                        IndividualBonusCeiling = true,
                        OtherBenefits = true,
                        OvertimePerCapita = true,
                        DisciplinaryOvertime = true,
                        ApprovedOvertimeHours = true,
                        OvertimeOutsideUnit = true,
                        ServiceRight = true,
                        ShiftReplacementOvertime = true,
                        CashOvertime = true,
                        TotalOvertime = true,
                        EfficiencyAndBonusRight = true,
                        MissionAndShift = true,
                        OtherPaymentsAndDeductions = true,
                        Efficiency100Percent = true,
                        ApprovedEfficiency = true,
                        ApprovedEfficiencyReserve = true,
                        FridayWorkHours = true,
                        FridayWorkAllowance = true,
                        NightWorkAllowance = true
                    };

                    _unitOfWork.Context.PersonnelFunctionVisibles.Add(defaultRecord);
                    _unitOfWork.Context.SaveChanges();
                    visibilityRecord = defaultRecord;
                }

                var dto = _mapper.Map<PersonnelFunctionVisibleDTO>(visibilityRecord);
                return OperationResult.Succeeded("تنظیمات نمایش‌پذیری با موفقیت واکشی شد", dto);
            }
            catch (Exception ex)
            {
                return OperationResult.Failed(ex.Message);
            }
        }
    }
}


