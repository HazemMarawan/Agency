namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updates1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Transactions", "transaction_id", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Transactions", "transaction_id", c => c.Int());
        }
    }
}
