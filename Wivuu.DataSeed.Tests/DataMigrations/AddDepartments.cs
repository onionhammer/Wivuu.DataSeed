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
            var scienceDept = db.Departments.SingleOrDefault(d => d.Name == "Science");

            if (scienceDept == null)
            {
                scienceDept = db.Departments.Add(new Department
                {
                    Id = random.NextGuid(),
                    Name = "Science",
                    School = school
                });

                db.SaveChanges();
            }
        }
    }
}