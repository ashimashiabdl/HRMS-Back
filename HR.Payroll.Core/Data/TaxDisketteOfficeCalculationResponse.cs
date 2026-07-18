using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data;
[Table("Tax_Diskette_Office_Calculation_Response", Schema = "Payroll")]
public class TaxDisketteOfficeCalculationResponse : BaseEntity , IignoreDateRangeValidation
{
    [ForeignKey("TaxDiskette")]
    public long TaxDisketteId { get; set; }
    public virtual TaxDiskette? TaxDiskette { get; set; }

    [ForeignKey("TaxDisketteFile")]
    public long TaxDisketteFiled { get; set; }
    public virtual TaxDisketteFile? TaxDisketteFile { get; set; }


    [ForeignKey("BatchPayRollRequest")]
    public long? BatchPayRollRequestId { get; set; }
    public virtual BatchPayRollRequest? BatchPayRollRequest { get; set; }

    /// <summary>
    /// توضیحات
    /// </summary>

    public string? Description { get; set; }
}
