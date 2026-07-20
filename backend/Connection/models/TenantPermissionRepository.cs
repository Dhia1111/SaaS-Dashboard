
using Connection.Data;
using Connection.models;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.ComponentModel.DataAnnotations;


public interface ITenantPermissionRepository : IGenericRepo<TenantPermission>
{
    public Task<long> GetMaxBitAsync();
    Task<bool> IsPermissionKeyExist(string key);
    Task<List<TenantPermission>> GetAllByTenantIdWithFilterIgnoreAsync(int tenantId);



}
public class DtoTenantPermission
    {
        public int Id { get; set; }

        public int TenantId { get; set; }

    [RegularExpression(@"^[a-zA-Z0-9_]*$", ErrorMessage = "No spaces or special characters")]
    public string PermissionKey { get; set; } = string.Empty;

        public long BitValue { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public string CreatedAt { get; set; } = string.Empty;
    }

namespace Connection
{

    public class TenantPermissionRepository : GenericRepo<TenantPermission>, ITenantPermissionRepository
{
    public TenantPermissionRepository(
        SaasDashboardContext context,
        ILogger<TenantPermissionRepository> logger)
        : base(context, logger)
    {

         

    }

        public async Task<long> GetMaxBitAsync()
        {
            var maxBit = await _context.TenantsPermissions.MaxAsync(tp => (long?)tp.BitValue) ?? 0;
            return maxBit;
        }
        public async Task<bool> IsPermissionKeyExist(string key)
        {
            var v = await _context.TenantsPermissions.FirstOrDefaultAsync(tp => tp.PermissionKey == key);
            return v != null;
        }

        public async Task<List<TenantPermission>> GetAllByTenantIdWithFilterIgnoreAsync(int tenantId)
        {

            var res = await _context.TenantsPermissions.IgnoreQueryFilters().Where(e => e.TenantId == tenantId).ToListAsync();
            return res;

        }

    }
}