using Connection.models.Entites;
using Microsoft.Extensions.Logging;


namespace Business
{
    public interface ITenantFreePlanService:IGenericService<DtoTenantFreePlan> {

        public TenantFreePlan GetEntity(DtoTenantFreePlan dto);
        public Task<DtoTenantFreePlan?> GetByPlanIdAsync(int  planId);

        public DtoTenantFreePlan GetDto(TenantFreePlan Entity);


    }

    public class clsTenantFreePlanService:GenericService<DtoTenantFreePlan,TenantFreePlan>,ITenantFreePlanService
    {
        private readonly ITenantFreePlanRepo _TenantFreePlanRepo;
        public clsTenantFreePlanService(ITenantFreePlanRepo repo,ILogger<clsTenantFreePlanService> logger):base(repo, logger) { 
        
        _TenantFreePlanRepo = repo;
        
        }

        protected override TenantFreePlan FromDto(DtoTenantFreePlan dto)
        {
            return new TenantFreePlan
            {
                Id=dto.Id,    
                TenantId=dto.TenantId,
                StartAt=DateTime.Parse(dto.StartAt).ToUniversalTime(),
                EndAt=DateTime.Parse(dto.EndAt).ToUniversalTime(),
                CycleId=dto.CycleId,
                TenantPlanId=dto.TenantPlanId,
                
                


            };
        }

        protected override DtoTenantFreePlan ToDto(TenantFreePlan entity)
        {
            return new DtoTenantFreePlan
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                EndAt=entity.EndAt.ToString(),
                StartAt=entity.StartAt.ToString(),
                CycleId=entity.CycleId
                ,TenantPlanId=entity.TenantPlanId,
                

                


            };
        }

        public TenantFreePlan GetEntity(DtoTenantFreePlan dto)
        {
            return FromDto(dto);
        }

        public  DtoTenantFreePlan GetDto(TenantFreePlan entity)
        {
            return ToDto(entity);
        }
        public async Task<DtoTenantFreePlan?> GetByPlanIdAsync(int planId)
        {

            var res=await _TenantFreePlanRepo.GetByPlanIdAsync(planId);

            return res != null ? ToDto(res) : null;
            

        }

    }
}
