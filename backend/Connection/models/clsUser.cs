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
         public int PersonID { get; set; }
        public DtoPerson? Person { get; set; } 
        public int Role { get; set; }
        public string CreatedAt { get; set; } = null!;
        public string? UpdatedAt { get; set; }
        public string?PasswordHash { get; set; }
        public int TenantId { get; set; }
        public int Authorization { get; set; }
    }

    public interface IUserRepo:IGenericRepo<User>
    {
        Task<User?> GetByPersonIdAsync(int personId);
        Task<User?> GetByEmailAsync(string email); // via Person
        Task<User?> GetByEmailWithTenantIdUnSafeAsync(string email, int tenantId); 

    }


    public class clsUserRepo : GenericRepo<User>, IUserRepo
    {
        private readonly IPersonRepository _person;
        public clsUserRepo(SaasDashboardContext context, ILogger<clsUserRepo> logger,
            IPersonRepository person)
            : base(context, logger) { 

            _person= person;

        }
        

        public override async Task<IReadOnlyList<User>> GetAllAsync()
        {
            try
            {
                return await _context.Users
                    .AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all Users " );
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
                    .SingleOrDefaultAsync(u => u.PersonId == personId);
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
                    .SingleOrDefaultAsync(u => u.PersonId == person.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching User by Email {Email}", email);
                throw;
            }
        }

        public async Task<User?> GetByEmailWithTenantIdUnSafeAsync(string email,int  TenantId)
        {
            try
            {
                var person = await _context.Persons.IgnoreQueryFilters()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(p => p.Email == email && p.TenantId == TenantId);

                if (person == null) return null;

                var user = await _context.Users.IgnoreQueryFilters()
                    .AsNoTracking()
                    .SingleOrDefaultAsync(u => u.PersonId == person.Id && u.TenantId == TenantId);
                if (user == null) return null;
                // Manually set the Person navigation property
                // due to IgnoreQueryFilters
                user.Person = person;
                return user;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching User by Email {Email}", email);
                throw;
            }
        }


        override public async Task<int> AddAsync(User user)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                int personId = await _person.AddAsync(user.Person);

                if (personId == 0)
                    throw new Exception("Failed to create Person for User");

                user.PersonId = personId;

                int userId = await base.AddAsync(user);

                if (userId == 0) throw new Exception("Failed to create User");

                await tx.CommitAsync();

                return userId;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

        }

        override public async Task<bool> UpdateAsync(User user)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                bool personUpdate = await _person.UpdateAsync(user.Person);

                if (!personUpdate)
                    throw new Exception("Failed to update Person for User");

                bool UpdateUser = await base.UpdateAsync(user);

                if (!UpdateUser) throw new Exception("Failed to update User");

                await tx.CommitAsync();

                return UpdateUser;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }

        }


    }



}

