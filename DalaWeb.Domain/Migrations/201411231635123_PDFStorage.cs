namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PDFStorage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.PDFDocuments",
                c => new
                    {
                        PDFid = c.Int(nullable: false, identity: true),
                        AbonentId = c.Int(nullable: false),
                        date = c.DateTime(nullable: false),
                        Value = c.Binary(nullable: false),
                    })
                .PrimaryKey(t => t.PDFid);      
        }
        
        public override void Down()
        {
            DropTable("dbo.PDFDocuments");
        }
    }
}
