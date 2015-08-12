namespace Wivuu.DataSeed.Tests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbseed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.__DataMigrationHistory",
                c => new
                    {
                        MigrationId = c.String(nullable: false, maxLength: 128),
                        ContextKey = c.String(),
                    })
                .PrimaryKey(t => t.MigrationId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.__DataMigrationHistory");
        }
    }
}
