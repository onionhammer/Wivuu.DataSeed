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
    }
}