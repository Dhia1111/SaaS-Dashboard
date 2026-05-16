
using Connection.models.Entites;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Connection.models
{ 

    public class User : IEntityWithTenantId, IEntity
    {
           [Key]
           [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; } // PK, identity(1,1)
            [Required]
            public int TenantId { get; set; } // required
            [Required]
            public int PersonId { get; set; } // required public Person Person { get; set; } = null!;
            public Person? Person { get; set; }
            public string? PasswordHash { get; set; } 
            public int Role { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }
        public int Authorization { get; set; }
      public bool IsActive { get; set; } = default;

    }
}


