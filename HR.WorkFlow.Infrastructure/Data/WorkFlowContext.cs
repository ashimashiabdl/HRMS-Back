using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HR.WorkFlow.Core;
using HR.WorkFlow.Core.Data;
using HR.SharedKernel.Service;

namespace HR.WorkFlow.Infrastructure.Data
{
    public class WorkFlowContext : BaseDbContext
    {
        public WorkFlowContext()
        {

        }
        public WorkFlowContext(DbContextOptions<WorkFlowContext> options, UserResolverService userService) : base(options, userService)
        {

        }
        public DbSet<Core.Data.WorkFlow> WorkFlows { get; set; }
        public DbSet<Core.Data.Node> Nodes { get; set; }
     //   public DbSet<Core.Data.Activity> Activities { get; set; }
        public DbSet<Core.Data.Action> Actions { get; set; }
        public DbSet<Core.Data.ActivityTemplate> ActivityTemplates { get; set; }
        public DbSet<UserSignature> UserSignatures { get; set; }
        public DbSet<Core.Data.Definition> Definitions { get; set; }
        public DbSet<Core.Data.NodeUserRel> NodeUserRels { get; set; }
        public DbSet<Core.Data.NodeRoleRel> NodeRoleRels { get; set; }
        public DbSet<Core.Data.WorkFlowInstance> WorkFlowInstances { get; set; }
        public DbSet<Core.Data.WorkFlowType> WorkFlowTypes { get; set; }
        public DbSet<HR.Order.Core.Data.InterdictOrder> InterdictOrders { get; set; }
        public DbSet<HR.Order.Core.Data.RecruitOrder> RecruitOrders { get; set; }
        public DbSet<HR.Payroll.Core.Data.EmployeeRelated.EmployeeSettlement> EmployeeSettlements { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var cascadeFKs = modelBuilder.Model.GetEntityTypes()
                  .SelectMany(t => t.GetForeignKeys())
                  .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

            foreach (var fk in cascadeFKs)
                fk.DeleteBehavior = DeleteBehavior.Restrict;
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
    }
}
