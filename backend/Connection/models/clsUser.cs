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
        public int Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public interface IUserRepo
    {
        Task<IReadOnlyList<User>> GetAllAsync(int dataKey);
        Task<User?> GetByIdAsync(int id);
        Task<User?> GetByPersonIdAsync(int personId);
        Task<User?> GetByEmailAsync(string email); // via Person
        Task<bool> AddAsync(User user);
        Task<bool> UpdateAsync(User user);
        Task<bool> DeleteAsync(User user);
    }

    public class clsUserRepo : IUserRepo
    {
        private readonly SaasDashboardContext _context;
        private readonly ILogger<clsUserRepo> _logger;

        public clsUserRepo(SaasDashboardContext context, ILogger<clsUserRepo> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IReadOnlyList<User>> GetAllAsync(int dataKey)
        {
            return await _context.Users
                .AsNoTracking()
                .Where(u => u.DataKey == dataKey)
                .ToListAsync();
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User?> GetByPersonIdAsync(int personId)
        {
            return await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.PersonID == personId);
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            var person = await _context.Persons
                .AsNoTracking()
                .SingleOrDefaultAsync(p => p.Email == email);

            if (person == null) return null;

            return await _context.Users
                .AsNoTracking()
                .SingleOrDefaultAsync(u => u.PersonID == person.Id);
        }

        public async Task<bool> AddAsync(User user)
        {
            _context.Users.Add(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> UpdateAsync(User user)
        {
            _context.Users.Update(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }

        public async Task<bool> DeleteAsync(User user)
        {
            _context.Users.Remove(user);
            var result = await _context.SaveChangesAsync();
            return result > 0;
        }
    }
}


