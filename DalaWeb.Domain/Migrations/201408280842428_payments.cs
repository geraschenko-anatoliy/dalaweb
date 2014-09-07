namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class payments : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Payments",
                c => new
                    {
                        PaymentId = c.Int(nullable: false, identity: true),
                        AbonentId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        Sum = c.Double(nullable: false),
                        Balance = c.Double(nullable: false),
                        Comment = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentId)
                .ForeignKey("dbo.Abonents", t => t.AbonentId, cascadeDelete: true)
                .Index(t => t.AbonentId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Payments", new[] { "AbonentId" });
            DropForeignKey("dbo.Payments", "AbonentId", "dbo.Abonents");
            DropTable("dbo.Payments");
        }
    }
}
