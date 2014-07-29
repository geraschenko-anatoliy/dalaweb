namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class archivalservices : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Services", "Archival", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Services", "Archival");
        }
    }
}
