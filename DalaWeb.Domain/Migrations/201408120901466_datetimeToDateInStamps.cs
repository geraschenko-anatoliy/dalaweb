namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetimeToDateInStamps : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Stamps", "StartDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.Stamps", "FinishDate", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Stamps", "FinishDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Stamps", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
