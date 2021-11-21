namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class createVendorTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Vendors",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Code = c.String(),
                        Name = c.String(),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.EventHotels", "vendor_id", c => c.Int());
            AddColumn("dbo.Reservations", "vendor_id", c => c.Int());
            CreateIndex("dbo.EventHotels", "vendor_id");
            CreateIndex("dbo.Reservations", "vendor_id");
            AddForeignKey("dbo.Reservations", "vendor_id", "dbo.Vendors", "Id");
            AddForeignKey("dbo.EventHotels", "vendor_id", "dbo.Vendors", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.EventHotels", "vendor_id", "dbo.Vendors");
            DropForeignKey("dbo.Reservations", "vendor_id", "dbo.Vendors");
            DropIndex("dbo.Reservations", new[] { "vendor_id" });
            DropIndex("dbo.EventHotels", new[] { "vendor_id" });
            DropColumn("dbo.Reservations", "vendor_id");
            DropColumn("dbo.EventHotels", "vendor_id");
            DropTable("dbo.Vendors");
        }
    }
}
