

using Connection.Data;
using Connection.models;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedDto_Enum;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class DtoClientSubscription {

    
    public int Id { get; set; }

    public int TenantId { get; set; }

    public bool IsFree { get; set; } = false;
    public enSubscriptionGradeStatus GradeStatus { get; set; } // Downgrade, Upgrade, Renewal,MoveToPaid, MoveToFree
    public string? Discription { get; set; }
    public string CreatedAt { get; set; } = null!;
    
    public Tenant ? Tenant { get; set; }
    public string TenantClientIdentifier { get; set; } = null!;






}

public interface IClientSubscriptionRepo:IGenericRepo<ClientSubscription> {

    Task<List<DtoPlatformCount>> GetConversionRatePerPlatform();




}


namespace Connection.models
{
    public class clsClientSubscriptionRepo:GenericRepo<ClientSubscription>,IClientSubscriptionRepo
    {
        private readonly IDiscoveryPlatformRepo _discoveryPlatformRepo;
        public clsClientSubscriptionRepo(SaasDashboardContext context,ILogger<clsClientSubscriptionRepo> logger
            ,IDiscoveryPlatformRepo discoveryPlatformRepo) : base(context, logger) { 
        
            _discoveryPlatformRepo=discoveryPlatformRepo;


        }
    

        public async Task<List<DtoPlatformCount>> GetConversionRatePerPlatform()
        {

            var result =
       await (
           from discoveryPlatform in _context.DiscoveriesPlatforms
           join clientSubscription in _context.ClientSubscriptions
               on discoveryPlatform.TenantClientIdentifier
               equals clientSubscription.TenantClientIdentifier
           group discoveryPlatform by discoveryPlatform.MarkettingPlatform into g
           select new DtoPlatformCount
           {
               Platform = ((enMarkettingPlatforms)g.Key).ToString(),
               Count = g.Count()
           })
       .ToListAsync();

            return result;
        }


    }
}
