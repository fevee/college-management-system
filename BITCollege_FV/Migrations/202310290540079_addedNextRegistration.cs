namespace BITCollege_FV.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addedNextRegistration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.NextRegistrations",
                c => new
                    {
                        NextUniqueNumberId = c.Int(nullable: false, identity: true),
                        NextAvailableNumber = c.Long(nullable: false),
                    })
                .PrimaryKey(t => t.NextUniqueNumberId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.NextRegistrations");
        }
    }
}
