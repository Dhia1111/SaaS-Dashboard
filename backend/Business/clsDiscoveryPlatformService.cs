using Connection.models;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;
using SharedDto_Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IDiscoveryPlatformService:IGenericService<DtoDiscoveryPlatform> {
        
        public Task<List<DtoPlatformCount>> PlatformsDiscoveryContrubutionAsync();



    }

    public class clsDiscoveryPlatformService:GenericService<DtoDiscoveryPlatform,DiscoveryPlatform>,IDiscoveryPlatformService
    {
        private readonly IDiscoveryPlatformRepo _discoveryPlatformRepo;
        public clsDiscoveryPlatformService(IDiscoveryPlatformRepo repo,ILogger<clsDiscoveryPlatformService> logger):base(repo, logger) { 
        
        _discoveryPlatformRepo = repo;
        
        }

        protected override DiscoveryPlatform FromDto(DtoDiscoveryPlatform dto)
        {
            return new DiscoveryPlatform
            {
                Id=dto.Id,
                MarkettingPlatform=(int)dto.MarkettingPlatform,
                TenantClientIdentifier=dto.TenantClientIdentifier,
                TenantId=dto.TenantId,
                
                


            };
        }

        protected override DtoDiscoveryPlatform ToDto(DiscoveryPlatform entity)
        {
            return new DtoDiscoveryPlatform
            {
                Id = entity.Id,
                MarkettingPlatform = (enMarkettingPlatforms)entity.MarkettingPlatform,
                TenantClientIdentifier = entity.TenantClientIdentifier,
                TenantId = entity.TenantId,





            };
        }

            public async Task<List<DtoPlatformCount>> PlatformsDiscoveryContrubutionAsync()
        {
            return await _discoveryPlatformRepo.PlatformsDiscoveryContrubutionAsync();
        }
    }

    }
