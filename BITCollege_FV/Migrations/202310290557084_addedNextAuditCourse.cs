namespace BITCollege_FV.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedNextAuditCourse : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NextAuditCourses",
                c => new
                    {
                        NextUniqueNumberId = c.Int(nullable: false, identity: true),
                        NextAvailableNumber = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.NextUniqueNumberId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NextAuditCourses");
        }
    }
}
