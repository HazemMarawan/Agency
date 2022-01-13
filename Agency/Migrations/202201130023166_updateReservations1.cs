namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateReservations1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "reservation_avg_price", c => c.Double());
            AddColumn("dbo.Reservations", "vendor_avg_price", c => c.Double());
            AddColumn("dbo.Reservations", "total_price", c => c.Double());
            AddColumn("dbo.InitialReservations", "reservation_avg_price", c => c.Double());
            AddColumn("dbo.InitialReservations", "vendor_avg_price", c => c.Double());
            AddColumn("dbo.InitialReservations", "total_price", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InitialReservations", "total_price");
            DropColumn("dbo.InitialReservations", "vendor_avg_price");
            DropColumn("dbo.InitialReservations", "reservation_avg_price");
            DropColumn("dbo.Reservations", "total_price");
            DropColumn("dbo.Reservations", "vendor_avg_price");
            DropColumn("dbo.Reservations", "reservation_avg_price");
        }
    }
}
