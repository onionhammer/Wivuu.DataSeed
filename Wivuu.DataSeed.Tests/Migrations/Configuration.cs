namespace Wivuu.DataSeed.Tests.Migrations
{
    using System.Data.Entity.Migrations;
    using Wivuu.DataSeed.Tests.Domain;

    internal sealed class Configuration : DbMigrationsConfiguration<Wivuu.DataSeed.Tests.Domain.DataSeedTestContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(DataSeedTestContext context)
        {
            this.Execute(context);
        }
    }
}
