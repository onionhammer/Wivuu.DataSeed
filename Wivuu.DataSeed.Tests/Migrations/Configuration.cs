#pragma warning disable 612, 618

namespace Wivuu.DataSeed.Tests.Migrations
{
    using System.Data.Entity.Migrations;
    using DataMigrations;
    using Domain;

    internal sealed class Configuration : DbMigrationsConfiguration<DataSeedTestContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DataSeedTestContext context)
        {
            // New Way
            this.Execute(context, new[] {
                new AddSchools()
            });

            // Old way
            this.Execute(context);
        }
    }
}
