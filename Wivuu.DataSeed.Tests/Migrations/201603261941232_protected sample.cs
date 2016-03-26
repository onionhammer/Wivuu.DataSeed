namespace Wivuu.DataSeed.Tests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class protectedsample : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProtectedEntities",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Age = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ProtectedEntities");
        }
    }
}
