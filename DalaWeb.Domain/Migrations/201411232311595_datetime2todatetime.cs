namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2todatetime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AbonentServices", "FinishDate", c => c.DateTime(nullable: false, storeType: "date"));
            DropPrimaryKey("dbo.AbonentServices", new[] { "AbonentId", "ServiceId", "StartDate" });
            AddPrimaryKey("dbo.AbonentServices", new[] { "AbonentId", "ServiceId" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.AbonentServices", new[] { "AbonentId", "ServiceId" });
            AddPrimaryKey("dbo.AbonentServices", new[] { "AbonentId", "ServiceId", "StartDate" });
            AlterColumn("dbo.AbonentServices", "FinishDate", c => c.DateTime(nullable: false));
        }
    }
}
