using APIs.Controllers;
using Business;
using Business.Config;
using Connection;
using Connection.models;
using Microsoft.Extensions.Options;

namespace APIs.ConfigClasses
{
    public class PermissionsLoader:IPermissionLoader
    {

        private DateTime _LastReloadTime;
        private Dictionary<string, long> _permissions;
        private readonly PlatformInfo _platformInfo;
        private readonly ITenantPermissionRepository _tenantPermissionRepository;
        private readonly ITenantRepo _tenantRepo;
        private ILogger<PermissionsLoader> _logger;
        public DateTime LastReloadTime { get { return _LastReloadTime; } }

        public PermissionsLoader(ITenantPermissionRepository PlatformPermissionRepo,
        IOptions<PlatformInfo> platforminfo,
        ITenantRepo tenantRepo,
        ILogger<PermissionsLoader> logger
           

         )
        {

            _logger = logger;
            _tenantRepo = tenantRepo;
            _LastReloadTime = DateTime.UtcNow;
            _tenantPermissionRepository = PlatformPermissionRepo;
            _platformInfo = platforminfo.Value;
            _permissions = new();


        }

       
        private async Task<Dictionary<string, long>> GetAllAsMapByTenantNameWithFilterIgnoreAsync()
        {
            Tenant? Tenant = await _tenantRepo.GetByNameAsync(_platformInfo.TenantName);

            if (Tenant == null)
            {

                _logger.LogError("Serch for no longer exsisting tenant");
                throw new ArgumentException("Not tenant with this name ");

            }

            var res = await _tenantPermissionRepository.GetAllByTenantIdWithFilterIgnoreAsync(Tenant.TenantId);

            return res.ToDictionary(e => e.PermissionKey, e => e.BitValue);
        }

        private async Task Loader()
        {
            _permissions = await GetAllAsMapByTenantNameWithFilterIgnoreAsync();
            _LastReloadTime = DateTime.UtcNow;

        }
    

       public  IReadOnlyDictionary<string, long> GetPermissions()
        {
            return _permissions;
        }

       public bool TryGetPermission(string key, out long permission)
        {

            return  _permissions.TryGetValue(key, out permission);

        }

       public async Task ReloadAsync(CancellationToken cancellationToken = default)
        {

         await  Loader();

        }


    }

}
