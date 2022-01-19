namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateRefund : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "refund", c => c.Double());
            DropColumn("dbo.Reservations", "refud");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservations", "refud", c => c.Double());
            DropColumn("dbo.Reservations", "refund");
        }
    }
}
