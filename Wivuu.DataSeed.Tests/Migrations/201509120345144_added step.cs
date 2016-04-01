namespace Wivuu.DataSeed.Tests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedstep : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Departments", "Step", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Departments", "Step");
        }
    }
}
