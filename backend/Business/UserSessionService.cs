using Connection.models;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    public interface IUserSessionService : IGenericService<DtoUserSession>
    {
        public Task<DtoUserSession?> GetByToken(string HashedToken);
        public Task<bool> UpdateIfCurrentAsync(string newHash, string OldHash, DateTime graceUntil, int SessionId);
        public Task<DtoUserSession?> GetByTenantId(int TenantId);   
    }

    public class clsUserSessionService : GenericService<DtoUserSession, UserSession>, IUserSessionService
    {
        private readonly IUserSessionRepo _repo;
        public clsUserSessionService(IUserSessionRepo repo, ILogger<clsUserSessionService> logger) : base(repo, logger)
        {
            _repo = repo;

        }
        override protected DtoUserSession ToDto(UserSession entity)
        {
            if (entity == null)
                return null!;

            return new DtoUserSession
            {
                SessionId = entity.Id,
                UserId = entity.UserId,     
                TenantId = entity.TenantId,
                GraceUntil = entity.GraceUntil.ToUniversalTime().ToString(),
                ExpiresAt = entity.ExpiresAt.ToUniversalTime().ToString(),
                RevokedAt = entity.RevokedAt?.ToUniversalTime().ToString(),
                TenantAgent = entity.TenantAgent,
                IpAddress = entity.IpAddress,
                CurrentRefreshTokenHash = entity.CurrentRefreshTokenHash,
                PreviousRefreshTokenHash = entity.PreviousRefreshTokenHash,
                RevokedByIp = entity.RevokedByIp,
                RevokedReason = entity.RevokedReason,
                LastRefreshedAt = entity.LastRefreshedAt.ToString(),
                LastRefreshedIp = entity.LastRefreshedIp,
                
            };
        }
        protected override UserSession FromDto(DtoUserSession dto)
        {
            if (dto == null)
                return null!;

            return new UserSession  
            {
                Id = dto.SessionId,
                UserId = dto.UserId,
                TenantId = dto.TenantId,
                GraceUntil = DateTime.Parse(dto.GraceUntil).ToUniversalTime(),
                ExpiresAt = DateTime.Parse(dto.ExpiresAt).ToUniversalTime(),
                RevokedAt = string.IsNullOrEmpty(dto.RevokedAt) ? null : DateTime.Parse(dto.RevokedAt).ToUniversalTime(),
                TenantAgent = dto.TenantAgent,
                IpAddress = dto.IpAddress,
                CurrentRefreshTokenHash = dto.CurrentRefreshTokenHash,
                PreviousRefreshTokenHash = dto.PreviousRefreshTokenHash,
                RevokedReason = dto.RevokedReason,
                RevokedByIp = dto.RevokedByIp,
                LastRefreshedIp = dto.LastRefreshedIp,
                LastRefreshedAt = DateTime.TryParse(dto.LastRefreshedAt, out DateTime lastRefreshedAt) ? lastRefreshedAt.ToUniversalTime() : null,
                

            };

        }

        public async Task<DtoUserSession?> GetByToken(string HashedToken)
        {
            UserSession? t = await _repo.GetByToken(HashedToken);
            return t == null ? null : ToDto(t);
        }
        public async Task<bool> UpdateIfCurrentAsync(string newHash, string OldHash, DateTime graceUntil, int SessionId)
        {
            return await _repo.UpdateIfCurrentAsync(newHash, OldHash, graceUntil.ToUniversalTime(), SessionId);
        }
        public async Task<DtoUserSession?> GetByTenantId(int TenantId)
        {
            UserSession? t = await _repo.GetByTenantId(TenantId);
            return t == null ? null : ToDto(t);

        }
    }
}