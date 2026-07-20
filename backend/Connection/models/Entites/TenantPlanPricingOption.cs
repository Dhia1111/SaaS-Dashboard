using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Connection.models.Entites
{
    public class TenantPlanPricingOption:IEntity, IEntityWithTenantId
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public int TenantId { get; set; }
        [Required]
        public int TenantPricingCycleId { get; set; }

        public TenantPricingCycle? TenantPricingCycle { get; set; }
        public decimal Price { get; set; }
        public string Currency { get; set; } = "USD";
        public bool IsActive { get; set; } = true;
        [Required]
        public int TenantPlanId { get; set; }

        public TenantPlan? TenantPlan { get; set; }
        public Tenant? Tenant { get; set; }
    }
}
