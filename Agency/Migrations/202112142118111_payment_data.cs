namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payment_data : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "payment_type", c => c.Int(nullable: false));
            AddColumn("dbo.Reservations", "credit_card_number", c => c.String());
            AddColumn("dbo.Reservations", "security_code", c => c.String());
            AddColumn("dbo.Reservations", "card_expiration_date", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "card_expiration_date");
            DropColumn("dbo.Reservations", "security_code");
            DropColumn("dbo.Reservations", "credit_card_number");
            DropColumn("dbo.Reservations", "payment_type");
        }
    }
}
