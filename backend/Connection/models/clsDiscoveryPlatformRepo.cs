

using Connection.Data;
using Connection.models;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedDto_Enum;

public class DtoDiscoveryPlatform {

    public int Id { get; set; }

    public int TenantId { get; set; }//PlatformOwner 

    public string TenantClientIdentifier { get; set; } = null!;// PlatformClient could be Id Guid ..

    public enMarkettingPlatforms MarkettingPlatform { get; set; }


}
public class DtoPlatformCount {

    public string Platform { get; set; } = null!;
    public int Count { get; set; }

}

public interface IDiscoveryPlatformRepo:IGenericRepo<DiscoveryPlatform> {


    public Task<List<DtoPlatformCount>> PlatformsDiscoveryContrubutionAsync();  // platform identifyer ,  





}


namespace Connection.models
{
    public class clsDiscoveryPlatformRepo:GenericRepo<DiscoveryPlatform>,IDiscoveryPlatformRepo
    {
        public clsDiscoveryPlatformRepo(SaasDashboardContext context,ILogger<clsDiscoveryPlatformRepo> logger) : base(context, logger) { 
        


        }
        public async Task<List<DtoPlatformCount>> PlatformsDiscoveryContrubutionAsync()
        {

            var res =  await _context.DiscoveriesPlatforms.GroupBy(p => p.MarkettingPlatform).Select(e => new DtoPlatformCount { Platform =((enMarkettingPlatforms) e.Key).ToString(), Count = e.Count() }).ToListAsync();

            return res;

        }


    }
}
