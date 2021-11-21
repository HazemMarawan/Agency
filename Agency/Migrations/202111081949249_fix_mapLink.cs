namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fix_mapLink : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Locations", "map_link", c => c.String());
            DropColumn("dbo.Locations", "mapLink");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Locations", "mapLink", c => c.String());
            DropColumn("dbo.Locations", "map_link");
        }
    }
}
