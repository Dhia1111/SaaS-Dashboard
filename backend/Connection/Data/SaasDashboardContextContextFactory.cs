
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Connection.Data
{
    public class SaasDashboardContextContextFactory
        : IDesignTimeDbContextFactory<SaasDashboardContext>
    {
        public SaasDashboardContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<SaasDashboardContext>();
            string ConnectionString = "Host=localhost;Port=5431;Database=Saas-Dashboard;Username=d1111;Password=mypassword;";// ?? throw  new InvalidOperationException("Can't find Connection String");
            optionsBuilder
                .UseNpgsql(
                ConnectionString
                );

            return new SaasDashboardContext(optionsBuilder.Options);
        }
    }
}
