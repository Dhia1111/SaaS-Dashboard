
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Connection.models
{ 

    public class Tenant
    {
           [Key]
           [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; } // PK, identity(1,1)
            [Required]
            public int DataKey { get; set; } // required
          [Required]
            public int PersonID { get; set; } // required
            public string PasswordHash { get; set; } = null!;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }
            public Person Person { get; set; }= null!;
        

        }
    }


