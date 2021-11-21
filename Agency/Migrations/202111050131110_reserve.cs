namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reserve : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reservations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        total_amount = c.Double(),
                        tax = c.Double(),
                        financial_advance = c.Double(),
                        financial_due = c.Double(),
                        status = c.Int(),
                        single_price = c.Double(),
                        douple_price = c.Double(),
                        currency = c.Int(),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                        company_id = c.Int(),
                        event_hotel_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Companies", t => t.company_id)
                .ForeignKey("dbo.EventHotels", t => t.event_hotel_id)
                .Index(t => t.company_id)
                .Index(t => t.event_hotel_id);
            
            CreateTable(
                "dbo.Companies",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        email = c.String(),
                        phone = c.String(),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Clients",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        first_name = c.String(),
                        last_name = c.String(),
                        email = c.String(),
                        phone = c.String(),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                        company_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Companies", t => t.company_id)
                .Index(t => t.company_id);
            
            CreateTable(
                "dbo.ReservationDetails",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        amount = c.Double(),
                        room_type = c.Int(),
                        reservation_from = c.DateTime(nullable: false),
                        reservation_to = c.DateTime(nullable: false),
                        no_of_days = c.Int(),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                        client_id = c.Int(),
                        reservation_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Clients", t => t.client_id)
                .ForeignKey("dbo.Reservations", t => t.reservation_id)
                .Index(t => t.client_id)
                .Index(t => t.reservation_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReservationDetails", "reservation_id", "dbo.Reservations");
            DropForeignKey("dbo.ReservationDetails", "client_id", "dbo.Clients");
            DropForeignKey("dbo.Reservations", "event_hotel_id", "dbo.EventHotels");
            DropForeignKey("dbo.Reservations", "company_id", "dbo.Companies");
            DropForeignKey("dbo.Clients", "company_id", "dbo.Companies");
            DropIndex("dbo.ReservationDetails", new[] { "reservation_id" });
            DropIndex("dbo.ReservationDetails", new[] { "client_id" });
            DropIndex("dbo.Clients", new[] { "company_id" });
            DropIndex("dbo.Reservations", new[] { "event_hotel_id" });
            DropIndex("dbo.Reservations", new[] { "company_id" });
            DropTable("dbo.ReservationDetails");
            DropTable("dbo.Clients");
            DropTable("dbo.Companies");
            DropTable("dbo.Reservations");
        }
    }
}
