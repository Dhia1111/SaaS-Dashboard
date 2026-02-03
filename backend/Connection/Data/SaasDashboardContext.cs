using Connection.models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Connection.Data
{
    public class SaasDashboardContext:DbContext
    {
        public virtual DbSet<clsCustomerRepository> Customers { get; set; } = null!;

        public SaasDashboardContext(DbContextOptions<SaasDashboardContext> options):base(options) {

    }

    }
}
