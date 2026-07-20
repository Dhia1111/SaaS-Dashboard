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
    public class ClientSubscription :IEntity,IEntityWithTenantId
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        public bool IsFree { get; set; }// free, paid
        [Required]
        public int GradeStatus { get; set; } // Downgrade, Upgrade, Renewal,MoveToPaid, MoveToFree
        public string? Discription { get; set; }

        public Tenant? Tenant { get; set; }

        [Required]
        public DateTime CreatedAt { get; set; }


        [Required]
        public string TenantClientIdentifier { get; set; } = null!;


    }
}
