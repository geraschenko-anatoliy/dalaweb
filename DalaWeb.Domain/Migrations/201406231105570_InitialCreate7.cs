namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate7 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Streets", "City_CityId", "dbo.Cities");
            DropIndex("dbo.Streets", new[] { "City_CityId" });
            AddColumn("dbo.Streets", "CityId", c => c.Int(nullable: false));
            AddForeignKey("dbo.Streets", "CityId", "dbo.Cities", "CityId", cascadeDelete: true);
            CreateIndex("dbo.Streets", "CityId");
            DropColumn("dbo.Streets", "City_CityId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Streets", "City_CityId", c => c.Int());
            DropIndex("dbo.Streets", new[] { "CityId" });
            DropForeignKey("dbo.Streets", "CityId", "dbo.Cities");
            DropColumn("dbo.Streets", "CityId");
            CreateIndex("dbo.Streets", "City_CityId");
            AddForeignKey("dbo.Streets", "City_CityId", "dbo.Cities", "CityId");
        }
    }
}
