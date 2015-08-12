using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wivuu.DataSeed
{
    public class SeededDbContext : DbContext
    {
        public DbSet<DataMigrationHistory> __DataMigrationHistory { get; set; }
    }
}