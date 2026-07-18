using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("_EFMigrationsHistroy", Schema = "emp")]
public partial class EfmigrationsHistroy2
{
    [Key]
    [StringLength(150)]
    public string MigrationId { get; set; } = null!;

    [StringLength(32)]
    public string ProductVersion { get; set; } = null!;
}
