using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models.Entites
{
    public  class Payment :IEntity,IEntityWithTenantId
    {
        [Key]
        [DatabaseGenerated (DatabaseGeneratedOption.Identity)]
        public int  Id { get; set; }
        [Required]
        public int TenantId { get; set; }
    
        [Required]
        public int Provider {  get; set; }

        public string? ProviderPaymentId { get; set; }

        [Required]
        public int PaymentStatus { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }

        public DateTime? CompletedAt { get; set; } = null;

        [Required]

        public string Currency {  get; set; }=null!;

        [Required]
        public decimal Amount { get; set; }
        
        [Required]
        public int SubscriptionId { get; set; }

       public PlatformSubscription?Subscription { get; set; }

 
    }

}
