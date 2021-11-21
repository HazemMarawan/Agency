namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class location : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Locations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        description = c.String(),
                        distance = c.String(),
                        mapLink = c.String(),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                        city_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Cities", t => t.city_id)
                .Index(t => t.city_id);
            
            CreateTable(
                "dbo.HotelLocations",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                        location_id = c.Int(),
                        hotel_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Hotels", t => t.hotel_id)
                .ForeignKey("dbo.Locations", t => t.location_id)
                .Index(t => t.location_id)
                .Index(t => t.hotel_id);
            
            AddColumn("dbo.Events", "location_id", c => c.Int());
            CreateIndex("dbo.Events", "location_id");
            AddForeignKey("dbo.Events", "location_id", "dbo.Locations", "id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Events", "location_id", "dbo.Locations");
            DropForeignKey("dbo.HotelLocations", "location_id", "dbo.Locations");
            DropForeignKey("dbo.HotelLocations", "hotel_id", "dbo.Hotels");
            DropForeignKey("dbo.Locations", "city_id", "dbo.Cities");
            DropIndex("dbo.HotelLocations", new[] { "hotel_id" });
            DropIndex("dbo.HotelLocations", new[] { "location_id" });
            DropIndex("dbo.Locations", new[] { "city_id" });
            DropIndex("dbo.Events", new[] { "location_id" });
            DropColumn("dbo.Events", "location_id");
            DropTable("dbo.HotelLocations");
            DropTable("dbo.Locations");
        }
    }
}
