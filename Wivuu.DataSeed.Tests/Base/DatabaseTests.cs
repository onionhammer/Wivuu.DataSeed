using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.Entity;
using Wivuu.DataSeed.Tests.Domain;
using Wivuu.DataSeed.Tests.Migrations;

namespace Wivuu.DataSeed.Tests
{
    [TestClass]
    public class TestInitialization
    {
        [AssemblyInitialize]
        public static void AssemblyInit(TestContext context)
        {
            Database.SetInitializer(
                new MigrateDatabaseToLatestVersion<DataSeedTestContext, Configuration>());
        }
    }

    public abstract class DatabaseTests
    {
        #region Properties

        protected DataSeedTestContext Db { get; set; }

        protected DbContextTransaction Transaction { get; set; }

        #endregion

        #region Tests

        public abstract void TestSetup();

        #endregion

        #region Setup

        [TestInitialize]
        public void TestStart()
        {
            this.Db          = new DataSeedTestContext();
            this.Transaction = Db.Database.BeginTransaction();

            this.TestSetup();

            this.Db.SaveChanges();
        }

        #endregion

        #region Teardown

        [TestCleanup]
        public void TestEnd()
        {
            Transaction.Rollback();
            this.Db.Dispose();
        }

        #endregion
    }
}