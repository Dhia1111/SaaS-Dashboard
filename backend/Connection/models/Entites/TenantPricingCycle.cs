using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models.Entites
{
    public  class TenantPricingCycle:IEntity,IEntityWithTenantId
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int TenantId { get; set; }

        [Required]
        public string CycleName { get; set; } = null!;

        public int Period { get; set; }  // e.g., 1,2

        public string PeriodUnit { get; set; } = null!;// in days or in months

            



    }
}
