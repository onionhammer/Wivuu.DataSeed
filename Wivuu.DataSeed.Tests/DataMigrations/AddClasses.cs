using System;
using System.Linq;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests.DataMigrations
{
    public class AddClasses : DataMigration<DataSeedTestContext>
    {
        protected override void Apply(DataSeedTestContext context)
        {
            // Add classes
            context.Classes.Add(new Class
            {
                Id   = Guid.NewGuid(),
                Name = "Biology 101"
            });

            context.Classes.Add(new Class
            {
                Id   = Guid.NewGuid(),
                Name = "Physics 201"
            });
        }

        public override void Cleanup(DataSeedTestContext context)
        {
            // Remove classes
            foreach (var course in context.Classes)
                context.Classes.Remove(course);
        }

        public override bool AlwaysRun
        {
            get { return true; }
        }

        public override int Order
        {
            get { return 1; }
        }
    }
}