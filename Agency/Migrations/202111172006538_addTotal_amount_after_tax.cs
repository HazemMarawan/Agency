namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addTotal_amount_after_tax : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "total_amount_after_tax", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "total_amount_after_tax");
        }
    }
}
