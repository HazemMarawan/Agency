namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hotelimage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HotelImages",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        path = c.String(),
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HotelImages", "hotel_id", "dbo.Hotels");
            DropIndex("dbo.HotelImages", new[] { "hotel_id" });
            DropTable("dbo.HotelImages");
        }
    }
}
