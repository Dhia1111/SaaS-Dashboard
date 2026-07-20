using Connection.Data;
using Connection.models.Entites;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.models
{
    public class DtoUserSession
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int TenantId { get; set; }
        public string GraceUntil { get; set; } = null!;
        public string ExpiresAt { get; set; } = null!;
        public string? RevokedAt { get; set; }
        public string? TenantAgent { get; set; }
        public string? RevokedByIp { get; set; }
        public string? RevokedReason { get; set; }
        public string? LastRefreshedAt { get; set; }
        public string? LastRefreshedIp { get; set; }
        public string? IpAddress { get; set; }
        public string CurrentRefreshTokenHash { get; set; } = null!;
        public string? PreviousRefreshTokenHash { get; set; }

    }
    public interface IUserSessionRepo : IGenericRepo<UserSession>
    {
        public Task<UserSession?> GetByToken(string Token);
        public Task<bool> UpdateIfCurrentAsync(string newHash, string OldHash, DateTime graceUntil, int SessionId);
        public Task<UserSession?> GetByTenantId(int TenantId);  
    }

    public class clsUserSessionRepo : GenericRepo<UserSession>, IUserSessionRepo
    {
        public clsUserSessionRepo(SaasDashboardContext dbContext, ILogger<clsUserSessionRepo> logger) : base(dbContext, logger)
        {

        }
        public async Task<UserSession?> GetByToken(string HashedToken)
        {
            try
            {
                return await _context.UsersSessions.IgnoreQueryFilters()
                                  .AsNoTracking().
                                  SingleOrDefaultAsync(u => u.CurrentRefreshTokenHash == HashedToken || u.PreviousRefreshTokenHash == HashedToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching {TenantSession} by CurrentRefreshTokenHash ", typeof(TenantSession).Name);
                throw;
            }
        }
        public async Task<bool> UpdateIfCurrentAsync(string newHash, string OldHash, DateTime graceUntil, int SessionId)
        {
            int n = await _context.Database.ExecuteSqlRawAsync(@$"
    UPDATE ""UsersSessions""
    SET
        ""PreviousRefreshTokenHash"" = ""CurrentRefreshTokenHash"",
        ""CurrentRefreshTokenHash"" = @p0,
        ""GraceUntil"" = @p1
    WHERE
        ""Id"" = @p2
        AND ""CurrentRefreshTokenHash"" = @p3",
  newHash, graceUntil, SessionId, OldHash);

            return n > 0;

        }
        public async Task<UserSession?> GetByTenantId(int TenantId)
        {
            try
            {
                return await _context.UsersSessions.IgnoreQueryFilters()
                                  .AsNoTracking()
                                  .SingleOrDefaultAsync(u => u.TenantId == TenantId && u.RevokedAt == null);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching {UserSession} by TenantId {TenantId}", typeof(UserSession).Name, TenantId);
                throw;
            }
        }
        public override async Task<UserSession?> GetByIdAsync(int sessionId)
        {
            try
            {
                return await _context.UsersSessions.IgnoreQueryFilters()
                                  .AsNoTracking()
                                  .SingleOrDefaultAsync(u => u.Id == sessionId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching {TenantSession} by SessionId {SessionId}", typeof(TenantSession).Name, sessionId);
                throw;
            }
        }
    }
}
