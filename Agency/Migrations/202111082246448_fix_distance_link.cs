namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix_distance_link : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HotelLocations", "distance", c => c.String());
            AddColumn("dbo.HotelLocations", "map_link", c => c.String());
            DropColumn("dbo.Locations", "distance");
            DropColumn("dbo.Locations", "map_link");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Locations", "map_link", c => c.String());
            AddColumn("dbo.Locations", "distance", c => c.String());
            DropColumn("dbo.HotelLocations", "map_link");
            DropColumn("dbo.HotelLocations", "distance");
        }
    }
}
