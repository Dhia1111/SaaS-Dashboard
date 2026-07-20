using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models.Entites
{
    // how did the client haired about the tenant platform 
    public class DiscoveryPlatform:IEntityWithTenantId,IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]  
        public int TenantId { get; set; }
       
        [Required]
        public string TenantClientIdentifier {  get; set; }=null!;

        [Required]
        public int MarkettingPlatform {  get; set; }


    }
}
