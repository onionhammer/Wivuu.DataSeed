using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wivuu.DataSeed
{
    public class InitialDataMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.__DataMigrationHistory",
                c => new
                {
                    MigrationId = c.String(nullable: false, maxLength: 150),
                    ContextKey  = c.String(nullable: false)
                })
                .Index(c => c.MigrationId)
                .PrimaryKey(c => c.MigrationId);
        }

        public override void Down()
        {
            DropIndex("dbo.__DataMigrationHistory", new[] { "MigrationId" });
            DropTable("dbo.__DataMigrationHistory");
        }
    }
}
