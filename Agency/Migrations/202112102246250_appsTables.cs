namespace Agency.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class appsTables : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.EmailAttachments",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        attachmentPath = c.String(),
                        email_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Emails", t => t.email_id)
                .Index(t => t.email_id);
            
            CreateTable(
                "dbo.Emails",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        subject = c.String(),
                        body = c.String(),
                        from_user = c.Int(),
                        to_user = c.Int(),
                        active = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Notes",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        title = c.String(),
                        description = c.String(),
                        isFavourite = c.Boolean(nullable: false),
                        created_by = c.Int(nullable: false),
                        active = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.PermissionGroups",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        description = c.String(),
                        created_by = c.Int(nullable: false),
                        active = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        nice_name = c.String(),
                        description = c.String(),
                        created_by = c.Int(nullable: false),
                        active = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        permission_group_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.PermissionGroups", t => t.permission_group_id)
                .Index(t => t.permission_group_id);
            
            CreateTable(
                "dbo.RolePermissions",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        created_by = c.Int(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        permission_id = c.Int(),
                        role_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Permissions", t => t.permission_id)
                .ForeignKey("dbo.Roles", t => t.role_id)
                .Index(t => t.permission_id)
                .Index(t => t.role_id);
            
            CreateTable(
                "dbo.Roles",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        name = c.String(),
                        description = c.String(),
                        created_by = c.Int(nullable: false),
                        active = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.UserRoles",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        created_by = c.Int(nullable: false),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                        user_id = c.Int(),
                        role_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Roles", t => t.role_id)
                .ForeignKey("dbo.Users", t => t.user_id)
                .Index(t => t.user_id)
                .Index(t => t.role_id);
            
            CreateTable(
                "dbo.Tasks",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        title = c.String(),
                        description = c.String(),
                        created_by = c.Int(nullable: false),
                        active = c.Int(),
                        created_at = c.DateTime(),
                        updated_at = c.DateTime(),
                    })
                .PrimaryKey(t => t.id);
            
            CreateTable(
                "dbo.UserTasks",
                c => new
                    {
                        id = c.Int(nullable: false, identity: true),
                        status = c.Int(nullable: false),
                        user_id = c.Int(),
                        task_id = c.Int(),
                    })
                .PrimaryKey(t => t.id)
                .ForeignKey("dbo.Tasks", t => t.task_id)
                .ForeignKey("dbo.Users", t => t.user_id)
                .Index(t => t.user_id)
                .Index(t => t.task_id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserTasks", "user_id", "dbo.Users");
            DropForeignKey("dbo.UserTasks", "task_id", "dbo.Tasks");
            DropForeignKey("dbo.RolePermissions", "role_id", "dbo.Roles");
            DropForeignKey("dbo.UserRoles", "user_id", "dbo.Users");
            DropForeignKey("dbo.UserRoles", "role_id", "dbo.Roles");
            DropForeignKey("dbo.RolePermissions", "permission_id", "dbo.Permissions");
            DropForeignKey("dbo.Permissions", "permission_group_id", "dbo.PermissionGroups");
            DropForeignKey("dbo.EmailAttachments", "email_id", "dbo.Emails");
            DropIndex("dbo.UserTasks", new[] { "task_id" });
            DropIndex("dbo.UserTasks", new[] { "user_id" });
            DropIndex("dbo.UserRoles", new[] { "role_id" });
            DropIndex("dbo.UserRoles", new[] { "user_id" });
            DropIndex("dbo.RolePermissions", new[] { "role_id" });
            DropIndex("dbo.RolePermissions", new[] { "permission_id" });
            DropIndex("dbo.Permissions", new[] { "permission_group_id" });
            DropIndex("dbo.EmailAttachments", new[] { "email_id" });
            DropTable("dbo.UserTasks");
            DropTable("dbo.Tasks");
            DropTable("dbo.UserRoles");
            DropTable("dbo.Roles");
            DropTable("dbo.RolePermissions");
            DropTable("dbo.Permissions");
            DropTable("dbo.PermissionGroups");
            DropTable("dbo.Notes");
            DropTable("dbo.Emails");
            DropTable("dbo.EmailAttachments");
        }
    }
}
