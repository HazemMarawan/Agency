namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateResDet : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationDetails", "parent_id", c => c.Double());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationDetails", "parent_id");
        }
    }
}
