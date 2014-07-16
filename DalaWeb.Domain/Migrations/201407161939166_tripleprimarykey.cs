namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class tripleprimarykey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.AbonentServices", new[] { "StartDate" });
            AddPrimaryKey("dbo.AbonentServices", new[] { "StartDate", "AbonentId", "ServiceId" });
        }
        
        public override void Down()
        {
        }
    }
}
