  using Business;
using Connection.models;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;
 

    public interface IDomainService:IGenericService<DtoDomain>{

    Task<DtoDomain?> FindAsync(int Id);
    Task<DtoDomain?> FindAsync(string Name);

    }

  public interface IDomainsLoader
{
    IReadOnlyDictionary<string, int> DomainsAsync();

    bool TryGetTenantId(string DomainName, out int TenantId);

    Task ReloadAsync(CancellationToken cancellationToken = default);

    DateTime LastReloadTime { get; }

}

namespace Business
{
    public class clsDomainService:GenericService<DtoDomain,Domain> ,IDomainService
    {

        private readonly IDomainRepo _repo;
        private readonly ILogger<clsDomainService> _logger;
        private readonly IDomainsLoader _domainLoader;
        private readonly ITenantIdProvider _tenantIdProvider;

        public clsDomainService(IDomainRepo repo, ILogger<clsDomainService> logger, 
            IDomainsLoader domainLoader,ITenantIdProvider tenantIdProvider) : base(repo, logger)
        {
            _repo = repo;
            _logger = logger;
            _domainLoader = domainLoader;
            _tenantIdProvider = tenantIdProvider;
        }

        protected  override Domain FromDto(DtoDomain dto)
        {
            return new Domain
            {
                Id = dto.Id,
                Name = dto.Name,
                TenantId = dto.TenantId,
            };
        }

        protected override DtoDomain ToDto(Domain entity)
        {
            return new DtoDomain
            {
                Id = entity.Id,
                Name = entity.Name,
                TenantId = entity.TenantId,
            };
        }

        public async Task<DtoDomain?>FindAsync(int id)
        {

            var res=await _repo.FindAsync(id);
            return res != null ? ToDto(res) : null;

        }
       
        public async Task<DtoDomain?> FindAsync(string name) {


            var res = await _repo.FindAsync(name);

            return res != null ? ToDto(res) : null;


        }

        public override async Task<int> AddAsync(DtoDomain dto)
        {
            bool exists =  _domainLoader.TryGetTenantId(dto.Name, out int tenantId);
            dto.TenantId = _tenantIdProvider.TenantId;
            if (exists) { 
          
                throw new  Business.Exceptions.ResourceAlreadyExistsException($"Domain with name '{dto.Name}' already exists for tenant id {tenantId}.",dto.Name);
            }
            int id= await base.AddAsync(dto);
            await _domainLoader.ReloadAsync();
            return id;
        }
        
        public override async Task<bool> UpdateAsync(DtoDomain dto)
        {
            dto.TenantId = _tenantIdProvider.TenantId;
            bool res= await base.UpdateAsync(dto);
            await _domainLoader.ReloadAsync();
            return res;

        }
      
        public override async Task<bool> DeleteAsync(int id)
        {

           bool res= await base.DeleteAsync(id);
           await _domainLoader.ReloadAsync();
            return res;
        }

    }

  
}
