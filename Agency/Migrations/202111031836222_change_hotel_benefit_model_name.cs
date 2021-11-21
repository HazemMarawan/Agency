namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class change_hotel_benefit_model_name : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.HotelBenefits", newName: "EventHotelBenefits");
        }
        
        public override void Down()
        {
            RenameTable(name: "dbo.EventHotelBenefits", newName: "HotelBenefits");
        }
    }
}
