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
    public class TenantPlan : IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int DataKey { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        [Required]
         public int BillingInterval { get; set; } //enum{monthly ,year}

        [Required]
        public string StripePriceId { get; set; } = null!;

        [Required]
        public bool IsActive { get; set; }

        /* Navigation */
        public Tenant Tenant { get; set; } = null!;
    }

}
