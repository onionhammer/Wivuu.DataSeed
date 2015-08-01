using System;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests.DataMigrations
{
    public class AddSchools : DataMigration<DataSeedTestContext>
    {
        public override int Order => 0;

        protected override void Apply(DataSeedTestContext context)
        {
            // Add "Summer Heights High" school
            var school = new School
            {
                Id   = Guid.NewGuid(),
                Name = "Summer Heights High"
            };

            context.Schools.Add(school);

            // Add students
            school.Students.Add(new Student
            {
                Id        = Guid.NewGuid(),
                FirstName = "Jamie",
                LastName  = "Johnson"
            });
        }
    }
}