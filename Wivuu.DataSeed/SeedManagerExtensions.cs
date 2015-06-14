using System.Collections.Generic;
using System.Linq;
using Autofac;
using Wivuu.DataSeed;

namespace System.Data.Entity.Migrations
{
    public static class SeedManagerExtensions
    {
        public static void Execute<T>(this DbMigrationsConfiguration<T> config, T context)
            where T : DbContext
        {
            using (var transaction = context.Database.BeginTransaction())
            {
                try
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
                        var migrations = from migration in container.Resolve<IEnumerable<DataMigration<T>>>()
                                         orderby migration.Order
                                         select migration;

                        foreach (var migration in migrations)
                        {
                            if (migration.AlreadyApplied(context) == false || migration.AlwaysRun)
                                migration.ApplyInternal(context);
                        }
                    }

                    context.SaveChanges();
                    transaction.Commit();
                }
                catch
                {
                    transaction.Rollback();
                    throw;
                }
            }
        }
    }
}