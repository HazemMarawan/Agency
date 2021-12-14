namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateCompanyTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "state", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "state");
        }
    }
}
