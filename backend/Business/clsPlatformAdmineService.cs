using Business;
using Connection.models;
using Microsoft.Extensions.Logging;
using SharedDto_Enum;
public interface IPlatformAdmineService : IGenericService<DtoPlatformAdmine>
   {

    Task<DtoPlatformAdmine?> GetByTenantIdAsync(int TenantId);

     }

namespace Business
{
   
    public class clsPlatformAdmineService
        : GenericService<DtoPlatformAdmine, PlatformAdmine>,
          IPlatformAdmineService
    {
        private readonly IPlatformAdmineRepo _admineRepo;
        private readonly ILogger<clsPlatformAdmineService> _logger;

        public clsPlatformAdmineService
        (
            IPlatformAdmineRepo admineRepoRepo,
            ILogger<clsPlatformAdmineService> logger
        )
            : base(admineRepoRepo, logger)
        {
            _admineRepo = admineRepoRepo;
            _logger = logger;
        }

        protected override DtoPlatformAdmine ToDto(PlatformAdmine user)
        {
            return new DtoPlatformAdmine
            {
                IsActive = user.IsActive,
                Id = user.Id,
                Identifier = user?.Identifier?.ToString(),
                PlatformRole = (enPlaformRoles)user.PlatfromRole,
                TenantId=user.TenantId,
                


            };
        }

        protected override PlatformAdmine FromDto(DtoPlatformAdmine  dto)
        {
            return new PlatformAdmine
            {
                Id = dto.Id,
                Identifier = Guid.TryParse(dto.Identifier, out Guid ident) ? ident : null,
                IsActive=dto.IsActive,
                PlatfromRole=(int)dto.PlatformRole,
                TenantId=dto.TenantId,
               
                
            };
        }

       public async Task<DtoPlatformAdmine?> GetByTenantIdAsync(int TenantId)
        {
            var res= await _admineRepo.GetByTenantIdAsync(TenantId);
            return res!=null? ToDto(res):null;
        }

    }
}