using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using Autofac;
using Wivuu.DataSeed;

namespace System.Data.Entity.Migrations
{
    public static class SeedManagerExtensions
    {
        public static void Execute<T>(this DbMigrationsConfiguration<T> config, T context)
            where T : DbContext
        {
            var builder    = new ContainerBuilder();
            var configType = config.GetType();

            builder.RegisterAssemblyTypes(configType.Assembly)
                .Where(t => t.BaseType == typeof(DataMigration<T>))
                .As<DataMigration<T>>().PropertiesAutowired();

            builder.RegisterAssemblyTypes(configType.Assembly)
                .Where(t => t.BaseType == typeof(DbMigrationsConfiguration<T>))
                .As<DbMigrationsConfiguration<T>>();

            using (var container = builder.Build())
            {
                var migrations = container.Resolve<IEnumerable<DataMigration<T>>>();

                foreach (var migration in migrations)
                {
                    if (migration.AlreadyApplied(context) == false || migration.AlwaysRun)
                        migration.ApplyInternal(context);
                }
            }
        }
    }
}

namespace Wivuu.DataSeed
{
    public abstract class DataMigration<T>
        where T : DbContext
    {
        public DbMigrationsConfiguration<T> Configuration { get; set; }

        protected bool AlreadyRunResult { get; set; }

        private string MigrationId
        {
            get { return this.GetType().Name.ToLower(); }
        }

        private string ContextKey
        {
            get { return Configuration.GetType().FullName; }
        }

        public virtual bool AlwaysRun
        {
            get { return false; }
        }

        public virtual bool AlreadyApplied(T context)
        {
            // Check history
            var query = context.Database.SqlQuery(
                typeof(DataMigrationHistory), @"
                SELECT TOP 1 MigrationId, ContextKey
                FROM dbo.__DataMigrationHistory
                WHERE MigrationId = @migrationId AND
                      ContextKey = @contextKey",
                new SqlParameter("@migrationId", this.MigrationId),
                new SqlParameter("@contextKey", this.ContextKey));

            var results = query.ToListAsync().Result;

            foreach (var result in results)
            {
                // Perform cleanup
                Cleanup(context);
                AlreadyRunResult = true;
                return true;
            }

            AlreadyRunResult = false;
            return false;
        }

        public virtual void Cleanup(T context)
        {
            /* Implemented as-needed */
        }

        internal void ApplyInternal(T context)
        {
            Apply(context);

            // Append migration history, if this was not run previously
            if (AlreadyRunResult == false)
                context.Database.ExecuteSqlCommand(
                    "INSERT INTO dbo.__DataMigrationHistory VALUES (@migrationid, @contextKey)",
                    new SqlParameter("@migrationId", this.MigrationId),
                    new SqlParameter("@contextKey", this.ContextKey)
                );
        }

        protected abstract void Apply(T context);
    }

    public class DataMigrationHistory
    {
        public string MigrationId { get; set; }

        public string ContextKey { get; set; }
    }

    public class InitialDataMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.__DataMigrationHistory",
                c => new
                    {
                        MigrationId = c.String(nullable: false, maxLength: 150),
                        ContextKey  = c.String(nullable: false)
                    })
                .Index(c => c.MigrationId)
                .PrimaryKey(c => c.MigrationId);
        }

        public override void Down()
        {
            DropIndex("dbo.__DataMigrationHistory", new[] { "MigrationId" });
            DropTable("dbo.__DataMigrationHistory");
        }
    }
}