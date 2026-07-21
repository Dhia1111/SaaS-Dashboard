using Business.Config;
using Connection.models;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Business
{
    public interface IUserService : IGenericService<DtoUser>
    {
        Task<DtoUser?> GetByEmailAsync(string email);
        Task<List<KeyValuePair<long,string>>> GetTenantPermissionForUsers();
    }

    public class clsUserService
        : GenericService<DtoUser, User>,
          IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly ILogger<clsUserService> _logger;
        private readonly ITenantIdProvider _tenantIdProvider;
        private readonly ITenantRepo _tenantRepo;
        private readonly IPlatformSubscriptionRepo _platformSubscriptionRepo;
        private readonly ITenantPlanRepository _tenantPlanRepo;
        private readonly ITenantPermissionRepository _tenantPermissionRepo;
        private readonly PlatformInfo _platformInfo;
        public clsUserService
        (
            IUserRepo userRepo,
            ILogger<clsUserService> logger,
            ITenantIdProvider tenantIdProvider,
            IPlatformSubscriptionRepo platformSubscriptionRepo,
            ITenantRepo tenantRepo,
            ITenantPlanRepository tenantPlanRepository,
            ITenantPermissionRepository tenantPermissionRepository,
            IOptions<PlatformInfo>platformInfo
        )
            : base(userRepo, logger)
        {
            _userRepo = userRepo;
            _logger = logger;
            _tenantIdProvider = tenantIdProvider;
            _tenantRepo = tenantRepo;
            _platformSubscriptionRepo= platformSubscriptionRepo;
            _tenantPlanRepo = tenantPlanRepository;
            _tenantPermissionRepo= tenantPermissionRepository;
            _platformInfo = platformInfo.Value;
        }

        protected override DtoUser ToDto(User user)
        {
            return new DtoUser
            {
                Id = user.Id,
                PersonID = user.PersonId,
                Authorization = user.Authorization,
                Role = user.Role,
                CreatedAt = user.CreatedAt.ToString(),
                UpdatedAt = user.UpdatedAt?.ToString(),
                PasswordHash = user.PasswordHash,
                TenantId = user.TenantId,

                Person = user.Person == null
                    ? null
                    : new DtoPerson
                    {
                        Id = user.Person.Id,
                        Email = user.Person.Email,
                        Phone = user.Person.Phone,
                        FirstName = user.Person.FirstName,
                        LastName = user.Person.LastName,
                        Address = user.Person.Address,
                        SecureCode = user.Person.SecureCode,
                        EmailVerificationCodeExpiry =
                            user.Person.EmailVerificationCodeExpiry?.ToString(),
                        IsEmailVeryfied = user.Person.IsVeryfied,
                        Provider = user.Person.Provider,
                        ProviderId = user.Person.ProviderId
                        
                    },
                IsActive=user.IsActive
            };
        }

        protected override User FromDto(DtoUser dto)
        {
            return new User
            {
                Id = dto.Id,
                PersonId = dto.PersonID,
                Authorization = dto.Authorization,
                Role = dto.Role,
                PasswordHash = dto.PasswordHash,
                TenantId = dto.TenantId,

                CreatedAt =
                    DateTime.Parse(dto.CreatedAt).ToUniversalTime(),

                UpdatedAt =
                    dto.UpdatedAt == null
                    ? null
                    : DateTime.Parse(dto.UpdatedAt).ToUniversalTime(),

                Person = dto.Person == null
                    ? null
                    : new Person
                    {
                        Id = dto.PersonID,
                        Email = dto.Person.Email,
                        Phone = dto.Person.Phone,
                        FirstName = dto.Person.FirstName,
                        LastName = dto.Person.LastName,
                        Address = dto.Person.Address,
                        SecureCode = dto.Person.SecureCode,
                        IsVeryfied = dto.Person.IsEmailVeryfied,
                        Provider = dto.Person.Provider,
                        ProviderId = dto.Person.ProviderId,
                        TenantId = dto.TenantId
                    },
                IsActive=dto.IsActive

            };
        }

        public async Task<DtoUser?> GetByEmailAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);

            return user == null
                ? null
                : ToDto(user);
        }

        public override async Task<int> AddAsync(DtoUser dto)
        {
            return await _userRepo.AddAsync(FromDto(dto));
        }

        public override async Task<bool> UpdateAsync(DtoUser dto)
        {
            return await _userRepo.UpdateAsync(FromDto(dto));
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning
                (
                    "User {UserId} not found",
                    id
                );

                return false;
            }

            return await _userRepo.DeleteAsync(user);
        }


        public async Task<List<KeyValuePair<long,string>>> GetTenantPermissionForUsers()
        {

            List<KeyValuePair<long, string>> List = new();

            PlatformSubscription? ActiveSubscription = await _platformSubscriptionRepo.GetActiveByTenantIdAsync(_tenantIdProvider.TenantId);
            if(ActiveSubscription == null||ActiveSubscription.TenantPlanPricingOption==null)
            {
                return [];
            }
            var Plataform = await _tenantRepo.GetByNameAsync(_platformInfo.TenantName);
            if (Plataform == null)
            {
                _logger.LogError("could not read platform information ");
                throw new Exception("Could not read env");
            }

            TenantPlan? plan = await _tenantPlanRepo.GetSingleWithDependenciesIgnoreQuerryAsync(Plataform,ActiveSubscription.TenantPlanPricingOption.TenantPlanId);


            if (plan == null ||plan.Permissions==null||plan.Permissions.Count==0) {
                return []
                ;
            }


           
            var PlatformPermissions = await _tenantPermissionRepo.GetAllByTenantIdWithFilterIgnoreAsync(Plataform.TenantId);

 
            Dictionary<int, TenantPermission> map = new();

            foreach (var permission in PlatformPermissions) {

                map.Add(permission.Id,permission);
            
            }

            foreach(var permission in plan.Permissions)
            {

                var platformPermission = map[permission.PermissionId];
                List.Add(new KeyValuePair<long, string>(platformPermission.BitValue, platformPermission.PermissionKey));
                

            }

            return List;

           


        }

    }
}