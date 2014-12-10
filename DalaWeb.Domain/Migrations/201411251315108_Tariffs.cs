namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Tariffs : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tariffs",
                c => new
                    {
                        TarifId = c.Int(nullable: false, identity: true),
                        ServiceId = c.Int(nullable: false),
                        LimitValue = c.Int(nullable: false),
                        OverPrice = c.Double(nullable: false),
                    })
                .PrimaryKey(t => t.TarifId)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: false)
                .Index(t => t.ServiceId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Tariffs", new[] { "ServiceId" });
            DropForeignKey("dbo.Tariffs", "ServiceId", "dbo.Services");
            DropTable("dbo.Tariffs");
        }
    }
}
