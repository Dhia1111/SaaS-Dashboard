using Connection.models.Entites;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Connection.Models
{
    public class Email:IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
     
        public string To { get; set; } = null!;

        [Required]
        public string From { get; set; } = null!;

        [Required]
        public string Subject { get; set; } = null!;

        [Required]
        public string Body { get; set; } = null!;

        [Required]
        public bool IsBodyHtml { get; set; }

        [Required]
        public bool IsSent { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? SentAt { get; set; }
    }
}