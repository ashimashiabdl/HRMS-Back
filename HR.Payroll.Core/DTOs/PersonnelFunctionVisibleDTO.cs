using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs
{
    public class PersonnelFunctionVisibleDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }

        public bool? FunctionDay { get; set; }
        public bool? PersonnelFunctionDay { get; set; }
        public bool? PersonnelHourPresent { get; set; }
        public bool? PersonnelNoEnter { get; set; }
        public bool? PersonnelAbsenceDay { get; set; }
        public bool? PersonnelIllnessDay { get; set; }
        public bool? PersonnelMissionHours { get; set; }
        public bool? PersonnelOverTime { get; set; }
        public bool? PersonnelOverTimeMinutes { get; set; }
        public bool? PersonnelNightWork { get; set; }
        public bool? PersonnelWorkingHolidayHours { get; set; }
        public bool? Year { get; set; }
        public bool? Month { get; set; }
        public bool? RemoteWorkHours { get; set; }
        public bool? IsConfirmed { get; set; }
        public bool? RealFunctionDay { get; set; }
        public bool? HolidayFunctionDay { get; set; }
        public bool? PersonnelMissionDay { get; set; }
        public bool? PaylessDay { get; set; }
        public bool? PaylessHour { get; set; }
        public bool? ShiftWork10Percent { get; set; }
        public bool? ShiftWork15Percent { get; set; }
        public bool? ShiftWork22Point5Percent { get; set; }
        public bool? RewardsDay { get; set; }
        public bool? PostType { get; set; }
        public bool? DeservedFunctionInHoliday { get; set; }
        public bool? DeservedFunctionOutHoliday { get; set; }
        public bool? PersonnelNightWorkDay { get; set; }
        public bool? PersonnelWorkingHolidaysDay { get; set; }
        public bool? LinearFunctionDay { get; set; }
        public bool? IsModir { get; set; }
        public bool? PersonnelCeillingOvertime { get; set; }
        public bool? PersonnelOverTimeFixed { get; set; }
        public bool? CarServiceDeduction { get; set; }
        public bool? AttendanceId { get; set; }
        public bool? ShiftWorkAllowance { get; set; }
        public bool? ShiftCount { get; set; }
        public bool? Food { get; set; }
        public bool? AccordReward { get; set; }
        public bool? Reward { get; set; }
        public bool? Arear1 { get; set; }
        public bool? Arear2 { get; set; }
        public bool? Description { get; set; }
        // New visibility flags aligned with PersonnelFunction
        public bool? PersonnelHourlyWork { get; set; }
        public bool? PersonnelHourlyWorkMinutes { get; set; }
        public bool? PaylessMinutes { get; set; }
        public bool? Karaneh { get; set; }
        public bool? PersonnelNightWorkMinutes { get; set; }
        public bool? BasijOverTime { get; set; }
        
        // Additional new fields
        public bool? TravelExpenses { get; set; }
        public bool? MissionExpenses { get; set; }
        public bool? RequestForAdditionalInsuranceForEntry { get; set; }
        public bool? MaximumAmountOfAllowancePayable { get; set; }
        public bool? OtherDeductions { get; set; }
        public bool? HekmatDeductions { get; set; }
        public bool? PersonnelWorkingHolidayMinutes { get; set; }
        public bool? DebtToTheCompany { get; set; }
        public bool? LastMonthDemand { get; set; }
        public bool? BonusCeiling { get; set; }
        public bool? IndividualBonusCeiling { get; set; }
        public bool? OtherBenefits { get; set; }
        public bool? OvertimePerCapita { get; set; }
        public bool? DisciplinaryOvertime { get; set; }
        public bool? ApprovedOvertimeHours { get; set; }
        public bool? OvertimeOutsideUnit { get; set; }
        public bool? ServiceRight { get; set; }
        public bool? ShiftReplacementOvertime { get; set; }
        public bool? CashOvertime { get; set; }
        public bool? TotalOvertime { get; set; }
        public bool? EfficiencyAndBonusRight { get; set; }
        public bool? MissionAndShift { get; set; }
        public bool? OtherPaymentsAndDeductions { get; set; }
        public bool? Efficiency100Percent { get; set; }
        public bool? ApprovedEfficiency { get; set; }
        public bool? ApprovedEfficiencyReserve { get; set; }
        /// <summary>
        /// جمعه کاری
        /// </summary>
        public bool? FridayWorkHours { get; set; }
        /// <summary>
        /// فوق العاده جمعه کاری
        /// </summary>
        public bool? FridayWorkAllowance { get; set; }
        /// <summary>
        /// فوق العاده شبکاری
        /// </summary>
        public bool? NightWorkAllowance { get; set; }
    }
}


