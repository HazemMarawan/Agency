namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reservation_shift : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Reservations", "shift", c => c.Int());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Reservations", "shift");
        }
    }
}
