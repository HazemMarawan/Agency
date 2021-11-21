namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addAmountAfterTa : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationDetails", "amount_after_tax", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationDetails", "amount_after_tax");
        }
    }
}
