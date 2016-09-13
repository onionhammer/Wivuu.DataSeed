using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Wivuu.ExprUtils;

namespace Wivuu.DataSeed
{
    [Obsolete("Mapping will be replaced with strictly typed compiled mappings")]
    public static class Mapping
    {
        private static Dictionary<StructTuple<Type, bool>, object> _selfMappers
            = new Dictionary<StructTuple<Type, bool>, object>();

        private static Dictionary<Type, object> _dictMappers
            = new Dictionary<Type, object>();

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

            var key = StructTuple.Create(type, mapAll);

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

            var key = StructTuple.Create(type, true);

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
        public static T MapDictionary<T>(T destination, IDictionary<string, object> source)
            where T : class, new()
        {
            var type = typeof(T);
            if (destination == null)
                destination = new T();

            object mappingBox;
            Action<T, IDictionary<string, object>> mapping;
            if (!_dictMappers.TryGetValue(type, out mappingBox))
            {
                // Create Mapping logic
                mapping = CreateMap(destination);
                _dictMappers[type] = mapping;
            }
            else
                mapping = mappingBox as Action<T, IDictionary<string, object>>;

            mapping(destination, source);
            return destination;
        }

        private static Action<T, IDictionary<string, object>> CreateMap<T>(T value)
        {
            var owner      = typeof(T);
            var sourceType = typeof(IDictionary<string, object>);
            var props      = owner.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            ParameterExpression
                destination = Expression.Parameter(owner),
                source      = Expression.Parameter(sourceType);

            var variables   = new List<ParameterExpression>();
            var expressions = new List<Expression>(capacity: props.Length);
            var tryGetValue = sourceType.GetMethod(nameof(IDictionary<string, object>.TryGetValue));
            var local       = Expression.Variable(typeof(object));

            variables.Add(local);

            // Loop through properties and assign them one by one
            for (var i = 0; i < props.Length; ++i)
            {
                var prop     = props[i];
                var propType = prop.PropertyType;

                // Try to get the value
                var tryGetLocal = Expression.Call(source, tryGetValue, Expression.Constant(prop.Name), local);
                var doAssign    = Expression.Call(destination, 
                    prop.SetMethod, Expression.Convert(local, prop.PropertyType));

                expressions.Add(
                    Expression.IfThen(tryGetLocal, doAssign)
                );
            }

            // Build body of lambda
            var body = Expression.Block(
                variables, 
                expressions
            );

            var action = Expression.Lambda<Action<T, IDictionary<string, object>>>(
                body, destination, source
            );

            if (owner.IsVisible)
                return ILSerializer.Compile(action);
            else
                return action.Compile();
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

            if (owner.IsValueType)
                return ILSerializer.Compile(action);
            else
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

            var sourceT   = Expression.Variable(sourceType);
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

            if (destType.IsValueType && sourceType.IsVisible)
                return ILSerializer.Compile(action);
            else
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