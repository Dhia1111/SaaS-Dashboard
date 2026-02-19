using Connection.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models
{
    public class DtoTenant
    {
        public int Id { get; set; }
        public int DataKey { get; set; }
        public string UniqueIdentifier { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; }=null!;

        public int PersonId { get; set; }
        public DtoPerson Person { get; set; } = null!;
    }
    public interface ITenantRepo:IGenericRepo<Tenant>
    {
      
        Task<Tenant?> GetByUniqueIdentifierWithPersonAsync(string uniqueIdentifier);
 
    }
    public class clsTenantRepo :  GenericRepo<Tenant>, ITenantRepo
    {
        public clsTenantRepo(SaasDashboardContext context, ILogger<GenericRepo<Tenant>> logger)
            : base(context,logger) {
            
        
        }

        public Task<Tenant?> GetByUniqueIdentifierAsync(string UniqueIdentifier)
        {
            try
            {
                return this._context.Tenants.SingleOrDefaultAsync(t => t.UniqueIdentifier == UniqueIdentifier);
            }
            catch(Exception ex)
            {
                this._logger.LogError(ex.Message, this);
                throw new Exception(ex.Message);
            }
        }

        public Task<Tenant?> GetByUniqueIdentifierWithPersonAsync(string uniqueIdentifier)
        {

            {
                try
                {
                    return this._context.Tenants.Include(t => t.Person).SingleOrDefaultAsync(t => t.UniqueIdentifier == uniqueIdentifier);
                }
                catch (Exception ex)
                {
                    this._logger.LogError(ex.Message, this);
                    throw new Exception(ex.Message);
                }
            }
        }

    }


}

