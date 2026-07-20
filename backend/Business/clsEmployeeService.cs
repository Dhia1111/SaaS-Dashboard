using Business;
using Connection.models;
using Microsoft.Extensions.Logging;
using SharedDto_Enum;
public interface IEmployeeService : IGenericService<DtoEmployee>
    {
    Task<DtoEmployee?> GetByUserIdAsync(int UserId);
    }
namespace Business
{
    

    public class clsEmployeeService
        : GenericService<DtoEmployee, Employee>,
          IEmployeeService
    {
        private readonly IEmployeeRepo _userRepo;
        private readonly ILogger<clsEmployeeService> _logger;
        private readonly IPasswordHashService _passWordHashService;
        private readonly IEmailService _emailService;
        private readonly ITenantIdProvider _tenantIdProvider;
        private readonly ITenantService _tenantService;
        private readonly IPersonService _personService;

        public clsEmployeeService
        (
            IEmployeeRepo userRepo,
            ILogger<clsEmployeeService> logger,
            IPasswordHashService passWordHashService,
            IEmailService emailService,
            ITenantIdProvider tenantIdProvider,
            ITenantService tenantService ,
            IPersonService personService
        )
            : base(userRepo, logger)
        {
            _userRepo = userRepo;
            _logger = logger;
            _passWordHashService= passWordHashService;
            _emailService = emailService;
        
            _tenantIdProvider = tenantIdProvider;
            _tenantService = tenantService;
            _personService = personService;
        }

        protected override DtoEmployee ToDto(Employee platformUser)
        {
            return new DtoEmployee
            {
                AdminstrationAuth = platformUser.AdminstrationAuth,
                Id = platformUser.Id,
                identifier = platformUser?.Identifier?.ToString(),
                IsActive = platformUser.IsActive,
                PlatformRole = (enPlaformRoles)platformUser.PlatformRole,
                TenantId = platformUser.TenantId,
                UserId = platformUser.UserId,
                User = platformUser.User!=null ? new DtoUser
                {
                    Id = platformUser.User.Id,
                    PersonID = platformUser.User.PersonId,
                    Authorization = platformUser.User.Authorization,
                    Role = platformUser.User.Role,
                    CreatedAt = platformUser.User.CreatedAt.ToString(),
                    UpdatedAt = platformUser.User.UpdatedAt?.ToString(),
                    PasswordHash = platformUser.User.PasswordHash,
                    TenantId = platformUser.User.TenantId,

                    Person = platformUser.User.Person == null
                    ? null
                    : new DtoPerson
                    {
                        Id = platformUser.User.Person.Id,
                        Email = platformUser.User.Person.Email,
                        Phone = platformUser.User.Person.Phone,
                        FirstName = platformUser.User.Person.FirstName,
                        LastName = platformUser.User.Person.LastName,
                        Address = platformUser.User.Person.Address,
                        SecureCode = platformUser.User.Person.SecureCode,
                        EmailVerificationCodeExpiry =
                            platformUser.User.Person.EmailVerificationCodeExpiry?.ToString(),
                        IsEmailVeryfied = platformUser.User.Person.IsVeryfied,
                        Provider = platformUser.User.Person.Provider,
                        ProviderId = platformUser.User.Person.ProviderId

                    },
                    IsActive = platformUser.User.IsActive
                } :null



            };
        }

        protected override Employee FromDto(DtoEmployee platformUser)
        {
            return new Employee
            {
                AdminstrationAuth = platformUser.AdminstrationAuth,
                Id = platformUser.Id,
                Identifier = Guid.TryParse(platformUser.identifier,out Guid idnet)?idnet:null,
                IsActive = platformUser.IsActive,
                PlatformRole = (int)platformUser.PlatformRole,
                TenantId = platformUser.TenantId,
                UserId = platformUser.UserId,
                User = platformUser.User != null ? new User
                {
                    Id = platformUser.User.Id,
                    PersonId = platformUser.User.PersonID,
                    Authorization = platformUser.User.Authorization,
                    Role = platformUser.User.Role,
                    CreatedAt = DateTime.Parse(platformUser.User.CreatedAt),
                    UpdatedAt = DateTime.Parse(platformUser.User.UpdatedAt),
                    TenantId = platformUser.User.TenantId,

                    Person = platformUser.User.Person == null
                    ? null
                    : new Person
                    {
                        Id = platformUser.User.Person.Id,
                        Email = platformUser.User.Person.Email,
                        Phone = platformUser.User.Person.Phone,
                        FirstName = platformUser.User.Person.FirstName,
                        LastName = platformUser.User.Person.LastName,
                        Address = platformUser.User.Person.Address,
                        SecureCode = platformUser.User.Person.SecureCode,
                        EmailVerificationCodeExpiry=DateTime.Parse(platformUser.User.Person.EmailVerificationCodeExpiry),
                        IsVeryfied = platformUser.User.Person.IsEmailVeryfied,
                        Provider = platformUser.User.Person.Provider,
                        ProviderId = platformUser.User.Person.ProviderId

                    },
                    IsActive = platformUser.User.IsActive
                } : null




            };
        }

        public override async Task<int> AddAsync(DtoEmployee dto)
        {
            if (dto == null || dto?.User == null || dto?.User?.Person == null) throw new ArgumentException();


            DtoTenant? Tenant = await _tenantService.GetByIdAsync(_tenantIdProvider.TenantId);
            if (Tenant == null) throw new Exception($"Tenant with an ID {_tenantIdProvider.TenantId} does not exist ");

            DtoPerson? Person = await _personService.GetByIdAsync(Tenant.PersonId);
            if (Person == null) throw new Exception($"Person with an ID {Tenant.PersonId} does not exist ");

            dto.User.CreatedAt = DateTime.Now.ToString();
            dto.User.TenantId = _tenantIdProvider.TenantId;
            dto.User.Person.TenantId = _tenantIdProvider.TenantId;


            DtoEmail email = new DtoEmail
            {

                IsBodyAnHtml = false,
                Subject = "Account info",
                Body = $@"
                      PassWord : {dto.User.PasswordHash}
                      TenantName:{Tenant.Name} 
                                          
                ",
                CreatedAt = DateTime.Now,
                From = Person.Email,
                To = dto.User.Person.Email,






            };
            await _emailService.AddAsync(email);
            dto.User.PasswordHash=_passWordHashService.Hash(dto.User.PasswordHash);

            return await base.AddAsync(dto);
        
        }
        
        public async Task<DtoEmployee?>GetByUserIdAsync(int userId)
        {
            var res= await _userRepo.GetByUserIdAsync(userId);
            return res==null? null : ToDto(res);
        }

        }


}