namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updates6 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationDetails", "additional_show_name", c => c.String());
            AddColumn("dbo.ReservationDetails", "additional_shotel_name", c => c.String());
            DropColumn("dbo.ReservationDetails", "show_name");
            DropColumn("dbo.ReservationDetails", "hotel_name");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReservationDetails", "hotel_name", c => c.String());
            AddColumn("dbo.ReservationDetails", "show_name", c => c.String());
            DropColumn("dbo.ReservationDetails", "additional_shotel_name");
            DropColumn("dbo.ReservationDetails", "additional_show_name");
        }
    }
}
