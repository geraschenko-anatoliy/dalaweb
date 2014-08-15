namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicecounterrel1 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Counters", "Service_ServiceId", "dbo.Services");
            DropIndex("dbo.Counters", new[] { "Service_ServiceId" });
            RenameColumn(table: "dbo.Counters", name: "Service_ServiceId", newName: "ServiceId");
            AddForeignKey("dbo.Counters", "ServiceId", "dbo.Services", "ServiceId", cascadeDelete: true);
            CreateIndex("dbo.Counters", "ServiceId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Counters", new[] { "ServiceId" });
            DropForeignKey("dbo.Counters", "ServiceId", "dbo.Services");
            RenameColumn(table: "dbo.Counters", name: "ServiceId", newName: "Service_ServiceId");
            CreateIndex("dbo.Counters", "Service_ServiceId");
            AddForeignKey("dbo.Counters", "Service_ServiceId", "dbo.Services", "ServiceId");
        }
    }
}
