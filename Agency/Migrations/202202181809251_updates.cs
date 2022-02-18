namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updates : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InitialReservations", "hotel_name", c => c.String());
            AddColumn("dbo.InitialReservations", "credit", c => c.Double());
            AddColumn("dbo.InitialReservations", "refund", c => c.Double());
            AddColumn("dbo.InitialReservations", "refund_id", c => c.String());
            AddColumn("dbo.InitialReservations", "cancelation_fees", c => c.Double());
            AddColumn("dbo.InitialReservations", "is_refund", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.InitialReservations", "is_refund");
            DropColumn("dbo.InitialReservations", "cancelation_fees");
            DropColumn("dbo.InitialReservations", "refund_id");
            DropColumn("dbo.InitialReservations", "refund");
            DropColumn("dbo.InitialReservations", "credit");
            DropColumn("dbo.InitialReservations", "hotel_name");
        }
    }
}
