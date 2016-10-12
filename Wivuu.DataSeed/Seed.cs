using System.Data.Entity;

namespace Wivuu.DataSeed
{
    public abstract class Seed<T>
        where T : DbContext
    {
        public virtual bool ShouldRun(T context) => true;

        public abstract void Apply(T context);
    }
}