namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addv_code_res_det : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReservationDetails", "vendor_code", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReservationDetails", "vendor_code");
        }
    }
}
