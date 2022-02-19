namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updates2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MailServers",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        outgoing_mail = c.String(),
                        outgoing_mail_password = c.String(),
                        outgoing_mail_server = c.String(),
                        port = c.String(),
                        title = c.String(),
                        welcome_message = c.String(),
                        incomiing_mail = c.String(),
                    })
                .PrimaryKey(t => t.id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MailServers");
        }
    }
}
