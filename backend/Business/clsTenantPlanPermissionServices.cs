
using Connection;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;

namespace Business
{
    public interface ITenantPlanPermissionServices : IGenericService<DtoTenantPlanPermission>
    {

        public TenantPlanPermission GetEntity(DtoTenantPlanPermission dto);

        public DtoTenantPlanPermission GetDto(TenantPlanPermission Entity);




    }



    public class clsTenantPlanPermissionServices : GenericService<DtoTenantPlanPermission, TenantPlanPermission>, ITenantPlanPermissionServices
   {

        private readonly ITenantPlanPermissionRepository _tenantPermissionRepository;
        private readonly ILogger<clsTenantPlanPermissionServices> _logger;
        public clsTenantPlanPermissionServices(ILogger<clsTenantPlanPermissionServices> logger, ITenantPlanPermissionRepository repo) 
               : base(repo, logger)
        {        
             _tenantPermissionRepository = repo;
                _logger = logger;
        }

        protected override TenantPlanPermission FromDto(DtoTenantPlanPermission dto)
        {
            return new TenantPlanPermission
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                PermissionId= dto.PermissionId,
                TenantPlanId=dto.TenantPlanId,
                
                
               

            }
            ;
        }

        protected override DtoTenantPlanPermission ToDto(TenantPlanPermission entity)
        {
            return new DtoTenantPlanPermission
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
               PermissionId= entity.PermissionId,
               TenantPlanId=entity.TenantPlanId,
               
               
            }
            ;
        }


        public TenantPlanPermission GetEntity(DtoTenantPlanPermission dto)
        {
            return FromDto(dto);
        }

        public DtoTenantPlanPermission GetDto(TenantPlanPermission entity)
        {
            return ToDto(entity);
        }

    }


}
