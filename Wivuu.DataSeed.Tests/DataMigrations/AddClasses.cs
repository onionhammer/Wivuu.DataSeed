using System;
using System.Collections.Generic;
using System.Linq;
using Wivuu.DataSeed.Tests.Domain;

namespace Wivuu.DataSeed.Tests.DataMigrations
{
    public class AddClasses : DataMigration<DataSeedTestContext>
    {
        public override bool AlwaysRun => true;

        public override int Order => 2;

        protected override void Apply(DataSeedTestContext db)
        {
            var random      = new Random(0x1);
            var scienceDept = db.Departments.Single(d => d.Name == "Science");

            var biologyId = random.NextGuid();
            var physicsId = random.NextGuid();

            // Add classes
            db.Classes.Find(biologyId)
                .Update(new Class
                {
                    Id         = biologyId,
                    Name       = "Biology 101",
                    Department = scienceDept
                })
                .Default(c => db.Classes.Add(c));

            db.Classes.Find(physicsId)
                .Update(new Dictionary<string, object>
                {
                    ["Id"]         = physicsId,
                    ["Name"]       = "Physics 201",
                    ["Department"] = scienceDept
                })
                .Default(c => db.Classes.Add(c));
        }
    }
}