namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class available_rooms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventHotels", "single_available_rooms", c => c.Int(nullable: false));
            AddColumn("dbo.EventHotels", "double_available_rooms", c => c.Int(nullable: false));
            AddColumn("dbo.EventHotels", "triple_available_rooms", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.EventHotels", "triple_available_rooms");
            DropColumn("dbo.EventHotels", "double_available_rooms");
            DropColumn("dbo.EventHotels", "single_available_rooms");
        }
    }
}
