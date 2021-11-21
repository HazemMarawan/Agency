namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class event_from : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "event_from", c => c.DateTime(nullable: false));
            DropColumn("dbo.Events", "from");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Events", "from", c => c.DateTime(nullable: false));
            DropColumn("dbo.Events", "event_from");
        }
    }
}
