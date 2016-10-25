using System;
using System.Linq;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests.DataMigrations
{
    internal class AddSchools : Seed<DataSeedTestContext>
    {
        public override bool ShouldRun(DataSeedTestContext context) =>
            context.Schools.Any() == false;

        public override void Apply(DataSeedTestContext db)
        {
            var random = new Random(0x2);

            // Add "Summer Heights High" school
            var school = db.Schools.Add(new School
            {
                Id   = "summerheightshigh".ToGuid(),
                Name = "Summer Heights High"
            });

            // Add students
            school.Students.Add(new Student
            {
                Id        = "jamie.johnson".ToGuid(),
                FirstName = "Jamie",
                LastName  = "Johnson"
            });
        }
    }
}