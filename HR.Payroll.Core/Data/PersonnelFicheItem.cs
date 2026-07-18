using Hr.SystemSetting.Core.Entities;
using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HR.SharedKernel.Share.Enums;

namespace HR.Payroll.Core.Data
{
    [Table("Personnel_FicheItem", Schema = "Payroll")]
    public class PersonnelFicheItem : SharedKernel.Data.BaseEntity, IOrganisationChartId
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }
        [ForeignKey("WageItem")]
        [IsEffectiveInDateOverLapChecking(IsEffective = true)]
        public long WageItemId { get; set; }
        public virtual WageItem? WageItem { get; set; }
        [ForeignKey("PersonnelPayment")]
        public long? PersonnelPaymentId { get; set; }
        public virtual PersonnelPayment? PersonnelPayment { get; set; }
        public long PaymentIntervalId { get; set; }
        public virtual BaseTableValue? PaymentInterval { get; set; }
        public Nullable<int> Value { get; set; }
        public bool DeductAtOnce { get; set; }
        [ForeignKey("OrganisationCheckFormula")]
        public long? OrganisationCheckFormulaId { get; set; }
        public virtual OrganisationFormula? OrganisationCheckFormula { get; set; }
        [ForeignKey("OrganisationFormula")]
        public long? OrganisationFormulaId { get; set; }
        public virtual OrganisationFormula? OrganisationFormula { get; set; }

        public virtual BaseTableValue? EnterType { get; set; }
        public long EnterTypeId { get; set; }
        //[StringLength(512)]
        //public string? Comment { get; set; }
        //public int? IsContinuous { get; set; }
        //public bool? IsFixed { get; set; }
        [NotMapped]
        private new string title { get; set; }
    }
}
