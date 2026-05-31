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
    public interface ITenantPermissionServices: IGenericService<DtoTenantPermission>
    {

        Task<bool> IsPermissionKeyExist(string key);



    }



    public class clsTenantPermissionServices : GenericService<DtoTenantPermission, TenantPermission>, ITenantPermissionServices
    {

        private readonly ITenantPermissionRepository _tenantPermissionRepository;
        private readonly ILogger<clsTenantPermissionServices> _logger;
        public clsTenantPermissionServices(ILogger<clsTenantPermissionServices> logger, ITenantPermissionRepository repo)
               : base(repo, logger)
        {
            _tenantPermissionRepository = repo;
            _logger = logger;
        }

        protected override TenantPermission FromDto(DtoTenantPermission dto)
        {
            return new TenantPermission
            {
                Id = dto.Id,
                TenantId = dto.TenantId,
                PermissionKey = dto.PermissionKey,
                BitValue = dto.BitValue,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedAt = DateTime.Parse(dto.CreatedAt).ToUniversalTime(),
            }
            ;
        }

        protected override DtoTenantPermission ToDto(TenantPermission entity)
        {
            return new DtoTenantPermission
            {
                Id = entity.Id,
                TenantId = entity.TenantId,
                PermissionKey = entity.PermissionKey,
                BitValue = entity.BitValue,
                Description = entity.Description,
                IsActive = entity.IsActive,
                CreatedAt = entity.CreatedAt.ToString("o"),

            }
            ;
        }

        override public  async Task<int> AddAsync(DtoTenantPermission dto)
        {

               
            _logger.LogInformation("Adding new tenant permission with key: {PermissionKey}", dto.PermissionKey);

            long existingBitValue =await _tenantPermissionRepository.GetMaxBitAsync();

            int  NextPowerValue = 0 ;
            if (existingBitValue == 0)
            {
                NextPowerValue = 0;
            }
            else {

                NextPowerValue =(int)( Math.Log(existingBitValue, 2) + 1);


            }
            dto.BitValue=(long)Math.Pow(2, NextPowerValue);
            dto.CreatedAt = DateTime.Now.ToString();
            

            return await base.AddAsync(dto);
        }
       public async Task<bool> IsPermissionKeyExist(string key)
        {
           return await _tenantPermissionRepository.IsPermissionKeyExist(key);

        }



    }
}
