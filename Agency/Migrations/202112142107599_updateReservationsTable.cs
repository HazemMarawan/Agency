namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updateReservationsTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Companies", "fax", c => c.String());
            AddColumn("dbo.Companies", "address", c => c.String());
            AddColumn("dbo.Companies", "postal_code", c => c.String());
            AddColumn("dbo.Companies", "country", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Companies", "country");
            DropColumn("dbo.Companies", "postal_code");
            DropColumn("dbo.Companies", "address");
            DropColumn("dbo.Companies", "fax");
        }
    }
}
