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
    public class PlatformPayment : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public Guid Guid { get; set; }
        [Required]
        public int TenantId { get; set; }
        [Required]

        [Required]
        public int PlatformPlanId {  get; set; }
        public int DataKey { get; set; }

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = null!;

        [Required]
        public int Status { get; set; }//enum={inProgress,Secsess,Failer}

        [Required]
        public string StripePaymentIntentId { get; set; } = null!;

        [Required]
        public DateTime PaidAt { get; set; }

        /* Navigation */
        public Tenant Tenant { get; set; } = null!;
        public PlatformPlan Plan { get; set; } = null!;
    }

}
