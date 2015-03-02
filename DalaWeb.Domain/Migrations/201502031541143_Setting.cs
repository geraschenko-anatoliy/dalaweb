namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Setting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Settings",
                c => new
                    {
                        SettingId = c.Int(nullable: false, identity: true),
                        SignatureText = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.SettingId);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Settings");
        }
    }
}
