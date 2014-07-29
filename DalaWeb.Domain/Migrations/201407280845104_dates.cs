namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dates : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AbonentCredits", "StartDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.AbonentCredits", "FinishDate", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AbonentCredits", "FinishDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AbonentCredits", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
