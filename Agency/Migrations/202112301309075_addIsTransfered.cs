namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addIsTransfered : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationDetails", "is_transfered", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationDetails", "is_transfered");
        }
    }
}
