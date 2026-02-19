// Business/clsUserService.cs
using Connection.models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Business
{
    public interface IUserService 
    {
        Task<DtoUser?> GetByEmailAsync(string email);
    }


    public class clsUserService : GenericService<DtoUser, User>, IUserService
    {
        private readonly IUserRepo _userRepo; // contains person-aware methods
        private readonly IPersonRepository _personRepo;
        private readonly ILogger<clsUserService> _typedLogger;

        public clsUserService(
            IUserRepo userRepo,
            IPersonRepository personRepo,
            ILogger<clsUserService> logger)

            : base(userRepo, logger)
        {
            _userRepo = userRepo;
            _personRepo = personRepo;
            _typedLogger = logger;
        }

        protected override DtoUser ToDto(User user)
        {
            return new DtoUser
            {
                Id = user.Id,
                DataKey = user.DataKey,
                PersonID = user.PersonID,
                Person = user.Person != null ? new DtoPerson
                {
                    Id = user.Person.Id,
                    DataKey = user.Person.DataKey,
                    Email = user.Person.Email,
                    Phone = user.Person.Phone,
                    FirstName = user.Person.FirstName,
                    LastName = user.Person.LastName,
                    Address = user.Person.Adress
                } : null!,
                Role = user.Role,
                CreatedAt = user.CreatedAt.ToLongDateString(),
                UpdatedAt = user.UpdatedAt!=null? user.UpdatedAt.ToString():null,
            };
        }

        protected override User FromDto(DtoUser dto)
        {
            // Validate person exists
            var personTask = _personRepo.GetByIdAsync(dto.PersonID);
            personTask.Wait(); // small sync call - prefer async flows in controllers
            var person = personTask.Result;
            if (person == null)
            {
                _typedLogger.LogWarning("Person {PersonId} not found when mapping User DTO", dto.PersonID);
                throw new KeyNotFoundException($"Person {dto.PersonID} not found.");
            }

            return new User
            {
                Id = dto.Id,
                DataKey = dto.DataKey,
                PersonID = dto.PersonID,
                Role = dto.Role,
                CreatedAt = DateTime.Parse(dto.CreatedAt) ,
                UpdatedAt = dto.UpdatedAt!=null?DateTime.Parse(dto.UpdatedAt):null,
            };
        }

        public async Task<DtoUser?> GetByEmailAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);
            return user != null ? ToDto(user) : null;
        }

        // Override Add/Update if you want special checks
        public override async Task<bool> AddAsync(DtoUser dto)
        {
            // explicit mapping will validate existence of Person
            var user = FromDto(dto);
            return await _userRepo.AddAsync(user);
        }

        public override async Task<bool> UpdateAsync(DtoUser dto)
        {
            var user = FromDto(dto);
            return await _userRepo.UpdateAsync(user);
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var existing = await _userRepo.GetByIdAsync(id);
            if (existing == null)
            {
                _typedLogger.LogWarning("User id {Id} not found for delete", id);
                return false;
            }
            return await _userRepo.DeleteAsync(existing);
        }
    }

}
