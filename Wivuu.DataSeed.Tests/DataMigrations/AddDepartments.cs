using System;
using System.Linq;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests.DataMigrations
{
    public class AddDepartments : DataMigration<DataSeedTestContext>
    {
        public override bool AlwaysRun => true;

        public override int Order => 1;

        protected override void Apply(DataSeedTestContext db)
        {
            var random      = new Random(0x3);
            var school      = db.Schools.First();

            // Option 1
            //var scienceDept = Map(
            //    db.Departments.SingleOrDefault(d => d.Name == "Science") ??
            //    new Department { Id = random.NextGuid() },
            //    new Department
            //    {
            //        Name   = "Science 2",
            //        School = school
            //    });

            // Option 2
            //var scienceDept = db.Departments
            //    .MapToExisting(random.NextGuid())
            //    .Source(() => new Department
            //    {
            //        Name   = "Science 2",
            //        School = school
            //    });

            // Option 3
            var scienceDept = db.AddOrUpdateEx(
                db.Departments,
                new
                {
                    Name   = "Science",
                    School = school
                }, random.NextGuid());

            db.SaveChanges();
        }
    }
}