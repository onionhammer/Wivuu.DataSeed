using System;
using System.Linq;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests.DataMigrations
{
    public class AddClasses : DataMigration<DataSeedTestContext>
    {
        public override bool AlwaysRun => true;

        public override int Order => 2;

        protected override void Apply(DataSeedTestContext db)
        {
            var random      = new Random(0x1);
            var scienceDept = db.Departments.Single(d => d.Name == "Science");

            // Add classes
            db.Classes.Add(new Class
            {
                Id         = random.NextGuid(),
                Name       = "Biology 101",
                Department = scienceDept
            });

            db.Classes.Add(new Class
            {
                Id         = random.NextGuid(),
                Name       = "Physics 201",
                Department = scienceDept
            });
        }

        public override void Cleanup(DataSeedTestContext context)
        {
            // Remove classes
            foreach (var course in context.Classes)
                context.Classes.Remove(course);
        }
    }
}