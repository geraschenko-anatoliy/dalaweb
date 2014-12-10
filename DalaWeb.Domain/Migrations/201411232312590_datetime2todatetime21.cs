namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime2todatetime21 : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.AbonentServices", new[] { "AbonentId", "ServiceId" });
            AddPrimaryKey("dbo.AbonentServices", new[] { "AbonentId", "ServiceId", "StartDate" });
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.AbonentServices", new[] { "AbonentId", "ServiceId", "StartDate" });
            AddPrimaryKey("dbo.AbonentServices", new[] { "AbonentId", "ServiceId" });
        }
    }
}
