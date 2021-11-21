namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reservatio_data : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "triple_price", c => c.Double());
            AddColumn("dbo.Reservations", "quad_price", c => c.Double());
            AddColumn("dbo.Reservations", "opener", c => c.Int());
            AddColumn("dbo.Reservations", "closer", c => c.Int());
            AddColumn("dbo.Reservations", "vendor_single_price", c => c.Double());
            AddColumn("dbo.Reservations", "vendor_douple_price", c => c.Double());
            AddColumn("dbo.Reservations", "vendor_triple_price", c => c.Double());
            AddColumn("dbo.Reservations", "vendor_quad_price", c => c.Double());
            AddColumn("dbo.Reservations", "total_rooms", c => c.Int());
            AddColumn("dbo.Reservations", "total_nights", c => c.Int());
            AddColumn("dbo.Reservations", "check_in", c => c.DateTime());
            AddColumn("dbo.Reservations", "check_out", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "check_out");
            DropColumn("dbo.Reservations", "check_in");
            DropColumn("dbo.Reservations", "total_nights");
            DropColumn("dbo.Reservations", "total_rooms");
            DropColumn("dbo.Reservations", "vendor_quad_price");
            DropColumn("dbo.Reservations", "vendor_triple_price");
            DropColumn("dbo.Reservations", "vendor_douple_price");
            DropColumn("dbo.Reservations", "vendor_single_price");
            DropColumn("dbo.Reservations", "closer");
            DropColumn("dbo.Reservations", "opener");
            DropColumn("dbo.Reservations", "quad_price");
            DropColumn("dbo.Reservations", "triple_price");
        }
    }
}
