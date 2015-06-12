using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests.DataMigrations
{
    public class AddSchools : DataMigration<DataSeedTestContext>
    {
        protected override void Execute(DataSeedTestContext context)
        {
            // Add "Summer Heights High" school
            var school = new School
            {
                Id = Guid.NewGuid(),
                Name = "Summer Heights High"
            };

            context.Schools.Add(school);

            // Add students
            school.Students.Add(new Student
            {
                Id = Guid.NewGuid()
            });
        }
    }
}