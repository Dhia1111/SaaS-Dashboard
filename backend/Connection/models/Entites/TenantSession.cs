using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models.Entites
{
    public class TenantSession:IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
            public int TenantId { get; set; }
           [Required]
            public int  DataKey {  get; set; }

            [Required]
            public string CurrentRefreshTokenHash { get; set; } = null!;
            public string? PreviousRefreshTokenHash { get; set; }
            public string ? RevokedReason { get; set; }
            public string ?  RevokedByIp { get; set; }
            public DateTime GraceUntil { get; set; }

            public DateTime ExpiresAt { get; set; }
            public DateTime? RevokedAt { get; set; }
           public DateTime? LastRefreshedAt { get; set; }
            public string? LastRefreshedIp { get; set; }
            public string? TenantAgent { get; set; }
            public string? IpAddress { get; set; }
        
    }
}
