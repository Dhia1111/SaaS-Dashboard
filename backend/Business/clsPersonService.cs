
using Connection.models;

using Microsoft.Extensions.Logging;




namespace Business
    {
        public interface IPersonService:IGenericService<DtoPerson>
        {
 

            Task<DtoPerson> GetByEmailAsync(string Email);


            Task<IReadOnlyList<DtoPerson>> SearchByNameAsync(string name);
        
            Task<DtoPerson?> FindBySecureCodeAsync(string secureCode);

           
    }
    

        public class clsPersonService :GenericService<DtoPerson,Person>, IPersonService  
        {
            private readonly IPersonRepository _repository;
            private readonly ILogger<clsPersonService> _logger;

         public clsPersonService(IPersonRepository repository,ILogger<clsPersonService> logger):base(repository,logger)
            {
                _repository = repository;
                _logger = logger;
            }
         public async Task<DtoPerson> GetByEmailAsync(string Email)
        {
            var person = await _repository.GetByEmailAsync(Email);

            if (person == null) {
               
                throw new KeyNotFoundException($"Person with id {Email} not found.");
                }
            return ToDto(person);
        }

   public async Task<DtoPerson> FindBySecureCodeAsync(string secureCode)
        {
            var person = await _repository.FindBySecureCodeAsync(secureCode);

            if (person == null) {
               
                throw new KeyNotFoundException($"Person with secureCode {secureCode} not found.");
            
              }
            return ToDto(person);
        }

        public async Task<IReadOnlyList<DtoPerson>> SearchByNameAsync(string name)
            {
                if (string.IsNullOrWhiteSpace(name))
                    throw new ArgumentException("Search name is required.");

                var people = await _repository.SearchByNameAsync(name.Trim());
                return people.Select(ToDto).ToList();
            }
 
         protected override  DtoPerson ToDto(Person person)
            {
                return new DtoPerson
                {
                    Id = person.Id,
                    Email = person.Email,
                    Phone = person.Phone,
                    FirstName = person.FirstName,
                    LastName = person.LastName,
                    Address=person.Address,
                    IsEmailVeryfied = person.IsVeryfied,
                    SecureCode = person.SecureCode,
                    EmailVerificationCodeExpiry= person.EmailVerificationCodeExpiry.HasValue ? person.EmailVerificationCodeExpiry.Value.ToString("o") : null,
                    ProviderId = person.ProviderId,
                        Provider = person.Provider

                };
            }
          protected   override  Person FromDto(DtoPerson person)
        {
            return new Person
            {
                Id = person.Id,
                Email = person.Email,
                Phone = person.Phone,
                FirstName = person.FirstName,
                LastName = person.LastName,
                Address = person.Address,
                IsVeryfied = person.IsEmailVeryfied,
                SecureCode = person.SecureCode,
                EmailVerificationCodeExpiry = DateTime.TryParse(person.EmailVerificationCodeExpiry, out DateTime date)? date.ToUniversalTime() :null,
                Provider= person.Provider,
                ProviderId = person.ProviderId,
                

            }; 
        }

    }
    }






