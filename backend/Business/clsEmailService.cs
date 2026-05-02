
using Connection.models;
using Connection.Models;
using Microsoft.Extensions.Logging;




namespace Business
    {
        public interface IEmailService:IGenericService<DtoEmail>
        {
 

        }
 


        public class clsEmailService :GenericService<DtoEmail,Email>, IEmailService  
        {
            private readonly IEmailRepository _repository;
            private readonly ILogger<clsEmailService> _logger;

         public clsEmailService(IEmailRepository repository,ILogger<clsEmailService> logger):base(repository,logger)
            {
                _repository = repository;
                _logger = logger;
            }
     
        protected override DtoEmail ToDto(Email entity)
        {
            return new DtoEmail
            {
                Id = entity.Id,
                To = entity.To,
                Subject = entity.Subject,
                Body = entity.Body,
                IsSent = entity.IsSent,
                SentAt = entity.SentAt,
                From = entity.From,
                IsBodyAnHtml = entity.IsBodyHtml,
                CreatedAt = entity.CreatedAt,

            };
        }
        protected override Email FromDto(DtoEmail dto)
        {
            return new Email
            {
                Id = dto.Id,
                To = dto.To,
                Subject = dto.Subject,
                Body = dto.Body,
                IsSent = dto.IsSent,
                SentAt = dto.SentAt,
                From=dto.From,
                IsBodyHtml=dto.IsBodyAnHtml,
                CreatedAt=dto.CreatedAt,
                


            };
        }
      



    }
}






