namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_fields : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "is_canceled", c => c.Int());
            AddColumn("dbo.ReservationDetails", "confirmation_id", c => c.Int());
            AddColumn("dbo.ReservationDetails", "cancelation_policy", c => c.String());
            AddColumn("dbo.ReservationDetails", "payment_to_vendor_deadline", c => c.DateTime());
            AddColumn("dbo.ReservationDetails", "paid_to_vendor_date", c => c.DateTime());
            AddColumn("dbo.ReservationDetails", "is_canceled", c => c.Int());
            AddColumn("dbo.ReservationDetails", "notify", c => c.Int());
            AddColumn("dbo.ReservationDetails", "paid_to_vendor", c => c.Int());
            AddColumn("dbo.ReservationDetails", "amount_paid_to_vendor", c => c.Double());
            AddColumn("dbo.ReservationDetails", "payment_to_vendor_notification_date", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationDetails", "payment_to_vendor_notification_date");
            DropColumn("dbo.ReservationDetails", "amount_paid_to_vendor");
            DropColumn("dbo.ReservationDetails", "paid_to_vendor");
            DropColumn("dbo.ReservationDetails", "notify");
            DropColumn("dbo.ReservationDetails", "is_canceled");
            DropColumn("dbo.ReservationDetails", "paid_to_vendor_date");
            DropColumn("dbo.ReservationDetails", "payment_to_vendor_deadline");
            DropColumn("dbo.ReservationDetails", "cancelation_policy");
            DropColumn("dbo.ReservationDetails", "confirmation_id");
            DropColumn("dbo.Reservations", "is_canceled");
        }
    }
}
