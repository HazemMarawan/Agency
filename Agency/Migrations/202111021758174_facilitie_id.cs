namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class facilitie_id : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.HotelFacilities", "facilitie_id", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.HotelFacilities", "facilitie_id");
        }
    }
}
