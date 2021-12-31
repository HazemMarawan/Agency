namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addInitialTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.InitialReservations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        reservation_id = c.Int(),
                        reservations_officer_name = c.String(),
                        reservations_officer_phone = c.String(),
                        reservations_officer_email = c.String(),
                        payment_type = c.Int(nullable: false),
                        credit_card_number = c.String(),
                        security_code = c.String(),
                        card_expiration_date = c.String(),
                        total_amount = c.Double(),
                        total_amount_after_tax = c.Double(),
                        paid_amount = c.Double(),
                        total_amount_from_vendor = c.Double(),
                        advance_reservation_percentage = c.Double(),
                        tax = c.Double(),
                        tax_amount = c.Double(),
                        financial_advance = c.Double(),
                        financial_due = c.Double(),
                        status = c.Int(),
                        single_price = c.Double(),
                        double_price = c.Double(),
                        triple_price = c.Double(),
                        quad_price = c.Double(),
                        currency = c.Int(),
                        opener = c.Int(),
                        closer = c.Int(),
                        vendor_single_price = c.Double(),
                        vendor_douple_price = c.Double(),
                        vendor_triple_price = c.Double(),
                        vendor_quad_price = c.Double(),
                        total_rooms = c.Int(),
                        total_nights = c.Int(),
                        check_in = c.DateTime(),
                        check_out = c.DateTime(),
                        profit = c.Double(),
                        shift = c.Int(),
                        is_canceled = c.Int(),
                        active = c.Int(),
                        financial_advance_date = c.DateTime(),
                        financial_due_date = c.DateTime(),
                        balance_due_date = c.DateTime(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                        company_id = c.Int(),
                        vendor_id = c.Int(),
                        event_hotel_id = c.Int(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.InitialReservations");
        }
    }
}
