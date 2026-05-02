
using Connection.models.Entites;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Connection.models
{ 

    public class Tenant :IEntityWithTenantId
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TenantId { get; set; } // PK Identity(1,1) == DataKey
        
        [MaxLength(100)]
        public string? UniqueIdentifier { get; set; }  // public tenant id (used in redirect)

         [MaxLength(200)]
        public string? CompanyName { get; set; } 

        [MaxLength(500)]
        public string? Description { get; set; }

        public bool IsActive { get; set; } = true;

        public string? PasswordHash { get; set; } 

        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
        public int Role { get; set; }
        // 🔑 OWNER
        [Required]
        public int PersonId { get; set; }


        [ForeignKey(nameof(PersonId))]
        public Person Person { get; set; } = null!;



    }
}


