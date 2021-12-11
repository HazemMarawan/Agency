namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_vendor_cost : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationDetails", "vendor_cost", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationDetails", "vendor_cost");
        }
    }
}
