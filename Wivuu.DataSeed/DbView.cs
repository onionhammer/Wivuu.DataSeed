using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Wivuu.DataSeed;

namespace Wivuu.DataSeed
{
    public class DbView<T>
    {
        private readonly Func<ObjectContext, IQueryable<T>> Query;

        protected DbView(Expression<Func<ObjectContext, IQueryable<T>>> query)
        {
            this.Query = CompiledQuery.Compile(query);
        }

        public static DbView<T> AsView<K>(Expression<Func<K, IQueryable<T>>> query)
            where K : DbContext
        {
            //var param = Expression.Parameter(typeof(ObjectContext));
            var param = query.Parameters.FirstOrDefault();

            var resultExpr = Expression.Lambda<Func<ObjectContext, IQueryable<T>>>(
                query.Body,
                param
            );

            return new DbView<T>(resultExpr);
        }

        public static DbView<T> AsView(Expression<Func<ObjectContext, IQueryable<T>>> query) =>
            new DbView<T>(query);

        public IQueryable<T> From<K>(K db)
            where K : DbContext
        {
            var adapter = db as IObjectContextAdapter;
            db.Configuration.ProxyCreationEnabled = false;
            return this.Query.Invoke(adapter.ObjectContext);
        }
    }
}

namespace System.Data.Entity
{
    public static class DbView
    {
        public static DbView<T> New<K,T>(this Expression<Func<K, IQueryable<T>>> query)
            where K : DbContext => 
            DbView<T>.AsView(query);

        public static DbView<T> AsView<T>(this Expression<Func<ObjectContext, IQueryable<T>>> query) => 
            DbView<T>.AsView(query);
    }
}
