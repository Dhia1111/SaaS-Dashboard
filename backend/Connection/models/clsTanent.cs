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
        public string UniqueIdentifier { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public string CreatedAt { get; set; }=null!;
        public string PasswordHash { get; set; } = null!;
        public string? UpdatedAt { get; set; }
        public int PersonId { get; set; }
        public int Role {  get; set; }
        public DtoPerson Person { get; set; } = null!;

        public DtoTenant() { }
        public DtoTenant( string uniqueIdentifier, string companyName, string? description, bool isActive, string createdAt, string passwordHash, int personId, DtoPerson person)
        {
            UniqueIdentifier = uniqueIdentifier;
            CompanyName = companyName;
            Description = description;
            IsActive = isActive;
            CreatedAt = createdAt;
            PasswordHash = passwordHash;
            PersonId = personId;
            Person = person;
        }
    }
    public interface ITenantRepo:IGenericRepo<Tenant>
    {
        public  Task<Tenant?> GetByEmailAsync(string email);
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

    }


}

