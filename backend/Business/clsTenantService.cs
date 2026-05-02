// Business/clsTenantService.cs
using Connection.models;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;


namespace Business
{
    public interface ITenantService : IGenericService<DtoTenant>
    {

        public Task<DtoTenant?> GetByEmailAsync(string email);

        Task<DtoTenant?> GetByUniqueIdentifierWithPersonAsync(string uniqueIdentifier);
        Task<DtoTenant?> GetByPersonHashSecureCodeAsync(string secureCode);
    }

    public class clsTenantService : GenericService<DtoTenant, Tenant>, ITenantService
    {
        private readonly ITenantRepo _tenantRepo;
        private readonly IPersonService _personService;


        public clsTenantService(ITenantRepo tenantRepo, ILogger<clsTenantService> logger, 
            IPersonService personService)
            : base(tenantRepo, logger)
        {
            _tenantRepo = tenantRepo;
            _personService = personService;
        }

        protected override DtoTenant ToDto(Tenant entity)
        {
            return new DtoTenant
            {
                TenantId = entity.TenantId,
                UniqueIdentifier = entity.UniqueIdentifier,
                CompanyName = entity.CompanyName,
                Description = entity.Description,
                PasswordHash=entity.PasswordHash,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt.ToLongDateString(),
                UpdatedAt = entity.UpdatedAt?.ToLongDateString(),
                Role=entity.Role,
                PersonId = entity.PersonId,
                Person = entity.Person != null ? new DtoPerson
                {
                    Id = entity.Person.Id,
                    Email = entity.Person.Email,
                    FirstName = entity.Person.FirstName,
                    SecureCode=entity.Person.SecureCode,
                    Address=entity.Person.Address,
                    EmailVerificationCodeExpiry = entity.Person.EmailVerificationCodeExpiry.HasValue ? entity.Person.EmailVerificationCodeExpiry.Value.ToString("o") : null ,
                    IsEmailVeryfied=entity.Person.IsVeryfied,
                    LastName=entity.Person.LastName,
                    Phone=entity.Person.Phone
                    
                } : null
                
            };
        }


        protected override Tenant FromDto(DtoTenant dto)
        {
            // No dates to parse here, simple mapping
            return new Tenant
            {
                TenantId = dto.TenantId,
                UniqueIdentifier = dto.UniqueIdentifier,
                CompanyName = dto.CompanyName,
                Description = dto.Description,
                PasswordHash = dto.PasswordHash,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Parse(dto.CreatedAt).ToUniversalTime(),
                UpdatedAt = DateTime.TryParse(dto.UpdatedAt, out DateTime updatedDate) ? updatedDate.ToUniversalTime() : null,
                Role = dto.Role,
                PersonId = dto.PersonId,
                Person = dto.Person != null ? new Person 
                { 
                    Id = dto.Person.Id,
                    Email = dto.Person.Email,
                    FirstName = dto.Person.FirstName,
                    SecureCode = dto.Person.SecureCode,
                    Address = dto.Person.Address,
                    EmailVerificationCodeExpiry = dto.Person.EmailVerificationCodeExpiry != null && DateTime.TryParse(dto.Person.EmailVerificationCodeExpiry?.ToString(), out DateTime EmailCodeExprtyAt) ? EmailCodeExprtyAt.ToUniversalTime() : null,
                    IsVeryfied = dto.Person.IsEmailVeryfied,
                    LastName = dto.Person.LastName,
                    Phone = dto.Person.Phone,
                    Provider = dto.Person.Provider,
                    ProviderId = dto.Person.ProviderId,



                } :null,

            }; 
        }

        public async Task<DtoTenant?> GetByUniqueIdentifierWithPersonAsync(string uniqueIdentifier)
        {
            var tenant = await _tenantRepo.GetByUniqueIdentifierWithPersonAsync(uniqueIdentifier);
            return tenant != null ? ToDto(tenant) : null;
        }
        public async Task<DtoTenant?> GetByPersonHashSecureCodeAsync(string secureCode)
        {
            var person = await _personService.FindBySecureCodeAsync(secureCode);
            if (person == null) return null;    
            var tenant = await _tenantRepo.GetByEmailAsync(person.Email);
            if (tenant == null) return null;
            DtoTenant dtoTenant = ToDto(tenant);
            dtoTenant.Person = person; // attach person to tenant for complete DTO
            return dtoTenant;
        }

        public override async Task<int> AddAsync(DtoTenant tenant)
        {

            return  await _tenantRepo.AddAsync(FromDto(tenant));
        }

        public override async Task<bool> UpdateAsync(DtoTenant tenant)
        {
            bool Result = false;
            Result=await _personService.UpdateAsync(tenant.Person);
            if (!Result) return false;
            Result= await base.UpdateAsync(tenant);
            return Result;
        }

        public async Task<DtoTenant?> GetByEmailAsync(string email)
        {
            Tenant? t=  await _tenantRepo.GetByEmailAsync(email);
            if(t != null) return this.ToDto(t);
            return null;
        }

        
    }
}
