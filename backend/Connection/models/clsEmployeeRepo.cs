using Connection.Data;
using Connection.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SharedDto_Enum;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IEmployeeRepo : IGenericRepo<Employee>
{
    public Task<bool>  DeleteEmployeeWithAllConnections(Employee platformUserId);

    Task<Employee?> GetByUserIdAsync(int TenantId);



}

public class DtoEmployee { 

    public int Id { get; set; }
    public int TenantId { get; set; }
    public string? identifier { get; set; }
    public enPlaformRoles PlatformRole { get; set; }
    public int AdminstrationAuth { get; set; }
    public int UserId { get; set; }

    public bool IsActive { get; set; }

    public DtoUser? User { get; set; }
}


namespace Connection.models
{
    public class clsEmployeeRepo:GenericRepo<Employee>, IEmployeeRepo
    {
        private readonly IUserRepo _userRepo;
        private readonly IPersonRepository _personRepo;

        public clsEmployeeRepo(SaasDashboardContext context,
            ILogger<clsEmployeeRepo> logger,
            IUserRepo userRepo,
            IPersonRepository personRepository
            ):base(context, logger) {
            _userRepo = userRepo;
            _personRepo= personRepository;

        }

        public override async Task<int> AddAsync(Employee entity)
        {
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                await _context.Persons.AddAsync(entity.User.Person);
                await _context.SaveChangesAsync();
                int personId= entity.Id;

                if (personId == 0)
                    throw new Exception("Failed to create Person for Employee");

                entity.User.PersonId = personId;

                 await _context.Users.AddAsync(entity.User);
                await _context.SaveChangesAsync();

                int userId= entity.User.Id;
                if (userId == 0) throw new Exception("Failed to create User for Employee");
                entity.UserId = userId;

                await _context.Employees.AddAsync(entity);
                await _context.SaveChangesAsync();
                if (entity.Id == 0) throw new Exception("Failed to create Employee");

                await tx.CommitAsync();

                return entity.Id;
            }
            catch
            {
                await tx.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> DeleteEmployeeWithAllConnections(Employee employee)
        {

            var User = await _userRepo.GetByIdAsync(employee.UserId);
            if (User == null) throw new ArgumentException($"Could not found a User with Id {employee.UserId}");
            
            var Person = await _personRepo.GetByIdAsync(User.PersonId);
            if (Person == null) throw new ArgumentException($"Could not found a Person with Id {User.PersonId}");
            
            bool result=await _personRepo.DeleteAsync(Person);

            return result;







        }    
        public async Task<Employee?>GetByUserIdAsync(int UserId)
        {
            return await _context.Employees.SingleOrDefaultAsync(i => i.UserId == UserId);
        }
    }
}
