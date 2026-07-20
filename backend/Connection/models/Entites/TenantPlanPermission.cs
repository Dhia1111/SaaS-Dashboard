 
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
 

namespace Connection.models.Entites
{
    public class TenantPlanPermission:IEntity,IEntityWithTenantId
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        [Required]
        public int TenantId { get; set; }
        [Required]
        public int TenantPlanId { get; set; }
        [Required]
        public int PermissionId { get; set; }

        
        public TenantPlan? TenantPlan { get; set; }

        public TenantPermission? Permission { get; set; }

    }
}
