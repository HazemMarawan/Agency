namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class secretKeyofEvent : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "secret_key", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "secret_key");
        }
    }
}
