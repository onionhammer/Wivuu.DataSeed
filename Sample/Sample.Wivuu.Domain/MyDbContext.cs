using System;
using System.Data.Entity;
using Sample.Wivuu.Domain.Models;

namespace Sample.Wivuu.Domain
{
    public class MyDbContext : System.Data.Entity.DbContext
    {
        public DbSet<UserForm> UserForms { get; set; }

        public MyDbContext()
        {
            this.Configuration.AutoDetectChangesEnabled = false;
            this.Configuration.ProxyCreationEnabled     = false;
        }
    }
}