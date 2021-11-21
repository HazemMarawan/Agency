namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class benefittable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.HotelBenefits",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        description = c.String(),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                        benefit_id = c.Int(),
                        hotel_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Hotels", t => t.hotel_id)
                .Index(t => t.hotel_id);
            
            AddColumn("dbo.Events", "due_date", c => c.DateTime(nullable: false));
            AddColumn("dbo.Events", "percentage", c => c.Double());
            AddColumn("dbo.Events", "promocode", c => c.Double());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.HotelBenefits", "hotel_id", "dbo.Hotels");
            DropIndex("dbo.HotelBenefits", new[] { "hotel_id" });
            DropColumn("dbo.Events", "promocode");
            DropColumn("dbo.Events", "percentage");
            DropColumn("dbo.Events", "due_date");
            DropTable("dbo.HotelBenefits");
        }
    }
}
