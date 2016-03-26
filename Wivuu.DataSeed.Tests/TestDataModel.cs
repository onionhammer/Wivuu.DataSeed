using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests
{
    [TestClass]
    public class TestDataModel : DatabaseTests
    {
        public override void TestSetup()
        {
        }

        [TestMethod]
        public void TestConnection()
        {
            var all = Db.Students.ToList();

            Assert.IsNotNull(all);
            Assert.AreEqual(1, all.Count);
        }

        [TestMethod]
        public async Task TestHasMigrations()
        {
            var query = Db.Database.SqlQuery<DataMigrationHistory>(
                "SELECT MigrationId, ContextKey FROM dbo.__DataMigrationHistory");

            var assembly = typeof(TestDataModel).Assembly;
            var types = from type in assembly.DefinedTypes
                        where type.BaseType == typeof(DataMigration<DataSeedTestContext>)
                        select type;

            var results       = await query.ToListAsync();
            var numMigrations = types.Count();

            Assert.AreEqual(numMigrations, results.Count);
        }

        [TestMethod]
        public async Task TestImmEntity()
        {
            var newEnt = new ProtectedEntity(1, "Sam");

            Db.Protected.Add(newEnt);
            await Db.SaveChangesAsync();

            // Not editable
            //newEnt.Name = "Craig";

            var extEnt = await Db.Protected
                .FirstOrDefaultAsync(p => p.Name == "Sam");
            Assert.IsNotNull(extEnt);
            Assert.AreEqual(extEnt.Age, 0);

            // Still not editable
            //extEnt.Name = "Craig";

            // Update entity (purposefully!)
            Db.UpdateSet(extEnt)
                .Set(e => e.Name, "Craig")
                .Set(e => e.Age, 15);
            await Db.SaveChangesAsync();

            extEnt = await Db.Protected
                .FirstOrDefaultAsync(p => p.Name == "Craig");

            Assert.IsNotNull(extEnt, "Unable to change entity");
            Assert.AreEqual(extEnt.Age, 15);
            await Db.SaveChangesAsync();
        }

        [TestMethod]
        public async Task TestViews()
        {
            var view = DbView.AsView(ctx =>
                from i in Db.Departments
                select i
            );

            //var view = DbView.New<DataSeedTestContext, Department>(
            //db => db.Departments);

            var all = await view.From(Db).ToListAsync();
        }
    }

    [TestClass]
    public class TestUOW
    {
        private DbView<Department> Departments;

        [TestInitialize]
        public void Setup()
        {
            using (var db = new DataSeedTestContext())
                Departments = DbView.AsView(ctx =>
                    from i in db.Departments
                    select i
                );
        }

        [TestMethod]
        public async Task TestViews()
        {
            using (var db = new DataSeedTestContext())
            {
                var all = await Departments.From(db).ToListAsync();
            }
        }
    }
}