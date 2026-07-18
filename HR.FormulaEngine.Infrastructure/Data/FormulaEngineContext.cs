using HR.FormulaEngine.Core.Data;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Infrastructure.Data;

public class FormulaEngineContext : BaseDbContext
{
    public FormulaEngineContext()
    {

    }
    public FormulaEngineContext(DbContextOptions<FormulaEngineContext> options, UserResolverService userService) : base(options, userService)
    {

    }
    public DbSet<FormulaDefinition> FormulaDefinitions { get; set; }
    public DbSet<FormulaDatabaseFunctionDefinition> FormulaDatabaseFunctionDefinitions { get; set; }
    public DbSet<FormulaTable> FormulaTables { get; set; }
    public DbSet<FormulaTableValue> FormulaTableValues { get; set; }
    public DbSet<FormulaOperand> FormulaOperands { get; set; }
    public DbSet<FormulaDefinitionHistory> FormulaDefinitionHistories { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
              .SelectMany(t => t.GetForeignKeys())
              .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        // Configure BaseTableValue relationships without FK constraints
        ConfigureBaseTableValueRelationships(modelBuilder);
    }

    private void ConfigureBaseTableValueRelationships(ModelBuilder modelBuilder)
    {
        // FormulaDatabaseFunctionDefinition entity relationships
        modelBuilder.Entity<FormulaDatabaseFunctionDefinition>()
            .Property<long?>("FuctionTypeId");
        modelBuilder.Entity<FormulaDatabaseFunctionDefinition>()
            .HasOne(f => f.FuctionType)
            .WithMany()
            .HasForeignKey("FuctionTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // FormulaTable entity relationships
        modelBuilder.Entity<FormulaTable>()
            .Property<long>("TableTypeId");
        modelBuilder.Entity<FormulaTable>()
            .HasOne(f => f.TableType)
            .WithMany()
            .HasForeignKey("TableTypeId")
            .OnDelete(DeleteBehavior.NoAction);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        

      
        //
       base.OnConfiguring(optionsBuilder);
    }
}
