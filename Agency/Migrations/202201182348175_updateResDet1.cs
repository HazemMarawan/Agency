namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateResDet1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.ReservationDetails", "parent_id", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.ReservationDetails", "parent_id", c => c.Double());
        }
    }
}
