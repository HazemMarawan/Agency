namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateReservationadvance_reservation_percentage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "advance_reservation_percentage", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "advance_reservation_percentage");
        }
    }
}
