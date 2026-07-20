using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models.Entites
{
    public class TenantPlanBenefit:IEntity,IEntityWithTenantId
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int  Id { get; set; }
        [Required]
         public int TenantId { get; set; }
        public int  TenantPlanId { get; set; }

        public string Title { get; set; } = null!;

        public string? Description { get; set; }

        public TenantPlan? TenantPlan { get; set; }


    }


}
