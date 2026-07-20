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
    public interface IClientSubscriptionService:IGenericService<DtoClientSubscription> {

        Task<List<DtoPlatformCount>> GetConversionRatePerPlatform();



    }

    public class clsClientSubscriptionService:GenericService<DtoClientSubscription,ClientSubscription>,IClientSubscriptionService
    {
        private readonly IClientSubscriptionRepo _clientSubscriptionRepo;

        public clsClientSubscriptionService(IClientSubscriptionRepo repo,ILogger<clsClientSubscriptionService> logger):base(repo, logger) { 
        
        _clientSubscriptionRepo = repo;
        
        }

        protected override ClientSubscription FromDto(DtoClientSubscription dto)
        {
            return new ClientSubscription
            {
                Id = dto.Id,
                IsFree = dto.IsFree,
                Discription = dto.Discription,
                GradeStatus = (int)dto.GradeStatus,
                TenantId = dto.TenantId,
                CreatedAt = DateTime.TryParse(dto.CreatedAt, out var createdAt) ? createdAt : throw new ArgumentException("Invalid CreatedAt value in DTO"),
                TenantClientIdentifier=dto.TenantClientIdentifier,
               
                


            };
        }

        protected override DtoClientSubscription ToDto(ClientSubscription entity)
        {
            return new DtoClientSubscription
            {
                CreatedAt = entity.CreatedAt.ToString(),
                Discription = entity.Discription,
                GradeStatus = (enSubscriptionGradeStatus)entity.GradeStatus,
                Id = entity.Id,
                IsFree = entity.IsFree  ,
                TenantId = entity.TenantId,
                TenantClientIdentifier=entity.TenantClientIdentifier,




            };
        }

      public async  Task<List<DtoPlatformCount>> GetConversionRatePerPlatform()
        {
            return await _clientSubscriptionRepo.GetConversionRatePerPlatform();
        }
    }
}
