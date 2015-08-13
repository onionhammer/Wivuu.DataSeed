namespace Wivuu.DataSeed.Tests.Migrations
{
    using System.Data.Entity.Migrations;
    using Domain;

    internal sealed class Configuration : DbMigrationsConfiguration<DataSeedTestContext>
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
