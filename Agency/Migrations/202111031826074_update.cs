namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class update : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.HotelBenefits", "hotel_id", "dbo.Hotels");
            DropIndex("dbo.HotelBenefits", new[] { "hotel_id" });
            AddColumn("dbo.HotelBenefits", "event_hotel_id", c => c.Int());
            CreateIndex("dbo.HotelBenefits", "event_hotel_id");
            AddForeignKey("dbo.HotelBenefits", "event_hotel_id", "dbo.EventHotels", "id");
            DropColumn("dbo.HotelBenefits", "hotel_id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.HotelBenefits", "hotel_id", c => c.Int());
            DropForeignKey("dbo.HotelBenefits", "event_hotel_id", "dbo.EventHotels");
            DropIndex("dbo.HotelBenefits", new[] { "event_hotel_id" });
            DropColumn("dbo.HotelBenefits", "event_hotel_id");
            CreateIndex("dbo.HotelBenefits", "hotel_id");
            AddForeignKey("dbo.HotelBenefits", "hotel_id", "dbo.Hotels", "id");
        }
    }
}
