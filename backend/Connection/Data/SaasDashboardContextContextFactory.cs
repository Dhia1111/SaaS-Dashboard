
using Connection.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Connection.Data
{
    public class DataKeyProvider : ITenantIdProvider
    {
       public int TenantId { get; set; }
    }

    public class SaasDashboardContextContextFactory
        : IDesignTimeDbContextFactory<SaasDashboardContext>
    {
        DataKeyProvider datakeyObj = new DataKeyProvider();
        public SaasDashboardContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SaasDashboardContext>();
            string ConnectionString = "Host=localhost;Port=5431;Database=Saas-Dashboard;Username=d1111;Password=mypassword;";// ?? throw  new InvalidOperationException("Can't find Connection String");
            optionsBuilder
                .UseNpgsql(
                ConnectionString
                );

            return new SaasDashboardContext(optionsBuilder.Options,datakeyObj );
        }
    }
}
