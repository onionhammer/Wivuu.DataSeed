using System;
using System.Collections.Generic;

namespace Wivuu.DataSeed
{
    #region State Machine

    public class StateContainer<T>
        where T : class, new()
    {
        #region Properties

        internal T Destination { get; set; }

        internal T SourceT { get; set; }

        internal Dictionary<string, object> SourceD { get; set; }

        internal object Source { get; set; }

        #endregion

        #region Constructor

        internal StateContainer() { }

        #endregion

        #region Methods

        /// <summary>
        /// Calls callback when the entity is not found
        /// </summary>
        public T Default(Func<T> callback)
        {
            if (Destination == default(T))
                Destination = callback();

            if (SourceT != null)
                return Mapping.Map(Destination, SourceT);
            else if (SourceD != null)
                return Mapping.MapDictionary(Destination, SourceD);
            else
                return Mapping.Map(Destination, Source);
        }

        #endregion
    }

    #endregion

    #region Fluent Interface

    public static class FluentExtensions
    {
        /// <summary>
        /// Starts the process of updating a record stored in this table
        /// </summary>
        public static StateContainer<T> Update<T>(this T self, T value)
            where T : class, new()
            => new StateContainer<T> { Destination = self, SourceT = value };

        /// <summary>
        /// Starts the process of updating a record stored in this table
        /// </summary>
        public static StateContainer<T> Update<T>(this T self, object value)
            where T : class, new()
            => new StateContainer<T> { Destination = self, Source = value };

        /// <summary>
        /// Starts the process of updating this entity
        /// </summary>
        public static StateContainer<T> Update<T>(this T self, Dictionary<string, object> values)
            where T : class, new()
            => new StateContainer<T> { Destination = self, Source = values };

        /// <summary>
        /// Starts the process of updating a record stored in this table
        /// </summary>
        public static T Default<T>(this T self, Func<T> callback)
            where T : class, new()
            => new StateContainer<T> { Destination = self }.Default(callback);
    }

    #endregion
}