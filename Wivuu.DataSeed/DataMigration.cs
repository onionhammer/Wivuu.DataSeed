using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Migrations;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

namespace Wivuu.DataSeed
{
    public abstract class DataMigration<T>
        where T : DbContext
    {
        public DbMigrationsConfiguration<T> Configuration { get; set; }

        protected bool AlreadyRunResult { get; set; }

        public abstract int Order { get; }

        private string ContextKey => Configuration.GetType().FullName;

        private string MigrationId => this.GetType().Name.ToLower();

        public virtual bool AlwaysRun => false;

        protected string AssemblyPath => Path.GetDirectoryName(
            new Uri(Assembly.GetExecutingAssembly().CodeBase).AbsolutePath);

        /// <summary>
        /// Map the primitve, non-default properties of the input source 
        /// object to the input destination object
        /// </summary>
        /// <param name="destination">The destination</param>
        /// <param name="source">The source</param>
        /// <returns>The destination object</returns>
        protected TModel Map<TModel>(TModel destination, TModel source)
            where TModel : class, new()
            => Mapping.Map<TModel, TModel>(destination, source);

        /// <summary>
        /// Map the properties of the input source dictionary to the input destination object
        /// </summary>
        /// <param name="destination">The destination</param>
        /// <param name="source">The source dictionary</param>
        /// <returns>The destination object</returns>
        protected TModel Map<TModel>(TModel destination, IDictionary<string, object> source)
            where TModel : class, new()
            => Mapping.MapDictionary(destination, source);

        /// <summary>
        /// Map the properties of the input source object to the input destination object
        /// </summary>
        /// <param name="destination">The destination</param>
        /// <param name="source">The source</param>
        /// <returns>The destination object</returns>
        protected TModel MapEx<TModel, K>(TModel destination, K source)
            where TModel : class, new()
            => Mapping.Map(destination, source);

        public virtual bool AlreadyApplied(T context)
        {
            // Construct query to check for existing migrations
            var query = context.Database.SqlQuery<DataMigrationHistory>(@"
                SELECT TOP 1 MigrationId, ContextKey
                FROM dbo.__DataMigrationHistory
                WHERE MigrationId = @migrationId AND
                      ContextKey = @contextKey",
                new SqlParameter("@migrationId", this.MigrationId),
                new SqlParameter("@contextKey", this.ContextKey));

            // Execute query
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

            if (AlreadyRunResult == false)
                // Append migration history, if this was not run previously
                context.Database.ExecuteSqlCommand(@"
                    INSERT INTO dbo.__DataMigrationHistory 
                    VALUES (@migrationid, @contextKey)",
                    new SqlParameter("@migrationId", this.MigrationId),
                    new SqlParameter("@contextKey", this.ContextKey));
        }

        protected abstract void Apply(T context);
    }
}