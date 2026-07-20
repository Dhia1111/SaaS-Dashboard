using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Config;
using Connection;
using Connection.models;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Business
{
    public interface IPermissionLoader
    {
        IReadOnlyDictionary<string, long> GetPermissions();

        bool TryGetPermission(string key, out long permission);

        Task ReloadAsync(CancellationToken cancellationToken = default);

        DateTime LastReloadTime { get; }

    }

    public interface ITenantPermissionServices: IGenericService<DtoTenantPermission>
    {

        Task<bool> IsPermissionKeyExist(string key);
        Task<List<DtoTenantPermission>> GetAllByTenantNameWithFilterIgnoreAsync(string TenantName);
 

    }



    public class clsTenantPermissionServices : GenericService<DtoTenantPermission, TenantPermission>, ITenantPermissionServices
    {

        private readonly ITenantPermissionRepository _tenantPermissionRepository;
        private readonly ILogger<clsTenantPermissionServices> _logger;
        private readonly ITenantRepo _tenantRepo;
        private readonly IPermissionLoader _permissionLoader;
        private readonly PlatformInfo _platformInfo;
        private readonly ITenantIdProvider _tenantIdProvider;
        public clsTenantPermissionServices(ILogger<clsTenantPermissionServices> logger,
            ITenantPermissionRepository repo,
            ITenantRepo tenantRepo,
            IPermissionLoader permissionLoader,
            IOptions<PlatformInfo>PlatformInformations,
            ITenantIdProvider TenantIdProvider)
               : base(repo, logger)
        {
            _tenantPermissionRepository = repo;
            _tenantRepo = tenantRepo;
            _logger = logger;
            _permissionLoader = permissionLoader;
            _platformInfo = PlatformInformations.Value;
            _tenantIdProvider = TenantIdProvider;

        }

        protected override TenantPermission FromDto(DtoTenantPermission dto)
        {
            return new TenantPermission
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                PermissionKey = dto.PermissionKey,
                BitValue = dto.BitValue,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Parse(dto.CreatedAt).ToUniversalTime(),
                
                
            }
            ;
        }

        protected override DtoTenantPermission ToDto(TenantPermission entity)
        {
            return new DtoTenantPermission
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                PermissionKey = entity.PermissionKey,
                BitValue = entity.BitValue,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt.ToString("o"),

            }
            ;
        }

        override public  async Task<int> AddAsync(DtoTenantPermission dto)
        {

               
            _logger.LogInformation("Adding new tenant permission with key: {PermissionKey}", dto.PermissionKey);

            long existingBitValue =await _tenantPermissionRepository.GetMaxBitAsync();

            int  NextPowerValue = 0 ;
            if (existingBitValue == 0)
            {
                NextPowerValue = 0;
            }
            else {

                NextPowerValue =(int)( Math.Log(existingBitValue, 2) + 1);


            }
            dto.BitValue=(long)Math.Pow(2, NextPowerValue);
            dto.CreatedAt = DateTime.Now.ToString();
            

             var res= await base.AddAsync(dto);


         if (await IsTheClientThePlatFormOwner())  await _permissionLoader.ReloadAsync();
            return res;
        }
        private async Task<bool> IsTheClientThePlatFormOwner()
        {
            int TenantId = _tenantIdProvider.TenantId;
            
            Tenant? Platform = await _tenantRepo.GetByNameAsync(_platformInfo.TenantName);

            if(Platform == null || Platform.TenantId != TenantId)
            {
                return false;
            }
            return true;


            

        }

        public override async Task<bool> UpdateAsync(DtoTenantPermission dto)
        {
            var res= await base.UpdateAsync(dto);
          if(await IsTheClientThePlatFormOwner())  await _permissionLoader.ReloadAsync();
            return res;
        }

        public override async Task<bool> DeleteAsync(int id)
        {

            var res= await base.DeleteAsync(id);
            if (await IsTheClientThePlatFormOwner())  await _permissionLoader.ReloadAsync();
            return res;
        }
        public async Task<bool> IsPermissionKeyExist(string key)
        {
           return await _tenantPermissionRepository.IsPermissionKeyExist(key);

        }

        public async Task<List<DtoTenantPermission>> GetAllByTenantNameWithFilterIgnoreAsync(string TenantName)
        {
            Tenant? Tenant=await _tenantRepo.GetByNameAsync(TenantName)   ;

            if (Tenant == null) {

                _logger.LogError("Serch for no longer exsisting tenant");
                throw new ArgumentException("Not tenant with this name ");

            }

            var res= await _tenantPermissionRepository.GetAllByTenantIdWithFilterIgnoreAsync(Tenant.TenantId);

            return res.Select(e => ToDto(e)).ToList();
        }


     }
}
