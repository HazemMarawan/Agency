namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_tax : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationDetails", "tax", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationDetails", "tax");
        }
    }
}
