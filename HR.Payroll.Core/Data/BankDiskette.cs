using HR.BaseInfo.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.Payroll.Core.Data
{
    [Table("Bank_Diskette", Schema = "Payroll")]
    public class BankDiskette : BaseEntity, IOrganisationChartId , IignoreDateRangeValidation
    {
        [ForeignKey("OrganisationChart")]
        public long OrganisationChartId { get; set; }
        public virtual OrganisationChart? OrganisationChart { get; set; }
        /// <summary>
        /// ������ ��Ә� ���� ����� ��ǘ� ����� �� �� ���� ���� ��� ����� ����� ���
        /// </summary>
        [Comment("������ ��Ә� ���� ����� ��ǘ� ����� �� �� ���� ���� ��� ����� ����� ���")]
        public bool CalculateAllFichesInCurrentPeriod { get; set; }

        [ForeignKey("PaymentPeriod")]
        public long PaymentPeriodId { get; set; }
        public virtual PaymentPeriod? PaymentPeriod { get; set; }
        public long BankDisketteStatusId { get; set; }
        public virtual BaseTableValue? BankDisketteStatus { get; set; }
        [ForeignKey("BatchPayRollRequest")]
        public long? BatchPayRollRequestId { get; set; }
        public virtual BatchPayRollRequest? BatchPayRollRequest { get; set; }
        public int AllPersonnelCount { get; set; }
        public long SumPaymentAmount { get; set; }
       
        /// <summary>
        /// �� ��� ����
        /// </summary>
        [StringLength(128)]
        public string? CodeList { get; set; }

        /// <summary>
        /// شرح واریزی
        /// </summary>
        public string? DescriptionOfTheDeposit { get; set; }
        [NotMapped]
        private new string title { get; set; }
        // [ForeignKey("EmployeeStatus")]
        // public long EmployeeStatusId { get; set; }
        // public virtual EmployeeStatus? EmployeeStatus { get; set; }
    }
}
