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
        public int TenantId { get; set; }
        public string? UniqueIdentifier { get; set; }
        public string Name { get; set; }=null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public bool HaveUsedTheFreeTry { get; set; }=false;

        public string CreatedAt { get; set; }=null!;
        public string? PasswordHash { get; set; }
        public string? UpdatedAt { get; set; }
        public int PersonId { get; set; }
        public int Role {  get; set; }
        public DtoPerson? Person { get; set; } 

        public DtoTenant() { }
        
    }
    public interface ITenantRepo:IGenericRepo<Tenant>
    {
        public  Task<Tenant?> GetByEmailAsync(string email);
        Task<Tenant?> GetByUniqueIdentifierWithPersonAsync(string uniqueIdentifier);
        public Task<Tenant?> GetByNameAsync(string name);

    }
    public class clsTenantRepo : ITenantRepo
    {
        private readonly IPersonRepository _person;
        private readonly ILogger<clsTenantRepo> _logger;
        private readonly SaasDashboardContext _context;
        public clsTenantRepo(SaasDashboardContext context, ILogger<clsTenantRepo> logger,IPersonRepository person)
            {
            _person = person;
            _logger = logger;
            _context = context;
            
        
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

        public async Task<Tenant?> GetByEmailAsync(string email)
        {
            try
            {
                var person = await _context.Persons
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.Email == email);

                if (person == null) return null;

                return await _context.Tenants
                    .AsNoTracking()
                    .Include(u => u.Person)
                    .SingleOrDefaultAsync(u => u.PersonId == person.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching User by Email {Email}", email);
                throw;
            }
        }


        public async Task<Tenant?> GetByNameAsync(string name)
        {
            try
            {
                return await _context.Tenants.IgnoreQueryFilters()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.Name == name);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching User by Name {Name}", name);
                throw;
            }
        }

        private async Task<int> Add(Tenant entity)
        {
            try
            {

                await _context.AddAsync(entity);
                await _context.SaveChangesAsync();
                var idProperty = _context.Entry(entity).Property("TenantId").CurrentValue;

                return (int)idProperty;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding {Entity}", typeof(Tenant).Name);
                throw;

            }
        }

        public async Task <int> AddAsync (Tenant tenant) 
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                int personId = await _person.AddAsync(tenant.Person);

                if (personId == 0)
                    throw new  Exception("Failed to create Person for Tenant");

                tenant.PersonId = personId;

                int tenantId = await Add(tenant);

                if (tenantId == 0) throw new Exception("Failed to create Tenant");

                tenant.Person.TenantId = tenantId;
                    
                bool UpdatePersonResult = await _person.UpdateAsync(tenant.Person);
                if (!UpdatePersonResult) throw new Exception("Failed to update Person with TenantId");

                await tx.CommitAsync();

                return tenantId;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

        }

        public async Task<bool> UpdateAsync(Tenant entity)
        {
            try
            {
                _context.Update(entity);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating {Entity}", typeof(Tenant).Name);
                return false;
            }
        }

        public async Task<bool> DeleteAsync(Tenant entity)
        {
            try
            {
                _context.Remove(entity);
                var result = await _context.SaveChangesAsync();
                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting {Entity}", typeof(Tenant).Name);
                return false;
            }
        }
    
       virtual public  async  Task<IReadOnlyList<Tenant>> GetAllAsync()
        {
            try
            {
                var dbSet = _context.Set<Tenant>();
                return await dbSet.AsNoTracking().ToListAsync(); // You can add filters via expression
            }
            catch (Exception  ex)
            {
                _logger.LogError(ex, "Error fetching all {Entity}", typeof(Tenant).Name);
                throw;
            }
        }

        public async Task<Tenant?> GetByIdAsync(int id)
        {
            try
            {
                return await _context.Set<Tenant>().AsNoTracking().Include(t => t.Person).FirstOrDefaultAsync(x => x.TenantId == id);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching {Entity} by Id {Id}", typeof(Tenant).Name, id);
                throw;
            }
        }
   

        public async Task<bool> AddRangeAsync(IEnumerable<Tenant>list)
        {
            try
            {

                await _context.AddRangeAsync(list);
                return true;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "faild to add Range");
                return false;
            }


        }
        public async Task<bool> UpdateRangeAsync(IEnumerable<Tenant> list)
        {
            try
            {

                 _context.UpdateRange(list);
                await _context.SaveChangesAsync();
                return true;

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "faild to Update Range");
                return false;

            }

        }

    
    }


}

