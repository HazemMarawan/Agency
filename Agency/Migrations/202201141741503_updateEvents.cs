namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateEvents : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Events", "is_special", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Events", "is_special");
        }
    }
}
