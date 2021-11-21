namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class hoteladdress : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Hotels", "address", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Hotels", "address");
        }
    }
}
