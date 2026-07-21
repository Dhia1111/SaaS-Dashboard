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
using AppAny.Quartz.EntityFrameworkCore.Migrations;
using AppAny.Quartz.EntityFrameworkCore.Migrations.PostgreSQL;
using SharedDto_Enum;
namespace Connection.Data
{
    public class SaasDashboardContext:DbContext
    {
        public int _TenantID { get; set; }
        public virtual DbSet<Person> Persons { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<Tenant>Tenants { get; set; } = null!;
         public virtual DbSet<Payment> PlatformPayments { get; set; } = null!;
        public virtual DbSet<PlatformSubscription> PlatformSubscriptions { get; set; } = null!;
        public virtual DbSet<TenantPlan> TenantPlans { get; set; } = null!;
         public virtual DbSet<TenantSession> TenantsSessions { get; set; } = null!;
        public virtual DbSet<UserSession> UsersSessions { get; set; } = null!;
        public virtual DbSet<Email> Emails { get; set; } = null!;

        
        
        public virtual DbSet<TenantPlan> TenantsPlans { get; set; } = null!;
        public virtual DbSet<TenantPermission> TenantsPermissions { get; set; } = null!;
        public virtual DbSet<TenantPlanPermission> TenantsPlansPermissions { get; set; } = null!;
        public virtual DbSet<TenantPlanBenefit> TenantsPlansBenifests { get; set; } = null!;
        public virtual DbSet<TenantPlanPricingOption> TenantsPricingOptions { get; set; } = null!;
        public virtual DbSet<TenantPricingCycle> TenantsPricingCycles { get; set; } = null!;

        public virtual DbSet<PlatformAdmine> PlatformAdmines { get; set; } = null!;
        public virtual DbSet<Employee> Employees { get; set; } = null!;


        public virtual DbSet<TenantFreePlan> TenantsFreePlans { get; set; } = null!;
        public virtual DbSet<DiscoveryPlatform> DiscoveriesPlatforms { get; set; } = null!;

        public virtual DbSet<ClientSubscription>ClientSubscriptions { get; set; } = null!;
        public SaasDashboardContext(DbContextOptions<SaasDashboardContext> options, ITenantIdProvider dataKeyProvider) :base(options) {

                _TenantID = dataKeyProvider.TenantId;

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.AddQuartz(builder =>
                builder.UsePostgreSql()
                );


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

            //Constraint 
            modelBuilder.Entity<Tenant>()
       .HasIndex(u => u.Name)
       .IsUnique();

            modelBuilder.Entity<User>()
  .HasIndex(u => new { u.TenantId, u.Id })
  .IsUnique();
            //Constraint 
            modelBuilder.Entity<TenantPricingCycle>()
       .HasIndex(u => new { u.CycleName, u.TenantId })
       .IsUnique();
            modelBuilder.Entity<TenantPricingCycle>()
          .HasIndex(u => new { u.PeriodUnit, u.Period, u.TenantId })
          .IsUnique();
            //
            modelBuilder.Entity<TenantPlan>()
        .HasIndex(u => new { u.Name, u.TenantId })
        .IsUnique();
            // constraint 
            modelBuilder.Entity<Payment>()
.HasIndex(p => new { p.TenantId, p.ProviderPaymentId })
.IsUnique();

            modelBuilder.Entity<Payment>()
.HasIndex(p => new { p.TenantId })
.IsUnique().HasFilter($" \"PaymentStatus\" ={(int)enGeneralState.Pending} ");

            modelBuilder.Entity<PlatformSubscription>()
.HasIndex(p => new { p.TenantId })
.IsUnique().HasFilter($"\"IsActive\" = {true} ");

            // cascad relation ship for plan

            modelBuilder.Entity<TenantPlanPermission>()
      .HasOne(x => x.TenantPlan)
      .WithMany(x => x.Permissions)
      .HasForeignKey(x => x.TenantPlanId)
      .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TenantPlanBenefit>()
      .HasOne(x => x.TenantPlan)
      .WithMany(x => x.Benefits)
      .HasForeignKey(x => x.TenantPlanId)
      .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TenantPlanPricingOption>()
      .HasOne(x => x.TenantPlan)
      .WithMany(x => x.PricingOptions)
      .HasForeignKey(x => x.TenantPlanId)
      .OnDelete(DeleteBehavior.Cascade);
            // set restric delete constrain o

            modelBuilder.Entity<TenantPlanPermission>()
    .HasOne(x => x.Permission)
    .WithMany()
    .HasForeignKey(x => x.PermissionId)
    .OnDelete(DeleteBehavior.Restrict);

            //Delete consttraint on PlatformUser
            modelBuilder.Entity<User>()
.HasOne(x => x.Person)
.WithOne()
.HasForeignKey<User>(x => x.PersonId)
.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
.HasOne(x => x.User)
.WithOne()
.HasForeignKey<Employee>(x => x.UserId)
.OnDelete(DeleteBehavior.Cascade);



            //     
            modelBuilder.Entity<Tenant>()
.HasOne(x => x.Person)
.WithOne()
.HasForeignKey<Tenant>(x => x.PersonId)
.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlatformAdmine>()
.HasOne(x => x.Tenant)
.WithOne()
.HasForeignKey<PlatformAdmine>(x => x.TenantId)
.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PlatformSubscription>()
                .HasOne(x => x.TenantPlanPricingOption)
                .WithMany()
                .HasForeignKey(x => x.TenantPlanPricingOptionId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<TenantFreePlan>()
.HasOne(x => x.TenantPlan)
.WithOne(x => x.TenantFreePlan)
.HasForeignKey<TenantFreePlan>(x => x.TenantPlanId)
.OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<TenantFreePlan>()
.HasIndex(p => new { p.TenantId, p.TenantPlanId })
.IsUnique();

            //Constraint 
            modelBuilder.Entity<DiscoveryPlatform>()
       .HasIndex(u => new {u.TenantId,u.TenantClientIdentifier})
       .IsUnique();


        }


        private void SetGlobalFilter<TEntity>(ModelBuilder builder)
      where TEntity : class, IEntityWithTenantId
        {
            builder.Entity<TEntity>()
                .HasQueryFilter(e => e.TenantId == _TenantID);
        }
    }
}
