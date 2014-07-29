namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class terms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbonentCredits", "StartDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.AbonentCredits", "FinishDate", c => c.DateTime(nullable: false, storeType: "date"));
            DropColumn("dbo.AbonentCredits", "DateWhereCreated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AbonentCredits", "DateWhereCreated", c => c.DateTime(nullable: false, storeType: "date"));
            DropColumn("dbo.AbonentCredits", "FinishDate");
            DropColumn("dbo.AbonentCredits", "StartDate");
        }
    }
}
