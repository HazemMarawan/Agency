namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tasks : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReservationTasks",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        task_title = c.String(),
                        task_detail = c.String(),
                        status = c.Int(nullable: false),
                        task_to_user = c.Int(nullable: false),
                        notification_date = c.DateTime(),
                        due_date = c.DateTime(),
                        parent_id = c.Int(),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                        reservation_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Reservations", t => t.reservation_id)
                .Index(t => t.reservation_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReservationTasks", "reservation_id", "dbo.Reservations");
            DropIndex("dbo.ReservationTasks", new[] { "reservation_id" });
            DropTable("dbo.ReservationTasks");
        }
    }
}
