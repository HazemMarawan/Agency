namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateReservations : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "paid_amount", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "paid_amount");
        }
    }
}
