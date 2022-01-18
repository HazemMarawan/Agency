namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_is_refunf : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "is_refund", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "is_refund");
        }
    }
}
