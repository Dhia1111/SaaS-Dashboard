namespace Business.Config
{

    public interface INamingCookies
    {
        string TenantRefreshToken { get; }
        string UserRefreshToken { get; }
        string PlatformRefreshToken { get; }
    }
    public class NamingCookies: INamingCookies
    {
        public string TenantRefreshToken { get; } = "TenantRefreshToken";
        public string UserRefreshToken { get; } = "UserRefreshToken";
        public string PlatformRefreshToken { get; } = "PlatformRefreshToken";
    }
}
