namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updates3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MailServers", "incoming_mail", c => c.String());
            AddColumn("dbo.MailServers", "type", c => c.Int());
            AddColumn("dbo.MailServers", "updated_by", c => c.Int());
            AddColumn("dbo.MailServers", "updated_at", c => c.DateTime());
            DropColumn("dbo.MailServers", "incomiing_mail");
        }
        
        public override void Down()
        {
            AddColumn("dbo.MailServers", "incomiing_mail", c => c.String());
            DropColumn("dbo.MailServers", "updated_at");
            DropColumn("dbo.MailServers", "updated_by");
            DropColumn("dbo.MailServers", "type");
            DropColumn("dbo.MailServers", "incoming_mail");
        }
    }
}
