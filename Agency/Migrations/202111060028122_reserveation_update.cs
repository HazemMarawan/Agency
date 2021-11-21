namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reserveation_update : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "reservations_officer_name", c => c.String());
            AddColumn("dbo.Reservations", "reservations_officer_phone", c => c.String());
            AddColumn("dbo.Reservations", "reservations_officer_email", c => c.String());
            AddColumn("dbo.Reservations", "financial_advance_date", c => c.DateTime());
            AddColumn("dbo.Reservations", "financial_due_date", c => c.DateTime());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "financial_due_date");
            DropColumn("dbo.Reservations", "financial_advance_date");
            DropColumn("dbo.Reservations", "reservations_officer_email");
            DropColumn("dbo.Reservations", "reservations_officer_phone");
            DropColumn("dbo.Reservations", "reservations_officer_name");
        }
    }
}
