// Business/clsTenantService.cs
using Connection.models;
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
                Id = entity.Id,
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
                    DataKey = entity.Person.DataKey,
                    Email = entity.Person.Email,
                    FirstName = entity.Person.FirstName
                } : null!
            };
        }


        protected override Tenant FromDto(DtoTenant dto)
        {
            // No dates to parse here, simple mapping
            return new Tenant
            {
                Id = dto.Id,
                UniqueIdentifier = dto.UniqueIdentifier,
                CompanyName = dto.CompanyName,
                Description = dto.Description,
                PasswordHash=dto.PasswordHash,
                PersonId = dto.PersonId,
                IsActive = dto.IsActive,
                Role=dto.Role,
                CreatedAt = dto.CreatedAt == null ? throw new Exception("error(Parsing): the DTo value Is null ") : DateTime.TryParse(dto.CreatedAt, out DateTime result) == false ? throw new Exception("invaild string value DateTime") : result.ToUniversalTime()
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
            bool Result = false;
            tenant.PersonId = await _personService.AddAsync(tenant.Person);
            tenant.Person.Id = tenant.PersonId;
            Result = (tenant.PersonId == 0);
            if (Result) return 0;
            tenant.Id = await base.AddAsync(tenant);
            Result = (tenant.Id == 0);
            return tenant.Id;
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
