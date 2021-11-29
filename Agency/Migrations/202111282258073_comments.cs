namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class comments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReservationComments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        comment = c.String(),
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
            DropForeignKey("dbo.ReservationComments", "reservation_id", "dbo.Reservations");
            DropIndex("dbo.ReservationComments", new[] { "reservation_id" });
            DropTable("dbo.ReservationComments");
        }
    }
}
