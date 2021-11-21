namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createDB : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Cities",
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
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Hotels",
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
                        city_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Cities", t => t.city_id)
                .Index(t => t.city_id);
            
            CreateTable(
                "dbo.EventHotels",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        rate = c.Double(nullable: false),
                        single_price = c.Double(nullable: false),
                        double_price = c.Double(nullable: false),
                        triple_price = c.Double(nullable: false),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                        event_id = c.Int(),
                        hotel_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Events", t => t.event_id)
                .ForeignKey("dbo.Hotels", t => t.hotel_id)
                .Index(t => t.event_id)
                .Index(t => t.hotel_id);
            
            CreateTable(
                "dbo.Events",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        title = c.String(),
                        description = c.String(),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        code = c.String(),
                        user_name = c.String(),
                        full_name = c.String(),
                        email = c.String(),
                        password = c.String(),
                        phone1 = c.String(),
                        phone2 = c.String(),
                        address1 = c.String(),
                        address2 = c.String(),
                        gender = c.Int(),
                        nationality = c.String(),
                        birthDate = c.DateTime(),
                        image = c.String(),
                        type = c.Int(),
                        active = c.Int(),
                        created_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventHotels", "hotel_id", "dbo.Hotels");
            DropForeignKey("dbo.EventHotels", "event_id", "dbo.Events");
            DropForeignKey("dbo.Hotels", "city_id", "dbo.Cities");
            DropIndex("dbo.EventHotels", new[] { "hotel_id" });
            DropIndex("dbo.EventHotels", new[] { "event_id" });
            DropIndex("dbo.Hotels", new[] { "city_id" });
            DropTable("dbo.Users");
            DropTable("dbo.Events");
            DropTable("dbo.EventHotels");
            DropTable("dbo.Hotels");
            DropTable("dbo.Cities");
        }
    }
}
