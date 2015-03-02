namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PaymentQueue : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PaymentQueues",
                c => new
                    {
                        PaymentQueueId = c.Int(nullable: false, identity: true),
                        Queue = c.String(nullable: false),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        AbonentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PaymentQueueId)
                .ForeignKey("dbo.Abonents", t => t.AbonentId)
                .Index(t => t.AbonentId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.PaymentQueues", new[] { "AbonentId" });
            DropForeignKey("dbo.PaymentQueues", "AbonentId", "dbo.Abonents");
            DropTable("dbo.PaymentQueues");
        }
    }
}
