using System;
using System.Collections.Generic;
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
            //var scienceDept = db.AddOrUpdate(
            //    db.Departments,
            //    new Department
            //    {
            //        Name   = "Science",
            //        School = school
            //    }, random.NextGuid());

            // Option 2
            //var scienceDept = db.AddOrUpdateEx(
            //    db.Departments,
            //    new
            //    {
            //        Name = "Science",
            //        School = school
            //    }, random.NextGuid());

            // Option 3
            var scienceDept = db.AddOrUpdate(
                db.Departments,
                new Dictionary<string, object>
                {
                    [nameof(Department.Name)]   = "Science",
                    [nameof(Department.School)] = school
                }, random.NextGuid());

            db.SaveChanges();
        }
    }
}