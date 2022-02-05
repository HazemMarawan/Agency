namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class add_transaction : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Transactions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        amount = c.Double(),
                        transaction_id = c.Int(),
                        reservation_id = c.Int(),
                        active = c.Int(),
                        created_by = c.Int(),
                        updated_by = c.Int(),
                        deleted_by = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        deleted_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Reservations", t => t.reservation_id)
                .Index(t => t.reservation_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Transactions", "reservation_id", "dbo.Reservations");
            DropIndex("dbo.Transactions", new[] { "reservation_id" });
            DropTable("dbo.Transactions");
        }
    }
}
