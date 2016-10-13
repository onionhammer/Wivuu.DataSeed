using System;
using System.Data.Entity;
using System.IO;

namespace Wivuu.DataSeed
{
    public abstract class Seed<T>
        where T : DbContext
    {
        private string _basePath;

        /// <summary>
        /// Gets the base path of the assembly
        /// </summary>
        protected string BasePath
        {
            get
            {
                if (_basePath == null)
                { 
                    var code  = typeof(T).Assembly.CodeBase;
                    _basePath = Path.GetDirectoryName(new UriBuilder(code).Path);
                }

                return _basePath;
            }
        }

        /// <summary>
        /// Determines if the seed should run (defaults to true)
        /// </summary>
        public virtual bool ShouldRun(T context) => true;

        /// <summary>
        /// Apply the seed to the input context
        /// </summary>
        public abstract void Apply(T context);
    }
}