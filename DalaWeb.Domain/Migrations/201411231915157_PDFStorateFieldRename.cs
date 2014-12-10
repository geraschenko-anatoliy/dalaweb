namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class PDFStorateFieldRename : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.PDFDocuments", "Date", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.PDFDocuments", "date", c => c.DateTime(nullable: false));
        }
    }
}
