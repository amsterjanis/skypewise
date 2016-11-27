namespace SkypeWise.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCommit : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.IntentEntityPairs",
                c => new
                    {
                        Id = c.Long(nullable: false, identity: true),
                        Intent_Intent = c.String(),
                        Intent_Score = c.Double(nullable: false),
                        Entity_Entity = c.String(),
                        Entity_Type = c.String(),
                        Entity_StartIndex = c.Int(nullable: false),
                        Entity_EndIndex = c.Int(nullable: false),
                        Entity_Score = c.Double(nullable: false),
                        ChannelId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.IntentEntityPairs");
        }
    }
}
