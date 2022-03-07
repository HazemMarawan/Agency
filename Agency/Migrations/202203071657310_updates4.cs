namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updates4 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "reservations_officer_phone_2", c => c.String());
            AddColumn("dbo.Reservations", "reservations_officer_email_2", c => c.String());
            AddColumn("dbo.Reservations", "reservations_officer_phone_3", c => c.String());
            AddColumn("dbo.Reservations", "reservations_officer_email_3", c => c.String());
            AddColumn("dbo.InitialReservations", "reservations_officer_phone_2", c => c.String());
            AddColumn("dbo.InitialReservations", "reservations_officer_email_2", c => c.String());
            AddColumn("dbo.InitialReservations", "reservations_officer_phone_3", c => c.String());
            AddColumn("dbo.InitialReservations", "reservations_officer_email_3", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.InitialReservations", "reservations_officer_email_3");
            DropColumn("dbo.InitialReservations", "reservations_officer_phone_3");
            DropColumn("dbo.InitialReservations", "reservations_officer_email_2");
            DropColumn("dbo.InitialReservations", "reservations_officer_phone_2");
            DropColumn("dbo.Reservations", "reservations_officer_email_3");
            DropColumn("dbo.Reservations", "reservations_officer_phone_3");
            DropColumn("dbo.Reservations", "reservations_officer_email_2");
            DropColumn("dbo.Reservations", "reservations_officer_phone_2");
        }
    }
}
