using System;
using System.Data.Entity;
using System.Linq;

namespace Wivuu.DataSeed.Tests.Domain
{
    public class DataSeedViews : IDisposable
    {
        private readonly bool DbOwned;

        public readonly DataSeedTestContext Db;
        public readonly DbView<DataSeedTestContext, Class> Classes;
        public readonly DbView<DataSeedTestContext, Department> Departments;
        public readonly DbView<DataSeedTestContext, School> Schools;
        public readonly DbView<DataSeedTestContext, Student> Students;
        public readonly DbView<DataSeedTestContext, Department> ScienceDept;
        public readonly DbView<DataSeedTestContext, Student> ScienceStudents;

        public DataSeedViews(DataSeedTestContext db = null)
        {
            DbOwned = db == null;
            Db      = db ?? new DataSeedTestContext();

            var builder = Db.ViewBuilder();

            // Set up views
            Departments = builder.View(d => db.Departments);
            Classes     = builder.View(d => db.Classes);
            Schools     = builder.View(d => db.Schools);
            Students    = builder.View(d => db.Students);

            ScienceDept = Departments.View(depts =>
                from d in depts
                where d.Name == "Science"
                select d
            );

            ScienceStudents = ScienceDept.View(depts => 
                (
                    from d in depts
                    join c in Db.Classes on d.Id equals c.DepartmentId
                    select c
                ).SelectMany(c => c.Students)
            );
        }

        public void Dispose()
        {
            if (DbOwned)
                Db.Dispose();

            GC.SuppressFinalize(this);
        }
    }
}