using System;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests.DataMigrations
{
    public class AddSchools : DataMigration<DataSeedTestContext>
    {
        public override int Order => 0;

        protected override void Apply(DataSeedTestContext context)
        {
            var random = new Random(0x2);

            // Add "Summer Heights High" school
            var school = context.Schools.Add(new School
            {
                Id   = random.NextGuid(),
                Name = "Summer Heights High"
            });

            // Add students
            school.Students.Add(new Student
            {
                Id        = random.NextGuid(),
                FirstName = "Jamie",
                LastName  = "Johnson"
            });

            context.SaveChanges();
        }
    }
}