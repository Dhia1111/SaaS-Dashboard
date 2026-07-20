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
    public interface ITenantPricingOptionServices: IGenericService<DtoTenantPricingOption> {


        public TenantPlanPricingOption GetEntity(DtoTenantPricingOption dto);
        public DtoTenantPricingOption GetDto(TenantPlanPricingOption Entity);
        public Task< List<DtoTenantPricingOption> > GetAllPlatformPlanPricingOptionsAsync();



    }



    public class clsTenantPricingOptionServices : GenericService<DtoTenantPricingOption, TenantPlanPricingOption>, ITenantPricingOptionServices
    {

        private readonly ITenantPricingOptionRepository _tenantPricingOptionRepository;
        private readonly ILogger<clsTenantPricingOptionServices> _logger;
        private readonly PlatformInfo _platformInfo;
        private readonly ITenantService _tenantService;
      
        
        public clsTenantPricingOptionServices(ILogger<clsTenantPricingOptionServices> logger, 
            ITenantPricingOptionRepository repo,
            IOptions<PlatformInfo>platformInfo,
            ITenantService tenantService) 
               : base(repo, logger)
        {        
             _tenantPricingOptionRepository = repo;
                _logger = logger;
            _platformInfo = platformInfo.Value;
            _tenantService = tenantService;
        }

        public async Task<List<DtoTenantPricingOption>> GetAllPlatformPlanPricingOptionsAsync()
        {
            DtoTenant tenant = await _tenantService.GetByNameAsync(_platformInfo.TenantName);

            var list = await _tenantPricingOptionRepository.GetAllPlanPricingOptionsWithFilterIgnoreAsync(tenant.TenantId);
            return list.Select(e => ToDto(e)).ToList();

;        }

        protected override TenantPlanPricingOption FromDto(DtoTenantPricingOption dto)
        {
            return new TenantPlanPricingOption      
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                Price = dto.Amount,
                Currency = dto.Currency,
                 IsActive = dto.IsActive,
                TenantPlanId = dto.TenantPlanId,
                TenantPricingCycleId = dto.TenantPricingCycleId,
                


            }
            ;
        }       

        protected override DtoTenantPricingOption ToDto(TenantPlanPricingOption entity)
        {
            return new DtoTenantPricingOption
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                Amount = entity.Price,
                Currency = entity.Currency,
                 IsActive = entity.IsActive,
                TenantPlanId = entity.TenantPlanId,
                TenantPricingCycleId=entity.TenantPricingCycleId
                
            }
            ;
        }              
        
        public TenantPlanPricingOption GetEntity(DtoTenantPricingOption dto)
        {
            return this.FromDto(dto);
        }

        public DtoTenantPricingOption GetDto(TenantPlanPricingOption entity)
        {
            return ToDto(entity);

        }

            }
            
        }


    



