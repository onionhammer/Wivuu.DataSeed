using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        }
    }
}