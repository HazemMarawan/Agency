namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateTables : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationDetails", "room_price", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationDetails", "room_price");
        }
    }
}
