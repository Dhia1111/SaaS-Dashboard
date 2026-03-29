using Connection.models.Entites;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Connection.models
{
    public  class Person :IEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public int DataKey { get; set; } 
        public string Email { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; } 
        public string? Phone { get; set; }

        public string SecureCode { get; set; } = null!;

        public DateTime EmailVerificationCodeExpiry { get; set; } = DateTime.UtcNow;
        [Required]
        public bool IsEmailVeryfied {  get; set; }
        




    }
}
