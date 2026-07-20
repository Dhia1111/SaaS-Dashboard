using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connection;
using Connection.models.Entites;
using Microsoft.Extensions.Logging;

namespace Business
{
    public interface ITenantPlanBenifestServices : IGenericService<DtoTenantPlanBenefit>
    {


        public TenantPlanBenefit GetEntity(DtoTenantPlanBenefit dto);
        public DtoTenantPlanBenefit GetDto(TenantPlanBenefit entity);




    }



    public class clsTenantPlanBenifestServices : GenericService<DtoTenantPlanBenefit, TenantPlanBenefit>, ITenantPlanBenifestServices {

        private readonly ITenantPlanBenifestRepository _tenantPlanBenifestRepository;
        private readonly ILogger<clsTenantPlanBenifestServices> _logger;
        public clsTenantPlanBenifestServices(ILogger<clsTenantPlanBenifestServices> logger, 
            ITenantPlanBenifestRepository repo) 
               : base(repo, logger)
        {        
             _tenantPlanBenifestRepository = repo;
                _logger = logger;
        }

        protected override TenantPlanBenefit FromDto(DtoTenantPlanBenefit dto)
        {
            return new TenantPlanBenefit       
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                TenantPlanId = dto.TenantPlanId,
                Title = dto.Title,
                Description = dto.Description,
                
               
                

            }
            ;
        }

        protected override DtoTenantPlanBenefit ToDto(TenantPlanBenefit entity)
        {
            return new DtoTenantPlanBenefit 
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                TenantPlanId = entity.TenantPlanId,
                Title = entity.Title,
                Description = entity.Description,
                


            }
            ;
        }
        public TenantPlanBenefit GetEntity(DtoTenantPlanBenefit dto)
        {
            return  this.FromDto(dto);
            
        }
        public DtoTenantPlanBenefit GetDto(TenantPlanBenefit entity)
        {
            return ToDto(entity);
        }

    }


}
