using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

            builder.RegisterInstance(context).As<T>();
            builder.RegisterAssemblyTypes(configType.Assembly)
                .Where(t => t.BaseType == typeof(DataMigration<T>))
                .As<DataMigration<T>>();

            var container = builder.Build();
            var migrations = container.Resolve<IEnumerable<DataMigration<T>>>();

            //if (System.Diagnostics.Debugger.IsAttached == false)
            //    System.Diagnostics.Debugger.Launch();

            foreach (var migration in migrations)
            {
                if (migration.AlreadyApplied(context) == false)
                    migration.Apply(context);
            }
        }
    }
}

namespace Wivuu.DataSeed
{
    public abstract class DataMigration<T>
        where T : DbContext
    {
        public bool AlreadyApplied(T context)
        {
            return false;
        }

        public void Apply(T context)
        {
            Execute(context);
        }

        protected abstract void Execute(T context);
    }
}