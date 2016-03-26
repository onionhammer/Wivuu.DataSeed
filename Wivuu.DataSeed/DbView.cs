using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Wivuu.DataSeed;

namespace Wivuu.DataSeed
{
    public class DbView<K, T> : IQueryable<T>, IDbAsyncEnumerable<T>
        where K : DbContext, new()
    {
        protected IQueryable<T> Query { get; }

        protected IDbAsyncEnumerable<T> AsyncQuery { get; }

        public DbViewBuilder<K> Builder { get; }

        public DbView(DbViewBuilder<K> builder, Func<K, IQueryable<T>> query)
        {
            this.Builder    = builder;
            this.Query      = query(builder.Db);
            this.AsyncQuery = this.Query as IDbAsyncEnumerable<T>;
        }

        #region IQueryable<T> & IDbAsyncEnumerable<T> Implementations

        public Expression Expression 
            => Query.Expression;

        public Type ElementType 
            => Query.ElementType;

        public IQueryProvider Provider 
            => Query.Provider;

        public IEnumerator<T> GetEnumerator() 
            => Query.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() 
            => Query.GetEnumerator();

        public IDbAsyncEnumerator<T> GetAsyncEnumerator() 
            => AsyncQuery.GetAsyncEnumerator();

        IDbAsyncEnumerator IDbAsyncEnumerable.GetAsyncEnumerator() 
            => AsyncQuery.GetAsyncEnumerator();

        #endregion
    }

    public class DbViewBuilder<K> : IDisposable
        where K : DbContext, new()
    {
        internal K Db { get; }

        internal DbViewBuilder(K db)
        {
            // Views should not be updatable - do not track changes
            db.Configuration.ProxyCreationEnabled = false;

            this.Db = db;
        }

        /// <summary>
        /// Generate a view from the input query
        /// </summary>
        public DbView<K, T> View<T>(Func<K, IQueryable<T>> query) =>
            new DbView<K, T>(this, query);

        #region Resouce Management

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
                Db.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DbViewBuilder()
        {
            Dispose(false);
        }

        #endregion
    }
}

namespace System.Data.Entity
{
    public static class DbViewBuilder
    {
        /// <summary>
        /// Creates a new DbViewBuilder which is a factory for stored
        /// queries of unbound objects. Lazy loading and proxy generation
        /// is disabled by default
        /// </summary>
        public static DbViewBuilder<K> ViewBuilder<K>(this K db)
            where K : DbContext, new() =>
            new DbViewBuilder<K>(db);
    }
}