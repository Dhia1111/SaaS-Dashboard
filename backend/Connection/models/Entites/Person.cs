using Connection.models.Entites;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Connection.models
{
    public  class Person :IEntity,IEntityWithTenantId
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [Required]
       public int TenantId { get; set; }

        [Required]
        public string Email { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Address { get; set; } 
        public string? Phone { get; set; }
        public string? SecureCode { get; set; }
        public DateTime? EmailVerificationCodeExpiry { get; set; }
        [Required]
        public bool IsVeryfied {  get; set; }
        public string? ProviderId { get; set; } 
        public int ? Provider { get; set; }

    }
}
