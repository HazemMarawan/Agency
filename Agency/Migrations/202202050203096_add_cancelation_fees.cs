namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_cancelation_fees : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "cancelation_fees", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "cancelation_fees");
        }
    }
}
