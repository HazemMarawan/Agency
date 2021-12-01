namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addLogTableReservationId : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationLogs", "reservation_id", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationLogs", "reservation_id");
        }
    }
}
