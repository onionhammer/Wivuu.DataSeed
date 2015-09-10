using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Wivuu.DataSeed
{
    public static class MapPrimitive
    {
        private static Dictionary<Type, object> _mappers
            = new Dictionary<Type, object>();

        public static T Map<T>(T destination, T source)
            where T : class, new()
        {
            var type = typeof(T);
            if (destination == null)
                destination = new T();

            object mappingBox;
            Action<T, T> mapping;
            if (!_mappers.TryGetValue(type, out mappingBox))
            {
                // Create Mapping logic
                mapping = CreateMap(source);
                _mappers[type] = mapping;
            }
            else
                mapping = mappingBox as Action<T, T>;

            mapping(destination, source);
            return destination;
        }

        private static Action<T, T> CreateMap<T>(T value)
        {
            var owner = typeof(T);
            var props = owner.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            ParameterExpression
                destination = Expression.Parameter(owner),
                source = Expression.Parameter(owner);

            var variables = new List<ParameterExpression>(capacity: props.Length);
            var expressions = new List<Expression>(capacity: props.Length);

            // Loop through properties and assign them one by one
            for (var i = 0; i < props.Length; ++i)
            {
                var prop = props[i];
                var propType = prop.PropertyType;

                if (ShouldCopy(propType) == false)
                    continue;

                // Create copy 
                var cached = Expression.Variable(propType);
                variables.Add(cached);

                var doAssign = Expression.Call(destination, prop.SetMethod, cached);
                var test = Expression.NotEqual(cached, Expression.Default(propType));

                expressions.Add(Expression.Assign(
                    cached, Expression.Call(source, prop.GetMethod)));
                expressions.Add(Expression.IfThen(
                    test, doAssign));
            }

            // Build body of lambda
            var body = Expression.Block(
                variables,
                expressions
            );

            var action = Expression.Lambda<Action<T, T>>(
                body, destination, source
            );

            return action.Compile();
        }

        private static bool ShouldCopy(Type t)
        {
            if (t.IsPrimitive)
                return true;

            reprocess:
            switch (t.Name)
            {
                case nameof(Nullable):
                case "Nullable`1":
                    t = Nullable.GetUnderlyingType(t);
                    goto reprocess;

                case "String":
                case "DateTime":
                case "DateTimeOffset":
                case "Guid":
                    return true;

                default:
                    return false;
            }
        }
    }

    public static class IDbExtensions
    {
        public static T AddOrUpdate<T>(this DbContext db, IDbSet<T> table, 
            T value, params object[] keys)
            where T : class, new()
        {
            var dest = table.Find(keys);
            if (dest == null)
            { 
                table.Add(value);

                // Map key to dest
                var objectContext = (db as IObjectContextAdapter).ObjectContext;
                var set           = objectContext.CreateObjectSet<T>();
                var keyMembers    = set.EntitySet.ElementType.KeyMembers;
                var properties    = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

                for (var i = 0; i < keys.Length; ++i)
                {
                    var member = keyMembers[i]?.Name;
                    var prop   = properties.Where(p => p.Name == member).SingleOrDefault();

                    if (prop != null)
                        prop.SetMethod.Invoke(value, new [] { keys[i] });
                }

                return value;
            }
            else 
                // Map changes
                return MapPrimitive.Map(dest, value);
        }
    }
}