using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Core.Data
{
    [Table("Fiche_PDF_archive", Schema = "Payroll")]
    public class FichePdfArchive : BaseEntity
    {
        [ForeignKey("Fiche")]
        public long FicheId { get; set; }
        public virtual Fiche? Fiche { get; set; }
        public byte[]? PdfbyteArray { get; set; }
        [ForeignKey("PaymentPeriod")]
        public long PaymentPeriodId { get; set; }
        public virtual PaymentPeriod? PaymentPeriod { get; set; }

        [ForeignKey("Employee")]
        public long EmployeeId { get; set; }
        public virtual HR.Employee.Core.Entities.Employee? Employee { get; set; }

        public byte[]? FichebinaryWithEmployer { get; set; }

        public int FicheTypeId { get; set; }

    }
}
