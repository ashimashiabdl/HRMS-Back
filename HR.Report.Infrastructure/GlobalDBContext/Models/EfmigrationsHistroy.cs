using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("_EFMigrationsHistroy", Schema = "bas")]
public partial class EfmigrationsHistroy
{
    [Key]
    [StringLength(150)]
    public string MigrationId { get; set; } = null!;

    [StringLength(32)]
    public string ProductVersion { get; set; } = null!;

    public long? Id { get; set; }
}
