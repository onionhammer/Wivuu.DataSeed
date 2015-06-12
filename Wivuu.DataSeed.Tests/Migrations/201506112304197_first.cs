namespace Wivuu.DataSeed.Tests.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Classes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                        School_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Schools", t => t.School_Id)
                .Index(t => t.School_Id);
            
            CreateTable(
                "dbo.Students",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        FirstName = c.String(),
                        LastName = c.String(),
                        School_Id = c.Guid(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Schools", t => t.School_Id)
                .Index(t => t.School_Id);
            
            CreateTable(
                "dbo.Schools",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Student_Class",
                c => new
                    {
                        Student_Id = c.Guid(nullable: false),
                        Class_Id = c.Guid(nullable: false),
                    })
                .PrimaryKey(t => new { t.Student_Id, t.Class_Id })
                .ForeignKey("dbo.Students", t => t.Student_Id, cascadeDelete: true)
                .ForeignKey("dbo.Classes", t => t.Class_Id, cascadeDelete: true)
                .Index(t => t.Student_Id)
                .Index(t => t.Class_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Students", "School_Id", "dbo.Schools");
            DropForeignKey("dbo.Classes", "School_Id", "dbo.Schools");
            DropForeignKey("dbo.Student_Class", "Class_Id", "dbo.Classes");
            DropForeignKey("dbo.Student_Class", "Student_Id", "dbo.Students");
            DropIndex("dbo.Student_Class", new[] { "Class_Id" });
            DropIndex("dbo.Student_Class", new[] { "Student_Id" });
            DropIndex("dbo.Students", new[] { "School_Id" });
            DropIndex("dbo.Classes", new[] { "School_Id" });
            DropTable("dbo.Student_Class");
            DropTable("dbo.Schools");
            DropTable("dbo.Students");
            DropTable("dbo.Classes");
        }
    }
}
