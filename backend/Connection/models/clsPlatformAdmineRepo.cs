using Connection.Data;
using Connection.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedDto_Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public interface IPlatformAdmineRepo:IGenericRepo<PlatformAdmine>
{

    Task<PlatformAdmine?> GetByTenantIdAsync(int TenantId);



}


public class DtoPlatformAdmine {
    public int Id { get; set; } 
    public int TenantId { get; set; } 
    public enPlaformRoles PlatformRole { get; set; }
    public bool IsActive { get; set; } = default;
    public string? Identifier { get; set; }
}



namespace Connection.models
{
    internal class clsPlatformAdmineRepo:GenericRepo<PlatformAdmine>, IPlatformAdmineRepo
    {

        public clsPlatformAdmineRepo(SaasDashboardContext contex, ILogger<clsPlatformAdmineRepo> logger) : base(contex, logger)
        {

        }

        public async Task<PlatformAdmine?> GetByTenantIdAsync(int TenantId)
        {

            return await _context.PlatformAdmines.SingleOrDefaultAsync(i => i.TenantId == TenantId);

        }

    }
}
