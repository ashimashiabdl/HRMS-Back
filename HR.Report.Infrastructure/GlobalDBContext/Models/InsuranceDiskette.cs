using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Insurance_Diskette", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BatchPayRollRequestId", Name = "IX_InsDisk_BatchReqId")]
[Microsoft.EntityFrameworkCore.Index("InsuranceBranchId", Name = "IX_InsDisk_InsuranceBranchId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_InsDisk_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_InsDisk_PaymentPeriodId")]
[Microsoft.EntityFrameworkCore.Index("PeymanRowId", Name = "IX_InsDisk_PeymanRowId")]
[Microsoft.EntityFrameworkCore.Index("ReportTypeId", Name = "IX_InsDisk_ReportTypeId")]
[Microsoft.EntityFrameworkCore.Index("InsuranceDisketteStatusId", Name = "IX_InsDisk_StatusId")]
[Microsoft.EntityFrameworkCore.Index("BatchPayRollRequestId", Name = "IX_Insurance_Diskette_BatchPayRollRequestId")]
[Microsoft.EntityFrameworkCore.Index("InsuranceBranchId", Name = "IX_Insurance_Diskette_InsuranceBranchId")]
[Microsoft.EntityFrameworkCore.Index("InsuranceDisketteStatusId", Name = "IX_Insurance_Diskette_InsuranceDisketteStatusId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Insurance_Diskette_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("PeymanRowId", Name = "IX_Insurance_Diskette_PeymanRowId")]
[Microsoft.EntityFrameworkCore.Index("ReportTypeId", Name = "IX_Insurance_Diskette_ReportTypeId")]
public partial class InsuranceDiskette
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

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

    public long InsuranceBranchId { get; set; }

    public long PaymentPeriodId { get; set; }

    public long? BatchPayRollRequestId { get; set; }

    public long InsuranceDisketteStatusId { get; set; }

    [Column("DSK_YY")]
    public int DskYy { get; set; }

    [Column("DSK_ADRS")]
    [StringLength(100)]
    public string? DskAdrs { get; set; }

    [Column("DSK_BIC")]
    public long DskBic { get; set; }

    [Column("DSK_BIMH")]
    public long DskBimh { get; set; }

    [Column("DSK_DISC")]
    [StringLength(100)]
    public string? DskDisc { get; set; }

    [Column("DSK_FARM")]
    [StringLength(100)]
    public string? DskFarm { get; set; }

    [Column("DSK_ID")]
    [StringLength(10)]
    public string? DskId { get; set; }

    [Column("DSK_KIND")]
    public int DskKind { get; set; }

    [Column("DSK_LISTNO")]
    [StringLength(12)]
    public string? DskListno { get; set; }

    [Column("DSK_MM")]
    public int DskMm { get; set; }

    [Column("DSK_NAME")]
    [StringLength(100)]
    public string? DskName { get; set; }

    [Column("DSK_NUM")]
    public int DskNum { get; set; }

    [Column("DSK_PRATE")]
    public long DskPrate { get; set; }

    [Column("MON_PYM")]
    [StringLength(100)]
    public string? MonPym { get; set; }

    [Column("DSK_RATE")]
    public int DskRate { get; set; }

    [Column("DSK_TBIME")]
    public long DskTbime { get; set; }

    [Column("DSK_TDD")]
    public long DskTdd { get; set; }

    [Column("DSK_TKOSO")]
    public long DskTkoso { get; set; }

    [Column("DSK_TMAH")]
    public long DskTmah { get; set; }

    [Column("DSK_TMASH")]
    public long DskTmash { get; set; }

    [Column("DSK_TMAZ")]
    public long DskTmaz { get; set; }

    [Column("DSK_TROOZ")]
    public long DskTrooz { get; set; }

    [Column("DSK_TTOTL")]
    public long DskTtotl { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    public string? ErrorMessage { get; set; }

    /// <summary>
    /// basetable value Id = 40282
    /// </summary>
    public long ReportTypeId { get; set; }

    /// <summary>
    /// ردیف پیمان متناظر
    /// </summary>
    public long PeymanRowId { get; set; }

    [Column("DSK_INC")]
    public long DskInc { get; set; }

    [Column("DSK_SPOUSE")]
    public long DskSpouse { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BatchPayRollRequestId")]
    [InverseProperty("InsuranceDiskettes")]
    public virtual BatchPayRollRequest? BatchPayRollRequest { get; set; }

    [InverseProperty("InsuranceDiskette")]
    public virtual ICollection<BatchPayRollRequest> BatchPayRollRequests { get; set; } = new List<BatchPayRollRequest>();

    [ForeignKey("InsuranceBranchId")]
    [InverseProperty("InsuranceDiskettes")]
    public virtual InsuranceBranch InsuranceBranch { get; set; } = null!;

    [InverseProperty("InsuranceDiskette")]
    public virtual ICollection<InsuranceDisketteCostCenter> InsuranceDisketteCostCenters { get; set; } = new List<InsuranceDisketteCostCenter>();

    [InverseProperty("InsuranceDiskette")]
    public virtual ICollection<InsuranceDisketteFile> InsuranceDisketteFiles { get; set; } = new List<InsuranceDisketteFile>();

    [InverseProperty("InsuranceDiskette")]
    public virtual ICollection<InsuranceDisketteItem> InsuranceDisketteItems { get; set; } = new List<InsuranceDisketteItem>();

    [ForeignKey("InsuranceDisketteStatusId")]
    [InverseProperty("InsuranceDisketteInsuranceDisketteStatuses")]
    public virtual BaseTableValue InsuranceDisketteStatus { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("InsuranceDiskettes")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("InsuranceDiskettes")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;

    [ForeignKey("PeymanRowId")]
    [InverseProperty("InsuranceDiskettes")]
    public virtual OrganisationPeymanRow PeymanRow { get; set; } = null!;

    [ForeignKey("ReportTypeId")]
    [InverseProperty("InsuranceDisketteReportTypes")]
    public virtual BaseTableValue ReportType { get; set; } = null!;
}
