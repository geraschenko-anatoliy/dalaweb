namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate5 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AbonentID = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        StreetId = c.Int(nullable: false),
                        House = c.String(),
                    })
                .PrimaryKey(t => t.AbonentID)
                .ForeignKey("dbo.Abonents", t => t.AbonentID)
                .Index(t => t.AbonentID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Addresses", new[] { "AbonentID" });
            DropForeignKey("dbo.Addresses", "AbonentID", "dbo.Abonents");
            DropTable("dbo.Addresses");
        }
    }
}
