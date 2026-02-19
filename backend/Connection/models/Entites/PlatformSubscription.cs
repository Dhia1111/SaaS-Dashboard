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

        public int DataKey {  get; set; }

        [Required]
        public int PlatformPlanId { get; set; }

        [Required]
        public int Status { get; set; }//enum={inProgress,Secsess,Failer}
        [Required]
        public string StripeSubscriptionId { get; set; } = null!;

        [Required]
        public DateTime StartedAt { get; set; }

        public DateTime? EndsAt { get; set; }

        /* Navigation */
        public Tenant Tenant { get; set; } = null!;
        public PlatformPlan PlatformPlan { get; set; } = null!;
 
    }
}
