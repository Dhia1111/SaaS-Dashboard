using Connection.models.Entites;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models
{
    public class TenantPlan : IEntityWithTenantId, IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]

        public string Name { get; set; } = null!;

        public string? Description { get; set; }


        [Required]

        public bool IsActive { get; set; }

        [Required]
        public bool HasFreeTryOption { get; set; } = false;
        /* Navigation */
        public Tenant? Tenant { get; set; }

        public ICollection<TenantPlanPermission> Permissions { get; set; }
            = new List<TenantPlanPermission>();

        public ICollection<TenantPlanBenefit> Benefits { get; set; }
            = new List<TenantPlanBenefit>();

        public ICollection<TenantPlanPricingOption> PricingOptions { get; set; }
            = new List<TenantPlanPricingOption>();
        public TenantFreePlan? TenantFreePlan { get; set; }

        [Required]
        public int GradeLevel { get; set; } = 0;


    }

}
