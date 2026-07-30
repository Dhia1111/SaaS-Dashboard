using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models.Entites
{
    public  class Domain :IEntity,IEntityWithTenantId
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id {  get; set; }
        public int TenantId {  get; set; }                      
        public string Name { get; set; }=null!;
        public Tenant? Tenant { get; set; }

    }

}
