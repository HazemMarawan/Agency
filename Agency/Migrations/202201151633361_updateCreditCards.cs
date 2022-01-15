namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCreditCards : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReservationCreditCards",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        credit_card_number = c.String(),
                        security_code = c.String(),
                        card_expiration_date = c.String(),
                        reservation_id = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Reservations", t => t.reservation_id)
                .Index(t => t.reservation_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReservationCreditCards", "reservation_id", "dbo.Reservations");
            DropIndex("dbo.ReservationCreditCards", new[] { "reservation_id" });
            DropTable("dbo.ReservationCreditCards");
        }
    }
}
