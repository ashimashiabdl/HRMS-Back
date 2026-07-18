using HR.BaseInfo.Core.Entities;
using HR.Identity.Core.Entities;
using HR.Organisation.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace HR.Payroll.Core.Data;

[Table("Batch_PayRoll_Request", Schema = "Payroll")]
public class BatchPayRollRequest : BaseEntity, IignoreDateRangeValidation
{
    [ForeignKey("Employee")]
    public long UserId { get; set; }
    public virtual AspNetUsers? User { get; set; }

    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    public long RequestStateId { get; set; }
    public long RequestTypeId { get; set; }

    [ForeignKey("PaymentPeriod")]
    public long? PaymentPeriodId { get; set; }
    public virtual PaymentPeriod? PaymentPeriod { get; set; }     
    
    [ForeignKey("BankDiskette")]
    public long? BankDisketteId { get; set; }
    public virtual BankDiskette? BankDiskette { get; set; }  
    
    [ForeignKey("TaxDiskette")]
    public long? TaxDisketteId { get; set; }
    public virtual TaxDiskette? TaxDiskette { get; set; }  
    
    [ForeignKey("InsuranceDiskette")]
    public long? InsuranceDisketteId { get; set; }
    public virtual InsuranceDiskette? InsuranceDiskette { get; set; }

    [StringLength(256)]
    public string? Username { get; set; }

    [StringLength(4096)]
    public string? RequsetDescription { get; set; }
    [Column(TypeName = "datetime")]
    public DateTime? LastPoolingTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FinishDateTime { get; set; }

    public bool IsDone { get; set; }
    public int EmployeeCount { get; set; }
    public int SuccessCount { get; set; }
    public long? PoolingEmployeeId { get; set; }

    /// <summary>
    ///    Exeption    
    /// </summary>
    public string? ExeptionMessage { get; set; }

    [NotMapped]
    private new string title { get; set; }
}
