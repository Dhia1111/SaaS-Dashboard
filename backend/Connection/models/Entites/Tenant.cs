
using Connection.models.Entites;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Connection.models
{ 

    public class Tenant 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; } // PK Identity(1,1) == DataKey

        [Required]
        [MaxLength(100)]
        public string UniqueIdentifier { get; set; } = null!; // public tenant id (used in redirect)

        [Required]
        [MaxLength(200)]
        public string CompanyName { get; set; } = null!;

        [MaxLength(500)]
        public string? Description { get; set; }

        [Required]
        public bool IsActive { get; set; } = true;

        [Required]
        public string PasswordHash { get; set; } = null!;

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


