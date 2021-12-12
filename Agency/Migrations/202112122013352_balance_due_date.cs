namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class balance_due_date : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "balance_due_date", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "balance_due_date");
        }
    }
}
