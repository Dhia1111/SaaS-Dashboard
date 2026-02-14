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
        public virtual DbSet<Person> Persons { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;

        public virtual DbSet<Tenant>Tenants { get; set; } = null!;

        public SaasDashboardContext(DbContextOptions<SaasDashboardContext> options):base(options) {


    }

    }
}
