namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addUsersInfo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "updated_by", c => c.Int());
            AddColumn("dbo.Users", "deleted_by", c => c.Int());
            AddColumn("dbo.Users", "deleted_at", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "deleted_at");
            DropColumn("dbo.Users", "deleted_by");
            DropColumn("dbo.Users", "updated_by");
        }
    }
}
