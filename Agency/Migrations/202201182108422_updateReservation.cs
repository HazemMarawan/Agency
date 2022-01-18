namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateReservation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "reservation_avg_price_before_tax", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "reservation_avg_price_before_tax");
        }
    }
}
