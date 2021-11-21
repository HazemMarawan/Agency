namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class event_from_to : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "from", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "to", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "to");
            DropColumn("dbo.Events", "from");
        }
    }
}
