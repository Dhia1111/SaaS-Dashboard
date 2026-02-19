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
    public class UserSubscription : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int UserId {  get; set; }
        [Required]
        public int TenantId { get; set; }
        [Required]
        public int DataKey { get; set; }

          [Required]
          public string UserReferenceId { get; set; } = null!;

        [Required]
        public int TenantPlanId { get; set; }

        [Required]
        public int Status { get; set; } //enum {inprograss,secsessd,failed}

        [Required]
        public int SubscriptionTypeId {  get; set; }

        [Required]
        public string StripeSubscriptionId { get; set; } = null!;

        [Required]
        public DateTime StartedAt { get; set; }

        public DateTime? EndsAt { get; set; }
        public Tenant Tenant { get; set; } = null!;
        public TenantPlan TenantPlan { get; set; } = null!;
        


    }

}
