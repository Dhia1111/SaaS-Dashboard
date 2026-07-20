using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models.Entites
{
    public class TenantFreePlan:IEntity,IEntityWithTenantId
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TenantId {  get; set; }

        [Required]
        public int TenantPlanId { get; set; }

        public int CycleId {  get; set; }

        public DateTime StartAt {  get; set; }

        public DateTime EndAt { get; set; }

        public Tenant? Tenant { get; set; }

        public TenantPlan? TenantPlan {  get; set; }



    }
}
