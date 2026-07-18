using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Insurance_Diskette_Item", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("CostCenterId", Name = "IX_Insurance_Diskette_Item_CostCenterId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Insurance_Diskette_Item_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("FicheId", Name = "IX_Insurance_Diskette_Item_FicheId")]
[Microsoft.EntityFrameworkCore.Index("InsuranceDisketteId", Name = "IX_Insurance_Diskette_Item_InsuranceDisketteId")]
[Microsoft.EntityFrameworkCore.Index("InterdictOrderId", Name = "IX_Insurance_Diskette_Item_InterdictOrderId")]
[Microsoft.EntityFrameworkCore.Index("PersonnelFunctionId", Name = "IX_Insurance_Diskette_Item_PersonnelFunctionId")]
public partial class InsuranceDisketteItem
{
    [Key]
    public long Id { get; set; }

    public long InsuranceDisketteId { get; set; }

    public long EmployeeId { get; set; }

    public long PersonnelFunctionId { get; set; }

    public long InterdictOrderId { get; set; }

    public long FicheId { get; set; }

    public long? DailyPayment { get; set; }

    public long? MonthPayment { get; set; }

    public long? PaymentInsuranceCoveredNotInMonthPayment { get; set; }

    public long? PaymentInsuranceCovered { get; set; }

    public long? TotalInsurancePayment { get; set; }

    public long? PersonnelInsuranceAmount { get; set; }

    public long? EmployerInsuranceAmount { get; set; }

    public long? UnEmployedInsuranceAmount { get; set; }

    [StringLength(128)]
    public string? InsuranceNo { get; set; }

    [StringLength(128)]
    public string? InsuranceWorkShopDisc { get; set; }

    [StringLength(128)]
    public string? PaymanRowDisc { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    [Column("DSW_BDATE")]
    [StringLength(8)]
    public string? DswBdate { get; set; }

    [Column("DSW_BIME")]
    public long DswBime { get; set; }

    [Column("DSW_DD")]
    public long DswDd { get; set; }

    [Column("DSW_DNAME")]
    [StringLength(100)]
    public string? DswDname { get; set; }

    [Column("DSW_EDATE")]
    [StringLength(8)]
    public string? DswEdate { get; set; }

    [Column("DSW_FNAME")]
    [StringLength(100)]
    public string? DswFname { get; set; }

    [Column("DSW_ID")]
    [StringLength(10)]
    public string? DswId { get; set; }

    [Column("DSW_ID1")]
    [StringLength(12)]
    public string? DswId1 { get; set; }

    [Column("DSW_IDATE")]
    [StringLength(8)]
    public string? DswIdate { get; set; }

    [Column("DSW_IDNO")]
    [StringLength(15)]
    public string? DswIdno { get; set; }

    [Column("DSW_IDPLC")]
    [StringLength(100)]
    public string? DswIdplc { get; set; }

    [Column("DSW_JOB")]
    [StringLength(6)]
    public string? DswJob { get; set; }

    [Column("DSW_LISTNO")]
    [StringLength(12)]
    public string? DswListno { get; set; }

    [Column("DSW_LNAME")]
    [StringLength(100)]
    public string? DswLname { get; set; }

    [Column("DSW_MAH")]
    public long DswMah { get; set; }

    [Column("DSW_MASH")]
    public long DswMash { get; set; }

    [Column("DSW_MAZ")]
    public long DswMaz { get; set; }

    [Column("DSW_MM")]
    public long DswMm { get; set; }

    [Column("DSW_NAT")]
    [StringLength(10)]
    public string? DswNat { get; set; }

    [Column("PER_NATCOD")]
    [StringLength(10)]
    public string? PerNatcod { get; set; }

    [Column("DSW_OCP")]
    [StringLength(100)]
    public string? DswOcp { get; set; }

    [Column("DSW_PRATE")]
    public long DswPrate { get; set; }

    [Column("DSW_ROOZ")]
    public long DswRooz { get; set; }

    [Column("DSW_SDATE")]
    [StringLength(8)]
    public string? DswSdate { get; set; }

    [Column("DSW_SEX")]
    [StringLength(3)]
    public string? DswSex { get; set; }

    [Column("DSW_TOTL")]
    public long DswTotl { get; set; }

    [Column("DSW_YY")]
    public long DswYy { get; set; }

    public long CostCenterId { get; set; }

    [Column("DSW_INC")]
    public long DswInc { get; set; }

    [Column("DSW_SPOUSE")]
    public long DswSpouse { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("CostCenterId")]
    [InverseProperty("InsuranceDisketteItems")]
    public virtual OrganisationChart CostCenter { get; set; } = null!;

    [ForeignKey("EmployeeId")]
    [InverseProperty("InsuranceDisketteItems")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("InsuranceDisketteId")]
    [InverseProperty("InsuranceDisketteItems")]
    public virtual InsuranceDiskette InsuranceDiskette { get; set; } = null!;

    [ForeignKey("InterdictOrderId")]
    [InverseProperty("InsuranceDisketteItems")]
    public virtual InterdictOrder InterdictOrder { get; set; } = null!;
}
