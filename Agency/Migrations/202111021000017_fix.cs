namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HotelFacilities",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        description = c.String(),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                        hotel_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Hotels", t => t.hotel_id)
                .Index(t => t.hotel_id);
            
            AddColumn("dbo.Hotels", "rate", c => c.Double());
            AddColumn("dbo.Events", "tax", c => c.Double());
            AddColumn("dbo.Events", "advance_reservation_percentage", c => c.Double());
            DropColumn("dbo.EventHotels", "rate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.EventHotels", "rate", c => c.Double(nullable: false));
            DropForeignKey("dbo.HotelFacilities", "hotel_id", "dbo.Hotels");
            DropIndex("dbo.HotelFacilities", new[] { "hotel_id" });
            DropColumn("dbo.Events", "advance_reservation_percentage");
            DropColumn("dbo.Events", "tax");
            DropColumn("dbo.Hotels", "rate");
            DropTable("dbo.HotelFacilities");
        }
    }
}
