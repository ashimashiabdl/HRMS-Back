using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Core.Entities
{
    [Table("Organisation_EmployeeType_OrderType_SummaryCalc", Schema = "Setting")]
    public class OrganisationEmployeeTypeOrderTypeSummaryCalc : HR.SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("EmployeeType")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long EmployeeTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual EmployeeType? EmployeeType { get; set; }
        [ForeignKey("OrderType")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long OrderTypeId { get; set; }
        [IsEffectiveInGenericSearch(IsEffective = true)]
        public virtual OrderType? OrderType { get; set; }
        public bool CalExperienceRecorded { get; set; }
        public bool CalRetiredRecorded { get; set; }
        public bool CalYearRecorded { get; set; }public virtual BaseTableValue? CalExperienceRecordedEntertype { get; set; }public virtual BaseTableValue? CalRetiredRecordedEntertype { get; set; }public virtual BaseTableValue? CalYearRecordedEntertype { get; set; }



        public bool CalHistoryOut { get; set; }
        public bool CalHistoryStop { get; set; }
        public bool CalretiredFlagOK { get; set; }
        public bool CalMarriageStatus { get; set; }
        public bool CalSponsorshipCount { get; set; }
        public bool CalEducationInfo { get; set; }
        public bool CalCapivityInfo { get; set; }
        public bool CalIsarInfo { get; set; }
        public bool CalBasijInfo { get; set; }
        public bool CalWarInfo { get; set; }
        public bool CalPersonnelInfo { get; set; }
        public bool CalIsWomenHeadInfo { get; set; }
        public bool CalOtherVeteransInfo { get; set; }
        public bool CalInsuranceTypeInfo { get; set; }
        public bool CalDrivingLicenseNumberInfo { get; set; }
        public bool CalEmployeeDate { get; set; }
        public bool CalMartyrStatus { get; set; }
        public bool CalFamilyInfo { get; set; }
        public bool CalChildCount { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
