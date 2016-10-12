namespace Sample.Wivuu.Domain.cs.Migrations
{
    using System.Data.Entity.Migrations;
    using Domain.Migrations.Data;

    internal sealed class Configuration : DbMigrationsConfiguration<MyDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(MyDbContext context)
        {
#if DEBUG
            this.Execute(context, new AddSampleForms());
#endif
        }
    }
}
