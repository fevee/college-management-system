namespace BITCollege_FV.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedNextUniqueNumberController : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.NextAuditCourses", newName: "NextUniqueNumbers");
            AddColumn("dbo.NextUniqueNumbers", "Discriminator", c => c.String(nullable: false, maxLength: 128));
            DropTable("dbo.NextGradedCourses");
            DropTable("dbo.NextMasteryCourses");
            DropTable("dbo.NextRegistrations");
            DropTable("dbo.NextStudents");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.NextStudents",
                c => new
                    {
                        NextUniqueNumberId = c.Int(nullable: false, identity: true),
                        NextAvailableNumber = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.NextUniqueNumberId);
            
            CreateTable(
                "dbo.NextRegistrations",
                c => new
                    {
                        NextUniqueNumberId = c.Int(nullable: false, identity: true),
                        NextAvailableNumber = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.NextUniqueNumberId);
            
            CreateTable(
                "dbo.NextMasteryCourses",
                c => new
                    {
                        NextUniqueNumberId = c.Int(nullable: false, identity: true),
                        NextAvailableNumber = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.NextUniqueNumberId);
            
            CreateTable(
                "dbo.NextGradedCourses",
                c => new
                    {
                        NextUniqueNumberId = c.Int(nullable: false, identity: true),
                        NextAvailableNumber = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.NextUniqueNumberId);
            
            DropColumn("dbo.NextUniqueNumbers", "Discriminator");
            RenameTable(name: "dbo.NextUniqueNumbers", newName: "NextAuditCourses");
        }
    }
}
