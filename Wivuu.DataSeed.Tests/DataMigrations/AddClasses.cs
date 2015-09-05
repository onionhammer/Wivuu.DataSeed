using System;
using System.Linq;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests.DataMigrations
{
    public class AddClasses : DataMigration<DataSeedTestContext>
    {
        public override bool AlwaysRun => true;

        public override int Order => 1;

        protected override void Apply(DataSeedTestContext context)
        {
            var random = new Random(0x1);
            var school = context.Schools.FirstOrDefault();

            // Add classes
            context.Classes.Add(new Class
            {
                Id     = random.NextGuid(),
                Name   = "Biology 101",
                School = school
            });

            context.Classes.Add(new Class
            {
                Id     = random.NextGuid(),
                Name   = "Physics 201",
                School = school
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