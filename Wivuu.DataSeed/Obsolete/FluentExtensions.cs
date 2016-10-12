#pragma warning disable 612, 618

using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Wivuu.DataSeed
{
    #region State Machine

    [Obsolete("Deprecated -- Use AddOrUpdate")]
    public class StateContainer<T>
        where T : class, new()
    {
        #region Properties

        internal T Destination { get; }

        internal T SourceT { get; set; }

        private HashSet<SourceMap> Sources { get; } 
            = new HashSet<SourceMap>();

        #endregion

        #region Constructor

        internal StateContainer(T destination)
        {
            this.Destination = destination;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Starts the process of updating a record stored in this table
        /// </summary>
        public StateContainer<T> Update(T value)
        {
            Contract.Requires(SourceT == null);

            SourceT = value;
            return this;
        }

        /// <summary>
        /// Starts the process of updating a record stored in this table
        /// </summary>
        public StateContainer<T> Update(object value)
        {
            Sources.Add(new SourceMap(value));
            return this;
        }

        /// <summary>
        /// Starts the process of updating this entity
        /// </summary>
        public StateContainer<T> Update(Dictionary<string, object> values)
        {
            Sources.Add(new SourceMap(values));
            return this;
        }

        /// <summary>
        /// Calls callback when the entity is not found
        /// </summary>
        public T Default(Func<T> callback)
        {
            var destination = Destination;
            var mapAll      = false;

            if (destination == default(T))
            {
                destination = callback();
                mapAll      = true;
            }

            if (SourceT != null)
                destination = Mapping.Map(destination, SourceT, mapAll);

            foreach (var source in Sources)
                destination = source.Map(destination);

            return destination;
        }

        /// <summary>
        /// Calls callback when the entity is not found
        /// </summary>
        public T Default(Func<T, T> callback)
        {
            var destination = Destination;

            if (destination == default(T))
            {
                var source = SourceT;
                if (source == null)
                    source = new T();

                destination = callback(source);
            }
            else if (SourceT != null)
                destination = Mapping.Map(destination, SourceT, false);

            foreach (var source in Sources)
                destination = source.Map(destination);

            return destination;
        }

        #endregion
    }

    #endregion

    #region Fluent Interface

    [Obsolete("Deprecated -- Use AddOrUpdate")]
    public static class FluentExtensions
    {
        /// <summary>
        /// Starts the process of updating a record stored in this table
        /// </summary>
        public static StateContainer<T> Update<T>(this T self, T value)
            where T : class, new()
            => new StateContainer<T>(self).Update(value);

        /// <summary>
        /// Starts the process of updating a record stored in this table
        /// </summary>
        public static StateContainer<T> Update<T>(this T self, object value)
            where T : class, new()
            => new StateContainer<T>(self).Update(value);

        /// <summary>
        /// Starts the process of updating this entity
        /// </summary>
        public static StateContainer<T> Update<T>(this T self, Dictionary<string, object> values)
            where T : class, new()
            => new StateContainer<T>(self).Update(values);

        /// <summary>
        /// Starts the process of updating a record stored in this table
        /// </summary>
        public static T Default<T>(this T self, Func<T> callback)
            where T : class, new()
            => new StateContainer<T>(self).Default(callback);
    }

    #endregion

    #region Source Mapping Container

    [Obsolete("Deprecated -- Use AddOrUpdate")]
    public struct SourceMap
    {
        private Dictionary<string, object> SourceD { get; }

        private object Source { get; }

        public SourceMap(Dictionary<string, object> source)
        {
            SourceD = source;
            Source  = null;
        }

        public SourceMap(object source)
        {
            SourceD = null;
            Source  = source;
        }

        public T Map<T>(T destination) where T : class, new()
            => SourceD != null 
             ? Mapping.MapDictionary(destination, SourceD)
             : Mapping.Map(destination, Source);

        public override int GetHashCode()
            => SourceD != null
             ? SourceD.GetHashCode()
             : Source.GetHashCode();
    }

    #endregion
}