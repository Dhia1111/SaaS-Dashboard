
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Connection.models
{ 

    public class User
    {
           [Key]
           [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
            public int Id { get; set; } // PK, identity(1,1)
            [Required]
            public int DataKey { get; set; } // required
            [Required]
            public int PersonID { get; set; } // required public Person Person { get; set; } = null!;
            public Person Person { get; set; }=null!;
            public string PasswordHash { get; set; } = null!;
            public int Role { get; set; }
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }
        }
    }


