using Connection.models;
using Connection.models.Entites;
using Connection.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Connection.Data
{
    public class SaasDashboardContext:DbContext
    {
        public int _TenantID { get; set; }
        public virtual DbSet<Person> Persons { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Tenant>Tenants { get; set; } = null!;
        public virtual DbSet<PlatformPlan> PlatformPlans { get; set; } = null!;
        public virtual DbSet<PlatformPayment> PlatformPayments { get; set; } = null!;
        public virtual DbSet<PlatformSubscription> PlatformSubscriptions { get; set; } = null!;
        public virtual DbSet<TenantPlan> TenantPlans { get; set; } = null!;
        public virtual DbSet<UserPayment> UserPayments { get; set; } = null!;
        public virtual DbSet<UserSubscription> UserSubscriptions { get; set; } = null!;
        public virtual DbSet<TenantSession> Sessions { get; set; } = null!;
        public virtual DbSet<Email> Emails { get; set; } = null!;

        public SaasDashboardContext(DbContextOptions<SaasDashboardContext> options, ITenantIdProvider dataKeyProvider) :base(options) {

                _TenantID = dataKeyProvider.TenantId;

        }
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                if (typeof(IEntityWithTenantId).IsAssignableFrom(entityType.ClrType))
                {
                    var method = typeof(SaasDashboardContext)
                        .GetMethod(nameof(SetGlobalFilter),
                        BindingFlags.NonPublic | BindingFlags.Instance)
                        ?.MakeGenericMethod(entityType.ClrType);

                    method?.Invoke(this, new object[] { modelBuilder });
                }
            }
        }

        private void SetGlobalFilter<TEntity>(ModelBuilder builder)
      where TEntity : class, IEntityWithTenantId
        {
            builder.Entity<TEntity>()
                .HasQueryFilter(e => e.TenantId == _TenantID);
        }
    }
}
