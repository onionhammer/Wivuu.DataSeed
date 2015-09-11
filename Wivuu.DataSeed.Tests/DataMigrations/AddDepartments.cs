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
            var random        = new Random(0x3);
            var school        = db.Schools.First();
            var scienceDeptId = random.NextGuid();

            db.Departments.Find(scienceDeptId)
                .Update(new
                {
                    Name   = "Science",
                    School = school
                })
                .Default(() => db.Departments.Add(new Department { Id = scienceDeptId }));

            db.SaveChanges();
        }
    }
}