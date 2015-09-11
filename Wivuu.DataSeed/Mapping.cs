using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Wivuu.DataSeed
{
    public static class Mapping
    {
        private static Dictionary<Tuple<Type, bool>, object> _selfMappers
            = new Dictionary<Tuple<Type, bool>, object>();

        /// <summary>
        /// Map the source to the destination
        /// </summary>
        /// <returns>The destination</returns>
        public static T Map<T>(T destination, T source, bool mapAll)
            where T : class, new()
        {
            var type = typeof(T);
            if (destination == null)
                destination = new T();

            var key = Tuple.Create(type, mapAll);

            object mappingBox;
            Action<T, T> mapping;
            if (!_selfMappers.TryGetValue(key, out mappingBox))
            {
                // Create Mapping logic
                mapping = CreateMap(source, mapAll);
                _selfMappers[key] = mapping;
            }
            else
                mapping = mappingBox as Action<T, T>;

            mapping(destination, source);
            return destination;
        }

        /// <summary>
        /// Map the source to the destination
        /// </summary>
        /// <returns>The destination</returns>
        public static T Map<T>(T destination, object source)
            where T : class, new()
        {
            var type = source.GetType();
            if (destination == null)
                destination = new T();

            var key = Tuple.Create(type, true);

            object mappingBox;
            Action<T, object> mapping;
            if (!_selfMappers.TryGetValue(key, out mappingBox))
            {
                // Create Mapping logic
                mapping = CreateMap(destination, source);
                _selfMappers[key] = mapping;
            }
            else
                mapping = mappingBox as Action<T, object>;

            mapping(destination, source);
            return destination;
        }

        /// <summary>
        /// Map the source dictionary to the destination
        /// </summary>
        /// <returns>The destination</returns>
        public static T MapDictionary<T>(T dest, IDictionary<string, object> value)
            where T : class, new()
        {
            var type  = typeof(T);
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .ToDictionary(t => t.Name);

            foreach (var pair in value)
            {
                PropertyInfo prop;
                if (props.TryGetValue(pair.Key, out prop) == false)
                    continue;

                prop.SetValue(dest, pair.Value);
            }

            return dest;
        }

        private static Action<T, T> CreateMap<T>(T value, bool mapAll)
        {
            var owner = typeof(T);
            var props = owner.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            ParameterExpression
                destination = Expression.Parameter(owner),
                source      = Expression.Parameter(owner);

            var variables   = new List<ParameterExpression>(capacity: props.Length);
            var expressions = new List<Expression>(capacity: props.Length);

            // Loop through properties and assign them one by one
            for (var i = 0; i < props.Length; ++i)
            {
                var prop     = props[i];
                var propType = prop.PropertyType;

                if (mapAll)
                {
                    var get = Expression.Call(source, prop.GetMethod);
                    var set = Expression.Call(destination, prop.SetMethod, get);

                    expressions.Add(set);
                }
                else
                {
                    if (ShouldCopy(propType) == false)
                        continue;

                    // Create copy 
                    var cached = Expression.Variable(propType);
                    variables.Add(cached);

                    var doAssign = Expression.Call(destination, prop.SetMethod, cached);
                    var test     = Expression.NotEqual(cached, Expression.Default(propType));

                    expressions.Add(Expression.Assign(
                        cached, Expression.Call(source, prop.GetMethod)));
                    expressions.Add(Expression.IfThen(
                        test, doAssign));
                }
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

        private static Action<T, K> CreateMap<T, K>(T destValue, K sourceValue)
        {
            var destType    = typeof(T);
            var sourceType  = sourceValue.GetType();
            var destProps   = destType.GetProperties(BindingFlags.Public | BindingFlags.Instance).ToDictionary(t => t.Name.ToLower());
            var sourceProps = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            ParameterExpression
                destination = Expression.Parameter(destType),
                source      = Expression.Parameter(typeof(K));

            var sourceT = Expression.Variable(sourceType);
            var variables = new List<ParameterExpression>(capacity: sourceProps.Length)
            {
                sourceT
            };

            var expressions = new List<Expression>(capacity: sourceProps.Length)
            {
                Expression.Assign(sourceT, Expression.Convert(source, sourceType))
            };

            // Loop through properties and assign them one by one
            for (var i = 0; i < sourceProps.Length; ++i)
            {
                var prop     = sourceProps[i];
                var propType = prop.PropertyType;

                // Check if this matches a destination property
                PropertyInfo destProp;
                if (destProps.TryGetValue(prop.Name.ToLower(), out destProp) == false)
                    continue;

                expressions.Add(
                    // dest.set_Prop =
                    Expression.Call(destination, destProp.SetMethod, 
                    // source.get_Prop`()
                    Expression.Call(sourceT, prop.GetMethod)));
            }

            // Build body of lambda
            var body = Expression.Block(
                variables,
                expressions
            );

            var action = Expression.Lambda<Action<T, K>>(
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
}