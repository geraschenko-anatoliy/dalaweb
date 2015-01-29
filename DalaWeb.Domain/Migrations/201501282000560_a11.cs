namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class a11 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PDFDocuments", "AbonentId", "dbo.Abonents");
            DropIndex("dbo.PDFDocuments", new[] { "AbonentId" });
            CreateTable(
                "dbo.PDFAbonentMonthlyReceipts",
                c => new
                    {
                        PDFId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        Value = c.Binary(nullable: false),
                        TimeStamp = c.DateTime(nullable: false, storeType: "date"),
                        AbonentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PDFId)
                .ForeignKey("dbo.Abonents", t => t.AbonentId)
                .Index(t => t.AbonentId);
            
            DropTable("dbo.PDFDocuments");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.PDFDocuments",
                c => new
                    {
                        PDFId = c.Int(nullable: false, identity: true),
                        Date = c.DateTime(nullable: false, storeType: "date"),
                        Value = c.Binary(nullable: false),
                        TimeStamp = c.DateTime(nullable: false, storeType: "date"),
                        AbonentId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.PDFId);
            
            DropIndex("dbo.PDFAbonentMonthlyReceipts", new[] { "AbonentId" });
            DropForeignKey("dbo.PDFAbonentMonthlyReceipts", "AbonentId", "dbo.Abonents");
            DropTable("dbo.PDFAbonentMonthlyReceipts");
            CreateIndex("dbo.PDFDocuments", "AbonentId");
            AddForeignKey("dbo.PDFDocuments", "AbonentId", "dbo.Abonents", "AbonentId");
        }
    }
}
