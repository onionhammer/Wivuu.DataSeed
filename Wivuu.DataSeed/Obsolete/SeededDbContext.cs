using System;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;

namespace Wivuu.DataSeed
{
    [Obsolete("Use new BaseMigration class for your migrations")]
    public class SeededDbContext : DbContext
    {
        #region Properties

        internal DbSet<DataMigrationHistory> __DataMigrationHistory { get; set; }

        #endregion

        #region Constructors

        public SeededDbContext(string nameOrConnectionString) : base(nameOrConnectionString) { }
        public SeededDbContext(DbConnection existingConnection, bool contextOwnsConnection) : base(existingConnection, contextOwnsConnection) { }
        public SeededDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext) : base(objectContext, dbContextOwnsObjectContext) { }
        public SeededDbContext(string nameOrConnectionString, DbCompiledModel model) : base(nameOrConnectionString, model) { }
        public SeededDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection) : base(existingConnection, model, contextOwnsConnection) { }
        protected SeededDbContext() : base() { }
        protected SeededDbContext(DbCompiledModel model) : base(model) { }
     
        #endregion
    }
}