namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updates7 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationDetails", "additional_hotel_name", c => c.String());
            DropColumn("dbo.ReservationDetails", "additional_shotel_name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReservationDetails", "additional_shotel_name", c => c.String());
            DropColumn("dbo.ReservationDetails", "additional_hotel_name");
        }
    }
}
