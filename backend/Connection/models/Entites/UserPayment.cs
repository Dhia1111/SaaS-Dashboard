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
    public class UserPayment : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TenantId { get; set; }
        [Required]
        public int DataKey { get; set; }

        [Required]
        public string UserReferenceId { get; set; } = null!;

        [Required]
        public decimal Amount { get; set; }

        [Required]
        public string Currency { get; set; } = null!;

        [Required]
        public string Status { get; set; } = null!;

        [Required]
        public string StripePaymentIntentId { get; set; } = null!;

        [Required]
        public DateTime PaidAt { get; set; }

        /* Navigation */
        public Tenant Tenant { get; set; } = null!;
    }

}
