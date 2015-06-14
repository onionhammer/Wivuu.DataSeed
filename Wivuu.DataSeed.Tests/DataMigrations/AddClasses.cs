using System;
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

        public override int Order
        {
            get { return 1; }
        }
    }
}