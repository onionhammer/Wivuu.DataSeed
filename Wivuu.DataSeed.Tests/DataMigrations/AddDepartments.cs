#pragma warning disable 612, 618

using System;
using System.Collections.Generic;
using System.Linq;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests.DataMigrations
{
    internal class AddDepartments : DataMigration<DataSeedTestContext>
    {
        public override bool AlwaysRun => true;

        public override int Order => 1;

        protected override void Apply(DataSeedTestContext db)
        {
            var random        = new Random(0x3);
            var school        = db.Schools.First();
            var scienceDeptId = random.NextGuid();

            db.Departments.Find(scienceDeptId)
                .Update(new Dictionary<string, object>
                {
                    [nameof(Department.Name)]   = "Science",
                    [nameof(Department.School)] = school
                })
                .Default(() => db.Departments.Add(new Department { Id = scienceDeptId }));

            db.SaveChanges();
        }
    }
}