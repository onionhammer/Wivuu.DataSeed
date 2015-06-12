using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests
{
    [TestClass]
    public class DatabaseTests
    {
        [TestMethod]
        public void TestConnection()
        {
            using (var db = new DataSeedTestContext())
            {
                var all = db.Students.ToList();

                Assert.IsNotNull(all);
            }
        }
    }
}
