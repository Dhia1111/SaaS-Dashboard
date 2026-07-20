
using Connection.models.Entites;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Connection.models
{ 

    public class Employee : IEntityWithTenantId, IEntity
    {
           [Key]
           [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; } // PK, identity(1,1)
           [Required]
            public int UserId { get; set; } 
            [Required]
            public int TenantId { get; set; } // required
            [Required]
            public int PlatformRole { get; set; }
           public int AdminstrationAuth { get; set; }
           public bool IsActive { get; set; } = default;
           public User? User { get; set; }
           
           public Guid? Identifier { get; set; }
        
           

    }
}


