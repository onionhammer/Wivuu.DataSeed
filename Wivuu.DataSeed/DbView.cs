using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using Wivuu.DataSeed;

namespace Wivuu.DataSeed
{
    internal class DbViewVisitor<K> : ExpressionVisitor
        where K : DbContext
    {
        private readonly Expression Swap;

        public DbViewVisitor(K context)
        {
            Expression<Func<K>> param = () => context;

            this.Swap = param.Body as MemberExpression;
        }

        public override Expression Visit(Expression node)
        {
            switch (node?.NodeType)
            {
                case ExpressionType.MemberAccess:
                    var memberNode = node as MemberExpression;
                    if (memberNode.Expression.Type == typeof(K))
                        return Expression.MakeMemberAccess(Swap, memberNode.Member);
                    else
                        return base.Visit(node);

                default:
                    return base.Visit(node);
            }
        }
    }

    public class DbView<K, T>
        where K : DbContext
    {
        private readonly Expression<Func<ObjectContext, K, IQueryable<T>>> Expression;

        public DbView(Expression<Func<ObjectContext, K, IQueryable<T>>> query)
        {
            this.Expression = query;
        }

        public IQueryable<T> From(K db)
        {
            var transform = new DbViewVisitor<K>(db).Visit(Expression) as
                Expression<Func<ObjectContext, K, IQueryable<T>>>;

            var query   = CompiledQuery.Compile(transform);
            var adapter = db as IObjectContextAdapter;

            db.Configuration.ProxyCreationEnabled = false;
            return query.Invoke(adapter.ObjectContext, db);
        }
    }
}

namespace System.Data.Entity
{
    public static class DbView
    {
        public static DbView<K, T> AsView<K, T>(this Expression<Func<ObjectContext, K, IQueryable<T>>> query)
            where K : DbContext =>
            new DbView<K, T>(query);
    }
}
