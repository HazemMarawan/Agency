namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addLogTable1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReservationLogs",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        user_id = c.Int(),
                        action = c.String(),
                        description = c.String(),
                        created_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ReservationLogs");
        }
    }
}
