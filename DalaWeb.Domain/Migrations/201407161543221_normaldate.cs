namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class normaldate : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.AbonentServices", new[] { "StartDate" });
            AlterColumn("dbo.AbonentServices", "StartDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.AbonentServices", "FinishDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddPrimaryKey("dbo.AbonentServices", new[] { "StartDate" , "AbonentId", "ServiceId" });
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AbonentServices", "FinishDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.AbonentServices", "StartDate", c => c.DateTime(nullable: false));
        }
    }
}
