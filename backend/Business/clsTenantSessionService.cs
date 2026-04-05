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
    public interface ITenantSessionService :IGenericService<DtoTenantSession>
    {
        public Task<DtoTenantSession?> GetByToken(string HashedToken);
        public Task<bool> UpdateIfCurrentAsync(string newHash, string OldHash, DateTime graceUntil, int SessionId);

    }

    public class clsTenantSessionService : GenericService<DtoTenantSession, TenantSession>, ITenantSessionService
    {
        private readonly IsessionRepo _repo;
        public clsTenantSessionService(IsessionRepo repo, ILogger<clsTenantSessionService> logger) : base(repo, logger)
        {
            _repo = repo;

        }
        override protected DtoTenantSession ToDto(TenantSession entity)
        {
            if (entity == null)
                return null!;

            return new DtoTenantSession
            {
                SessionId = entity.Id,
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
                LastRefreshedAt=entity.LastRefreshedAt.ToString(),
                LastRefreshedIp=entity.LastRefreshedIp
            };
        }
        protected override TenantSession FromDto(DtoTenantSession dto)
        {
            if (dto == null)
                return null!;

            return new TenantSession
            {
                Id = dto.SessionId,
                TenantId = dto.TenantId,
                GraceUntil = DateTime.Parse(dto.GraceUntil).ToUniversalTime(),
                ExpiresAt = DateTime.Parse(dto.ExpiresAt).ToUniversalTime(),
                RevokedAt = string.IsNullOrEmpty(dto.RevokedAt) ? null : DateTime.Parse(dto.RevokedAt).ToUniversalTime(),
                TenantAgent = dto.TenantAgent,
                IpAddress = dto.IpAddress,
                CurrentRefreshTokenHash = dto.CurrentRefreshTokenHash,
                PreviousRefreshTokenHash = dto.PreviousRefreshTokenHash,
                RevokedReason= dto.RevokedReason,
                RevokedByIp=dto.RevokedByIp,
                LastRefreshedIp = dto.LastRefreshedIp,
                LastRefreshedAt=DateTime.TryParse(dto.LastRefreshedAt,out DateTime lastRefreshedAt)?lastRefreshedAt.ToUniversalTime():null,
            };

        }

        public async Task<DtoTenantSession?> GetByToken(string HashedToken)
        {
            TenantSession? t = await _repo.GetByToken(HashedToken);
            return t == null ? null : ToDto(t);
        }
        public async Task<bool> UpdateIfCurrentAsync(string newHash, string OldHash, DateTime graceUntil, int SessionId)
{
            return await _repo.UpdateIfCurrentAsync(newHash,OldHash, graceUntil, SessionId);
}

    }
}
