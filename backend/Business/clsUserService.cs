using Connection.models;
using Microsoft.Extensions.Logging;

namespace Business
{
    public interface IUserService : IGenericService<DtoUser>
    {
        Task<DtoUser?> GetByEmailAsync(string email);
    }

    public class clsUserService
        : GenericService<DtoUser, User>,
          IUserService
    {
        private readonly IUserRepo _userRepo;
        private readonly ILogger<clsUserService> _logger;

        public clsUserService
        (
            IUserRepo userRepo,
            ILogger<clsUserService> logger
        )
            : base(userRepo, logger)
        {
            _userRepo = userRepo;
            _logger = logger;
        }

        protected override DtoUser ToDto(User user)
        {
            return new DtoUser
            {
                Id = user.Id,
                PersonID = user.PersonId,
                Authorization = user.Authorization,
                Role = user.Role,
                CreatedAt = user.CreatedAt.ToString(),
                UpdatedAt = user.UpdatedAt?.ToString(),
                PasswordHash = user.PasswordHash,
                TenantId = user.TenantId,

                Person = user.Person == null
                    ? null
                    : new DtoPerson
                    {
                        Id = user.Person.Id,
                        Email = user.Person.Email,
                        Phone = user.Person.Phone,
                        FirstName = user.Person.FirstName,
                        LastName = user.Person.LastName,
                        Address = user.Person.Address,
                        SecureCode = user.Person.SecureCode,
                        EmailVerificationCodeExpiry =
                            user.Person.EmailVerificationCodeExpiry?.ToString(),
                        IsEmailVeryfied = user.Person.IsVeryfied,
                        Provider = user.Person.Provider,
                        ProviderId = user.Person.ProviderId
                        
                    },
                IsActive=user.IsActive
            };
        }

        protected override User FromDto(DtoUser dto)
        {
            return new User
            {
                Id = dto.Id,
                PersonId = dto.PersonID,
                Authorization = dto.Authorization,
                Role = dto.Role,
                PasswordHash = dto.PasswordHash,
                TenantId = dto.TenantId,

                CreatedAt =
                    DateTime.Parse(dto.CreatedAt).ToUniversalTime(),

                UpdatedAt =
                    dto.UpdatedAt == null
                    ? null
                    : DateTime.Parse(dto.UpdatedAt).ToUniversalTime(),

                Person = dto.Person == null
                    ? null
                    : new Person
                    {
                        Id = dto.PersonID,
                        Email = dto.Person.Email,
                        Phone = dto.Person.Phone,
                        FirstName = dto.Person.FirstName,
                        LastName = dto.Person.LastName,
                        Address = dto.Person.Address,
                        SecureCode = dto.Person.SecureCode,
                        IsVeryfied = dto.Person.IsEmailVeryfied,
                        Provider = dto.Person.Provider,
                        ProviderId = dto.Person.ProviderId,
                        TenantId = dto.TenantId
                    },
                IsActive=dto.IsActive

            };
        }

        public async Task<DtoUser?> GetByEmailAsync(string email)
        {
            var user = await _userRepo.GetByEmailAsync(email);

            return user == null
                ? null
                : ToDto(user);
        }

        public override async Task<int> AddAsync(DtoUser dto)
        {
            return await _userRepo.AddAsync(FromDto(dto));
        }

        public override async Task<bool> UpdateAsync(DtoUser dto)
        {
            return await _userRepo.UpdateAsync(FromDto(dto));
        }

        public override async Task<bool> DeleteAsync(int id)
        {
            var user = await _userRepo.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning
                (
                    "User {UserId} not found",
                    id
                );

                return false;
            }

            return await _userRepo.DeleteAsync(user);
        }
    }
}