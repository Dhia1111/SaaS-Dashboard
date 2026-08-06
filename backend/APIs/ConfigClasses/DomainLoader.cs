using APIs.Controllers;
using Business;
using Business.Config;
using Connection;
using Connection.models;
using Microsoft.Extensions.Options;

namespace APIs.ConfigClasses
{
    public class DomainLoader:IDomainsLoader
    {

        private DateTime _LastReloadTime;
        private static Dictionary<string, int> _Domains=new();
        private readonly IDomainRepo _domainRepo;
        private ILogger<DomainLoader> _logger;
        public DateTime LastReloadTime { get { return _LastReloadTime; } }

        public DomainLoader(
        ILogger<DomainLoader> logger,
        IDomainRepo domainRepo
           

         )
        {

            _logger = logger;
            _LastReloadTime = DateTime.UtcNow;
            _domainRepo = domainRepo;

        }

       
     
        private async Task Loader()
        {
            var list = await  _domainRepo.GetAllAsync();
            _Domains = list.ToDictionary(e => e.Name, e => e.TenantId);
            _LastReloadTime = DateTime.UtcNow;

        }
    

       public  IReadOnlyDictionary<string, int> DomainsAsync()
        {
            return _Domains;
        }

       public bool TryGetTenantId(string DomainName, out int TenantId)
        {

            return  _Domains.TryGetValue(DomainName, out TenantId);

        }

       public async Task ReloadAsync(CancellationToken cancellationToken = default)
        {

         await  Loader();

        }


    }

}
