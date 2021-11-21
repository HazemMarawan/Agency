namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class modifyReservation : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "total_amount_from_vendor", c => c.Double());
            AddColumn("dbo.Reservations", "tax_amount", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "tax_amount");
            DropColumn("dbo.Reservations", "total_amount_from_vendor");
        }
    }
}
