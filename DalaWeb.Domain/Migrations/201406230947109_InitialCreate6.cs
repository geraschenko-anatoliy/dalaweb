namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate6 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Streets",
                c => new
                    {
                        StreetId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        City_CityId = c.Int(),
                    })
                .PrimaryKey(t => t.StreetId)
                .ForeignKey("dbo.Cities", t => t.City_CityId)
                .Index(t => t.City_CityId);
            
            CreateTable(
                "dbo.Cities",
                c => new
                    {
                        CityId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CityId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Streets", new[] { "City_CityId" });
            DropForeignKey("dbo.Streets", "City_CityId", "dbo.Cities");
            DropTable("dbo.Cities");
            DropTable("dbo.Streets");
        }
    }
}
