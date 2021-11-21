namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class currency : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventHotels", "currency", c => c.Int());
            AlterColumn("dbo.EventHotels", "single_price", c => c.Double());
            AlterColumn("dbo.EventHotels", "single_available_rooms", c => c.Int());
            AlterColumn("dbo.EventHotels", "double_price", c => c.Double());
            AlterColumn("dbo.EventHotels", "double_available_rooms", c => c.Int());
            AlterColumn("dbo.EventHotels", "triple_price", c => c.Double());
            AlterColumn("dbo.EventHotels", "triple_available_rooms", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.EventHotels", "triple_available_rooms", c => c.Int(nullable: false));
            AlterColumn("dbo.EventHotels", "triple_price", c => c.Double(nullable: false));
            AlterColumn("dbo.EventHotels", "double_available_rooms", c => c.Int(nullable: false));
            AlterColumn("dbo.EventHotels", "double_price", c => c.Double(nullable: false));
            AlterColumn("dbo.EventHotels", "single_available_rooms", c => c.Int(nullable: false));
            AlterColumn("dbo.EventHotels", "single_price", c => c.Double(nullable: false));
            DropColumn("dbo.EventHotels", "currency");
        }
    }
}
