using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wivuu.DataSeed.Tests.Domain
{
    public class DataSeedTestContext : SeededDbContext
    {
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            // Set up Student <-> Class relationship
            modelBuilder.Entity<Student>()
                .HasMany(s => s.Classes)
                .WithMany(c => c.Students)
                .Map(join => join
                    .MapLeftKey("Student_Id")
                    .MapRightKey("Class_Id")
                    .ToTable("Student_Class"));

            // Ensure Names are not-null
            modelBuilder.Entity<School>().Property(s => s.Name).IsRequired();
            modelBuilder.Entity<Student>().Property(s => s.FirstName).IsRequired();
            modelBuilder.Entity<Student>().Property(s => s.LastName).IsRequired();
            modelBuilder.Entity<Class>().Property(p => p.Name).IsRequired();
            modelBuilder.Entity<Department>().Property(p => p.Name).IsRequired();
        }

        public DbSet<School> Schools { get; set; }

        public DbSet<Class> Classes { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Department> Departments { get; set; }
    }
}