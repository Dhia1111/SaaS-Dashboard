using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.EndToEndService
{
    public interface IRefreshTokenService
    {
        public string CookieName { get; }
        Task<DtoTokens?> RefreshTokensAsync(
            string? ipAddress,
            DtoTokens tokens);
    }

}
