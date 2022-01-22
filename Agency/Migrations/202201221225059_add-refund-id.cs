namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addrefundid : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "refund_id", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "refund_id");
        }
    }
}
