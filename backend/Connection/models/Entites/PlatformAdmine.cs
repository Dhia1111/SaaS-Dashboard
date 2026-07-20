
using Connection.models.Entites;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Connection.models
{ 

    public class PlatformAdmine : IEntityWithTenantId, IEntity
    {
            [Key]
            [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; } // PK, identity(1,1)
            [Required]
            public int TenantId { get; set; } // required
            [Required]
            public int PlatfromRole { get; set; }
            public bool IsActive { get; set; } = default;
           public Guid? Identifier {  get; set; }
           public Tenant? Tenant { get; set; } 

    }
}


