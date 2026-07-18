using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.DTOs
{
    public class OrganisationEmployeeTypeOrderTypeSummaryCalcDTO : BaseDTO
    {
        public long EmployeeTypeId { get; set; }
        public string? EmployeeTypeTitle { get; set; }
        public long OrderTypeId { get; set; }
        public string? OrderTypeTitle { get; set; }
        public bool? CalExperienceRecorded { get; set; }
        public bool? CalRetiredRecorded { get; set; }
        public bool? CalYearRecorded { get; set; }
        public bool? CalHistoryOut { get; set; }
        public bool? CalHistoryStop { get; set; }
        public bool? CalretiredFlagOK { get; set; }
        public bool? CalMarriageStatus { get; set; }
        public bool? CalSponsorshipCount { get; set; }
        public bool? CalEducationInfo { get; set; }
        public bool? CalCapivityInfo { get; set; }
        public bool? CalIsarInfo { get; set; }
        public bool? CalBasijInfo { get; set; }
        public bool? CalWarInfo { get; set; }
        public bool? CalPersonnelInfo { get; set; }
        public bool? CalIsWomenHeadInfo { get; set; }
        public bool? CalOtherVeteransInfo { get; set; }
        public bool? CalInsuranceTypeInfo { get; set; }
        public bool? CalDrivingLicenseNumberInfo { get; set; }
        public bool? CalEmployeeDate { get; set; }
        public bool? CalMartyrStatus { get; set; }
        public bool? CalFamilyInfo { get; set; }
        public bool? CalChildCount { get; set; }

        
        public long? CalExperienceRecordedEntertypeId { get; set; }
        public string? CalExperienceRecordedEntertype { get; set; }
        public long? CalRetiredRecordedEntertypeId { get; set; }
        public string? CalRetiredRecordedEntertype { get; set; }
        
        public long? CalYearRecordedEntertypeId { get; set; }
        public string? CalYearRecordedEntertype { get; set; }


    }
}
