namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_hotel_name : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "hotel_name", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "hotel_name");
        }
    }
}
