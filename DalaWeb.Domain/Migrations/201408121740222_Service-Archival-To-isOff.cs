namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ServiceArchivalToisOff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "isOff", c => c.Boolean(nullable: false));
            DropColumn("dbo.Services", "Archival");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Services", "Archival", c => c.Boolean(nullable: false));
            DropColumn("dbo.Services", "isOff");
        }
    }
}
