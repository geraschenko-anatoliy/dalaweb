namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class servicecountersrel : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Counters", "Service_ServiceId", c => c.Int());
            AddForeignKey("dbo.Counters", "Service_ServiceId", "dbo.Services", "ServiceId");
            CreateIndex("dbo.Counters", "Service_ServiceId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Counters", new[] { "Service_ServiceId" });
            DropForeignKey("dbo.Counters", "Service_ServiceId", "dbo.Services");
            DropColumn("dbo.Counters", "Service_ServiceId");
        }
    }
}
