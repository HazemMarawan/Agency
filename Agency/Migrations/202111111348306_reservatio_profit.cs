namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reservatio_profit : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "profit", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "profit");
        }
    }
}
