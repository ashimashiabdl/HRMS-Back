namespace HR.Payroll.Core.DTOs
{
    public class FunctionBulkCartableDTO
    {
        public long EmployeeId { get; set; }
        public long? PersonnelFunctionId { get; set; } // ID from Personnel_Function table
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? FatherName { get; set; }
        public string? IdentityNo { get; set; }
        public string? NationalNo { get; set; }
        public string? PersonelCode { get; set; }
        public long? PayLocationId { get; set; }
        public string? PayLocation { get; set; }
        public long? EmployeeTypeId { get; set; }
        public string? EmployeeType { get; set; }
        public long? EmployeeStatusId { get; set; }
        public string? EmployeeStatus { get; set; }
        public long? CostCenterId { get; set; }
        public string? CostCenter { get; set; }
        public long? WorkPlaceId { get; set; }
        public string? WorkPlace { get; set; }
        public long? OrganizationUnitId { get; set; }
        public string? OrganizationUnit { get; set; }
        public bool? IsEmployed { get; set; }
        public long? EducationGradeId { get; set; }
        public string? EducationGrade { get; set; }
        public long? JobId { get; set; }
        public string? Job { get; set; }
        public long? JobNatureId { get; set; }
        public string? JobNature { get; set; }
        public string? JobCode { get; set; }
        public int? JobDegree { get; set; }
        public long? GenderId { get; set; }
        public string? Gender { get; set; }
        public long? MaritalStatusId { get; set; }
        public string? MaritalStatus { get; set; }
        public long? ProcessAreaId { get; set; }
        public string? ProcessArea { get; set; }
        public decimal? ProcessAreaValue { get; set; }
        public string? ProcessDescription { get; set; }
        
        // PersonnelFunction fields
        public long? ArearsStatusId { get; set; }
        public long OrganisationChartId { get; set; }
        public long? PersonnelFunctionExcelFileId { get; set; }
        public int? FunctionDay { get; set; }
        public int? PersonnelFunctionDay { get; set; }
        public int? PersonnelHourPresent { get; set; }
        public decimal? PersonnelNoEnter { get; set; }
        public decimal? PersonnelAbsenceDay { get; set; }
        public decimal? PersonnelIllnessDay { get; set; }
        public decimal? PersonnelMissionHours { get; set; }
        public decimal? PersonnelOverTime { get; set; }
        public decimal? PersonnelOverTimeMinutes { get; set; }
        public decimal? PersonnelNightWork { get; set; }
        public decimal? PersonnelWorkingHolidayHours { get; set; }
        public int? Year { get; set; }
        public int? Month { get; set; }
        public int? RemoteWorkHours { get; set; }
        public bool? IsConfirmed { get; set; }
        public string? PayRollApproveUser { get; set; }
        public DateTime? PayRollAproveDate { get; set; }
        public decimal? RealFunctionDay { get; set; }
        public decimal? HolidayFunctionDay { get; set; }
        public long? FunctionTypeId { get; set; }
        public int? PersonnelMissionDay { get; set; }
        public int? PaylessDay { get; set; }
        public int? PaylessHour { get; set; }
        public int? ShiftWork10Percent { get; set; }
        public int? ShiftWork15Percent { get; set; }
        public int? ShiftWork22Point5Percent { get; set; }
        public int? RewardsDay { get; set; }
        public int? PostType { get; set; }
        public int? DeservedFunctionInHoliday { get; set; }
        public int? DeservedFunctionOutHoliday { get; set; }
        public decimal? PersonnelNightWorkDay { get; set; }
        public float? PersonnelWorkingHolidaysDay { get; set; }
        public long? LinearFunctionDay { get; set; }
        public string? Comment { get; set; }
        public DateTime? ConfirmDate { get; set; }
        public bool? IsModir { get; set; }
        public decimal? PersonnelCeillingOvertime { get; set; }
        public decimal? PersonnelOverTimeFixed { get; set; }
        public long? CarServiceDeduction { get; set; }
        public long? AttendanceId { get; set; }
        public DateTime? ReceiveDate { get; set; }
        public string? Description { get; set; }
        public decimal? AccordReward { get; set; }
        public decimal? Arear1 { get; set; }
        public decimal? Arear2 { get; set; }
        public decimal? Food { get; set; }
        public decimal? Reward { get; set; }
        public int? ShiftCount { get; set; }
        public decimal? ShiftWorkAllowance { get; set; }
        public decimal? BasijOverTime { get; set; }
        public decimal? Karaneh { get; set; }
        public int? PaylessMinutes { get; set; }
        public decimal? PersonnelHourlyWork { get; set; }
        public decimal? PersonnelHourlyWorkMinutes { get; set; }
        public decimal? PersonnelNightWorkMinutes { get; set; }
        public decimal? BonusCeiling { get; set; }
        public decimal? DebtToTheCompany { get; set; }
        public decimal? HekmatDeductions { get; set; }
        public decimal? IndividualBonusCeiling { get; set; }
        public decimal? LastMonthDemand { get; set; }
        public decimal? MaximumAmountOfAllowancePayable { get; set; }
        public decimal? MissionExpenses { get; set; }
        public decimal? OtherBenefits { get; set; }
        public decimal? OtherDeductions { get; set; }
        public decimal? PersonnelWorkingHolidayMinutes { get; set; }
        public decimal? RequestForAdditionalInsuranceForEntry { get; set; }
        public decimal? TravelExpenses { get; set; }
        public long? CreatedBy { get; set; }
        public long? LastModifiedBy { get; set; }
        public decimal? ApprovedEfficiency { get; set; }
        public decimal? ApprovedEfficiencyReserve { get; set; }
        public decimal? ApprovedOvertimeHours { get; set; }
        public decimal? CashOvertime { get; set; }
        public decimal? DisciplinaryOvertime { get; set; }
        public decimal? Efficiency100Percent { get; set; }
        public decimal? EfficiencyAndBonusRight { get; set; }
        public decimal? MissionAndShift { get; set; }
        public decimal? OtherPaymentsAndDeductions { get; set; }
        public decimal? OvertimeOutsideUnit { get; set; }
        public decimal? OvertimePerCapita { get; set; }
        public decimal? ServiceRight { get; set; }
        public decimal? ShiftReplacementOvertime { get; set; }
        public decimal? TotalOvertime { get; set; }
        /// <summary>
        /// جمعه کاری
        /// </summary>
        public decimal? FridayWorkHours { get; set; }
        /// <summary>
        /// فوق العاده جمعه کاری
        /// </summary>
        public decimal? FridayWorkAllowance { get; set; }
        /// <summary>
        /// فوق العاده شبکاری
        /// </summary>
        public decimal? NightWorkAllowance { get; set; }
        
        // TotalCount for pagination (returned from SP)
        public int? TotalCount { get; set; }
    }
}
