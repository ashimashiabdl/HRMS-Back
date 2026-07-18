using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Keyless]
[Table("wwwwww", Schema = "bas")]
public partial class Wwwwww
{
    [Column("کد_ملی")]
    public string? کدملی { get; set; }

    public string? سال { get; set; }

    public string? ماه { get; set; }

    [Column("روز_ماه")]
    public string? روزماه { get; set; }

    public string? کارکرد { get; set; }

    [Column("کسر_کار_ساعت")]
    public string? کسرکارساعت { get; set; }

    [Column("gddfdf")]
    public string? Gddfdf { get; set; }

    public string? دورکاری { get; set; }

    [Column("اضافه_کاری")]
    public string? اضافهکاری { get; set; }

    public string? استعلاجی { get; set; }

    public string? استحقاقی { get; set; }

    public string? ساعتی { get; set; }

    [Column("شرح___توضیحلات")]
    public string? شرحتوضیحلات { get; set; }

    [Column("تاریخ_نولد")]
    public string? تاریخنولد { get; set; }
}
