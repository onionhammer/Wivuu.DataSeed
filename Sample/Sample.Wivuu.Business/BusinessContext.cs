using System;
using Sample.Wivuu.Domain;

namespace Sample.Wivuu.Business
{
    public sealed class BusinessContext : IDisposable
    {
        internal MyDbContext Db { get; }

        public UserFormRepository UserForms { get; }

        public BusinessContext()
        {
            Db        = new MyDbContext();
            UserForms = new UserFormRepository(this);
        }

        public void Dispose()
        {
            Db.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}