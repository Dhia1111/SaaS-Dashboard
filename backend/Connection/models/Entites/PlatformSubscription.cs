using Connection.models.Entites;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Connection.models
{
    public class PlatformSubscription : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TenantId { get; set; }


        [Required]
        public int TenantPlanPricingOptionId { get; set; }

        [Required]
        public DateTime StartedAt { get; set; }

        public DateTime? EndsAt { get; set; }

        public TenantPlanPricingOption? TenantPlanPricingOption { get; set; }
        public Tenant? Tenant { get; set; }
        public bool IsActive { get; set; } = false;
        public bool IsItFree{get;set;} = false;
        public bool IsRegisterdToClientSubscription { get; set; } = false;



    }
}
