namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updates5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationDetails", "show_name", c => c.String());
            AddColumn("dbo.ReservationDetails", "hotel_name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationDetails", "hotel_name");
            DropColumn("dbo.ReservationDetails", "show_name");
        }
    }
}
