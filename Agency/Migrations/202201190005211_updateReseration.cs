namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateReseration : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "credit", c => c.Double());
            AddColumn("dbo.Reservations", "refud", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "refud");
            DropColumn("dbo.Reservations", "credit");
        }
    }
}
