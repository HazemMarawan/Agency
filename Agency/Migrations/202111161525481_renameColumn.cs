namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class renameColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "double_price", c => c.Double());
            DropColumn("dbo.Reservations", "douple_price");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Reservations", "douple_price", c => c.Double());
            DropColumn("dbo.Reservations", "double_price");
        }
    }
}
