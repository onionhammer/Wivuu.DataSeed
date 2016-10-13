using System;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq.Expressions;
using Wivuu.DataSeed;

namespace System.Data.Entity
{
    public static class DbEntityExtensions
    {
        /// <summary>
        /// Create an UpdateSet to change one or more properties 
        /// of the input entity
        /// </summary>
        public static UpdateSet<T> UpdateSet<T>(this DbContext db, T entity)
            where T : class => 
            new UpdateSet<T>(db, entity);

        /// <summary>
        /// Specifically update a single property
        /// </summary>
        public static UpdateSet<T> Set<T, K>(this DbContext db, 
            T entity, Expression<Func<T, K>> change, K value)
            where T : class => 
            new UpdateSet<T>(db, entity).Set(change, value);

        /// <summary>
        /// Create a new entity of the specified type
        /// </summary>
        public static UpdateSet<T> New<T>(this DbContext db)
            where T : class =>
            new UpdateSet<T>(db, db.Set<T>().Create());

        /// <summary>
        /// Create a new entity of the specified type
        /// </summary>
        public static T New<T>(this DbContext db, T entity)
            where T : class
        {
            db.Entry(entity).State = EntityState.Added;

            return entity;
        }

        /// <summary>
        /// Remove entity 
        /// </summary>
        public static DbContext Remove<T>(this DbContext db, T entity)
            where T : class
        {
            var entry = db.Entry(entity);

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.State = EntityState.Detached;
                    return db;

                default:
                    entry.State = EntityState.Deleted;
                    return db;
            }
        }
    }
}

namespace Wivuu.DataSeed
{
    public struct UpdateSet<T>
        where T : class
    {
        private readonly DbContext Db;
        private readonly DbEntityEntry<T> Entry;

        /// <summary>
        /// Retrieve underlying entity
        /// </summary>
        public DbEntityEntry<T> Entity => Entry;

        internal UpdateSet(DbContext db, T entity)
        {
            this.Db    = db;
            this.Entry = Db.Entry(entity);

            // TODO - Determine if matching entity already exists in set
            if (Entry.State == EntityState.Detached)
                Db.Set<T>().Attach(entity);
        }

        /// <summary>
        /// Specifically update a single property
        /// </summary>
        public UpdateSet<T> Set<K>(Expression<Func<T, K>> change, K value)
        {
            var entry          = Entry.Property(change);
            entry.CurrentValue = value;
            entry.IsModified   = true;
            return this;
        }
    }
}