using HR.SharedKernel.Attribute;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Data;

namespace HR.Payroll.Core.DTOs
{
    public class BankDisketteDTO : BaseDTO
    {
        public long OrganisationChartId { get; set; }
        public string? OrganisationChart { get; set; }
   
        public long? PaymentPeriodId { get; set; }
        public string? PaymentPeriod { get; set; }
        public int AllPersonnelCount { get; set; }
        /// <summary>
        /// محاسبه دیسکت برای تمامی مراکز هزینه که در دوره جاری فیش دارند انجام شود
        /// </summary>
        public bool CalculateAllFichesInCurrentPeriod { get; set; }
        public long SumPaymentAmount { get; set; }
        /// <summary>
        /// شرح واریزی
        /// </summary>
        public string? DescriptionOfTheDeposit { get; set; }
        /// <summary>
        /// فهرست مراکز هزینه
        /// </summary>
        public List<long>? CostCenterIdList { get; set; }
        public long BankDisketteStatusId { get; set; }
        public long BatchPayRollRequestId { get; set; }
        public string? BankDisketteStatus { get; set; }
    }
}
