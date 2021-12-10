namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class chatsTable1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Chats",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        from_user = c.Int(),
                        to_user = c.Int(),
                        message = c.String(),
                        created_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Chats");
        }
    }
}
