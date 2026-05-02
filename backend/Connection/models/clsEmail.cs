
using Connection.Data;
using Connection.models;
using Connection.Models;
using Microsoft.Extensions.Logging;

public class DtoEmail
    {
        public string Subject { get; set; } = null!;
        public string From { get; set; } = null!;
        public string To { get; set; } = null!;
        public bool IsBodyAnHtml { get; set; }
        public string Body { get; set; } = null!;
        public bool IsSent { get; set; }=false;
        public DateTime? SentAt { get; set; }
        public DateTime CreatedAt { get; set; }=DateTime.UtcNow;
    public int Id { get; set; }
    
    }

public interface IEmailRepository: IGenericRepo<Email>
{


  
}

 

public class clsEmailRepository : GenericRepo<Email>,IEmailRepository
{

    public clsEmailRepository(SaasDashboardContext context, ILogger<clsEmailRepository> logger):base(context,logger)
    {
      
    }
    
    }

