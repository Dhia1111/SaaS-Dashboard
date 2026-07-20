using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Config
{
    public class PlatformInfo
    {
        [Required]
        public string TenantName { get; set; } = null!;
        public string DefaultCurrency { get; set; } = null!;

    }
}
