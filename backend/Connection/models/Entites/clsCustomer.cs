using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models
{
    public partial class clsCustomerRepository
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string LastName { get; set; }= null!;
        public string? Adress { get; set; } 
        public string? phone { get; set; }



    }
}
