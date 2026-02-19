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
    public class DtoUser
    {
        public int Id { get; set; }
        public int DataKey { get; set; }
        public int PersonID { get; set; }
        public DtoPerson Person { get; set; } = null!;
        public int Role { get; set; }
        public string CreatedAt { get; set; } = null!;
        public string? UpdatedAt { get; set; }=null!;
    }

    public interface IUserRepo:IGenericRepo<User>
    {
        Task<User?> GetByPersonIdAsync(int personId);
        Task<User?> GetByEmailAsync(string email); // via Person
        
    }


    public class clsUserRepo : GenericRepo<User>, IUserRepo
    {

        public clsUserRepo(SaasDashboardContext context, ILogger<clsUserRepo> logger)
            : base(context, logger) { 
          
        }
        

        public override async Task<IReadOnlyList<User>> GetAllAsync(int dataKey)
        {
            try
            {
                return await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Person) // include related Person entity
                    .Where(u => u.DataKey == dataKey)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all Users for DataKey {DataKey}", dataKey);
                throw;
            }
        }

      
        public async Task<User?> GetByPersonIdAsync(int personId)
        {
            try
            {
                return await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Person)
                    .SingleOrDefaultAsync(u => u.PersonID == personId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching User by PersonId {PersonId}", personId);
                throw;
            }
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            try
            {
                var person = await _context.Persons
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.Email == email);

                if (person == null) return null;

                return await _context.Users
                    .AsNoTracking()
                    .Include(u => u.Person)
                    .SingleOrDefaultAsync(u => u.PersonID == person.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching User by Email {Email}", email);
                throw;
            }
        }

      
    }



}

