using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models.Entites
{
   
        public class UserSession
        {
            public int SessionId { get; set; }
            public int UserId { get; set; }

            [Required]
            public string CurrentRefreshTokenHash { get; set; } = null!;
            public string? PreviousRefreshTokenHash { get; set; }

            public DateTime GraceUntil { get; set; }

            public DateTime ExpiresAt { get; set; }
            public DateTime? RevokedAt { get; set; }

            public string? UserAgent { get; set; }
            public string? IpAddress { get; set; }
        
    }
}
