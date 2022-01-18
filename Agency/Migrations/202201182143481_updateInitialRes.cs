namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateInitialRes : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.InitialReservations", "reservation_avg_price_before_tax", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InitialReservations", "reservation_avg_price_before_tax");
        }
    }
}
