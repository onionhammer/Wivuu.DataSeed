using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Sample.Wivuu.Domain;
using Sample.Wivuu.Domain.Models;
using Wivuu.DataSeed;

namespace Sample.Wivuu.Business
{
    public class UserFormRepository
    {
        protected BusinessContext Context { get; }

        protected DbView<MyDbContext, UserForm> ActiveForms { get; }

        internal UserFormRepository(BusinessContext context)
        {
            this.Context = context;

            var db      = Context.Db;
            var builder = db.ViewBuilder();

            // Build views
            this.ActiveForms = builder.View(d => 
                from form in db.UserForms
                where form.IsActive
                select form);
        }
        
        /// <summary>
        /// Retrieves active user forms by birth date
        /// </summary>
        public IQueryable<UserForm> GetFormsByBirthDate(DateTime bornAfter) =>
            from form in ActiveForms
            where form.DateOfBirth > bornAfter
            select form;

        /// <summary>
        /// Submit a new form
        /// </summary>
        public bool UpdateForm(UserForm form)
        {
            if (form == null)
                throw new ArgumentNullException($"{nameof(form)} cannot be null");

            // Validate input
            if (form.Id == Guid.Empty) return false;
            if (form.DateOfBirth < new DateTime(1900, 1, 1)) return false;

            var db = Context.Db;

            // Only save changes to email & date of birth
            db.UpdateProps(form, 
                nameof(form.Email), 
                nameof(form.DateOfBirth));

            return true;
        }

        /// <summary>
        /// Sets the input id as active
        /// </summary>
        public async Task SetActive(Guid id, bool active)
        {
            var db   = Context.Db;
            var form = await db.UserForms.FindAsync(id);

            db.UpdateSet(form)
              .Set(i => i.IsActive, active);
        }

        /// <summary>
        /// Find form by id
        /// </summary>
        public async Task<UserForm> Find(Guid id) => 
            await ActiveForms.FirstOrDefaultAsync(f => f.Id == id);
    }
}
