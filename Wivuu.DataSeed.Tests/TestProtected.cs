using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests
{
    [TestClass]
    public class TestProtected : DatabaseTests
    {
        private DbView<DataSeedTestContext, ProtectedEntity> ProtectedEntities;

        public override void TestSetup()
        {
            var testDb = new DataSeedTestContext(Db.Database.Connection);
            testDb.Database.UseTransaction(Db.Database.CurrentTransaction.UnderlyingTransaction);
            var builder = testDb.ViewBuilder();

            ProtectedEntities = builder.View(db => db.Protected);
        }

        [TestMethod]
        public async Task TestProtectedEntity()
        {
            // Create a new view
            using (var db = new DataSeedTestContext(Db.Database.Connection))
            {
                db.Database.UseTransaction(Db.Database.CurrentTransaction.UnderlyingTransaction);
                var newEnt = db.New(new ProtectedEntity(1, "Sam"));

                await db.SaveChangesAsync();

                // Not editable
                //newEnt.Name = "Craig";
            }

            // Ensure new entity was added
            var extEnt = await ProtectedEntities
                .FirstOrDefaultAsync(p => p.Name == "Sam");
            Assert.IsNotNull(extEnt);
            Assert.AreEqual(extEnt.Age, 0);

            // Still not editable
            //extEnt.Name = "Craig";

            using (var db = new DataSeedTestContext(Db.Database.Connection))
            {
                db.Database.UseTransaction(Db.Database.CurrentTransaction.UnderlyingTransaction);

                // Update entity
                db.Set(extEnt, e => e.Name, "Craig")
                  .Set(e => e.Age, 15);

                await db.SaveChangesAsync();

                extEnt = await ProtectedEntities
                    .FirstOrDefaultAsync(p => p.Name == "Craig");

                Assert.IsNotNull(extEnt, "Unable to change entity");
                Assert.AreEqual(extEnt.Age, 15);
                await db.SaveChangesAsync();
            }

            using (var db = new DataSeedTestContext(Db.Database.Connection))
            {
                db.Database.UseTransaction(Db.Database.CurrentTransaction.UnderlyingTransaction);

                // Delete entity
                db.Remove(extEnt);
                await db.SaveChangesAsync();

                extEnt = await ProtectedEntities
                    .FirstOrDefaultAsync(p => p.Name == "Craig");

                Assert.IsNull(extEnt);
            }
        }

        [TestMethod]
        public async Task TestViews()
        {
            using (var dbv = new DataSeedViews(Db))
            {
                // Create student
                var scienceDept = await dbv.ScienceDept.FirstOrDefaultAsync();
                var @class      = await dbv.Classes.Where(c => c.DepartmentId == scienceDept.Id).FirstOrDefaultAsync();

                var craig = Db.New(new Student
                {
                    Id        = Guid.NewGuid(),
                    FirstName = "Craig",
                    LastName  = "Harvey",
                    Classes   = new List<Class> { @class }
                });

                await Db.SaveChangesAsync();

                // Retrieve student
                var scienceStudents = await dbv.ScienceStudents.ToListAsync();
                Assert.IsTrue(scienceStudents.Any(c => c.Id == craig.Id));
            }
        }
    }
}