namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class event_hotel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.EventHotels", "quad_price", c => c.Double());
            AddColumn("dbo.EventHotels", "vendor_single_price", c => c.Double());
            AddColumn("dbo.EventHotels", "vendor_douple_price", c => c.Double());
            AddColumn("dbo.EventHotels", "vendor_triple_price", c => c.Double());
            AddColumn("dbo.EventHotels", "vendor_quad_price", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.EventHotels", "vendor_quad_price");
            DropColumn("dbo.EventHotels", "vendor_triple_price");
            DropColumn("dbo.EventHotels", "vendor_douple_price");
            DropColumn("dbo.EventHotels", "vendor_single_price");
            DropColumn("dbo.EventHotels", "quad_price");
        }
    }
}
