namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class simpledate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Abonents",
                c => new
                    {
                        AbonentId = c.Int(nullable: false, identity: true),
                        AbonentNumber = c.String(),
                        Name = c.String(nullable: false),
                        Telephone = c.String(nullable: false),
                        INN = c.String(nullable: false),
                        NumberOfInhabitants = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AbonentId);
            
            CreateTable(
                "dbo.AbonentCredits",
                c => new
                    {
                        AbonentId = c.Int(nullable: false),
                        CreditId = c.Int(nullable: false),
                        DateWhereCreated = c.DateTime(nullable: false, storeType: "date"),
                        PrePayment = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaidForTheEntirePeriod = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaidMonths = c.Int(nullable: false),
                        Term = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        PaymentForMonth = c.Decimal(nullable: false, precision: 18, scale: 2),
                        FullyPaid = c.Boolean(nullable: false),
                        Comment = c.String(),
                    })
                .PrimaryKey(t => new { t.AbonentId, t.CreditId })
                .ForeignKey("dbo.Abonents", t => t.AbonentId, cascadeDelete: true)
                .ForeignKey("dbo.Credits", t => t.CreditId, cascadeDelete: true)
                .Index(t => t.AbonentId)
                .Index(t => t.CreditId);
            
            CreateTable(
                "dbo.Credits",
                c => new
                    {
                        CreditId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CreditId);
            
            CreateTable(
                "dbo.AbonentServices",
                c => new
                    {
                        AbonentId = c.Int(nullable: false),
                        ServiceId = c.Int(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        FinishDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => new { t.AbonentId, t.ServiceId, t.StartDate })
                .ForeignKey("dbo.Abonents", t => t.AbonentId, cascadeDelete: true)
                .ForeignKey("dbo.Services", t => t.ServiceId, cascadeDelete: true)
                .Index(t => t.AbonentId)
                .Index(t => t.ServiceId);
            
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        ServiceId = c.Int(nullable: false, identity: true),
                        CompanyId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ServiceId)
                .ForeignKey("dbo.ServiceCompanies", t => t.CompanyId, cascadeDelete: true)
                .Index(t => t.CompanyId);
            
            CreateTable(
                "dbo.ServiceCompanies",
                c => new
                    {
                        CompanyId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.CompanyId);
            
            CreateTable(
                "dbo.Addresses",
                c => new
                    {
                        AbonentId = c.Int(nullable: false),
                        CityId = c.Int(nullable: false),
                        StreetId = c.Int(nullable: false),
                        House = c.String(),
                    })
                .PrimaryKey(t => t.AbonentId)
                .ForeignKey("dbo.Abonents", t => t.AbonentId)
                .Index(t => t.AbonentId);
            
            CreateTable(
                "dbo.Streets",
                c => new
                    {
                        StreetId = c.Int(nullable: false, identity: true),
                        CityId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.StreetId)
                .ForeignKey("dbo.Cities", t => t.CityId, cascadeDelete: true)
                .Index(t => t.CityId);
            
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
            DropIndex("dbo.Streets", new[] { "CityId" });
            DropIndex("dbo.Addresses", new[] { "AbonentId" });
            DropIndex("dbo.Services", new[] { "CompanyId" });
            DropIndex("dbo.AbonentServices", new[] { "ServiceId" });
            DropIndex("dbo.AbonentServices", new[] { "AbonentId" });
            DropIndex("dbo.AbonentCredits", new[] { "CreditId" });
            DropIndex("dbo.AbonentCredits", new[] { "AbonentId" });
            DropForeignKey("dbo.Streets", "CityId", "dbo.Cities");
            DropForeignKey("dbo.Addresses", "AbonentId", "dbo.Abonents");
            DropForeignKey("dbo.Services", "CompanyId", "dbo.ServiceCompanies");
            DropForeignKey("dbo.AbonentServices", "ServiceId", "dbo.Services");
            DropForeignKey("dbo.AbonentServices", "AbonentId", "dbo.Abonents");
            DropForeignKey("dbo.AbonentCredits", "CreditId", "dbo.Credits");
            DropForeignKey("dbo.AbonentCredits", "AbonentId", "dbo.Abonents");
            DropTable("dbo.Cities");
            DropTable("dbo.Streets");
            DropTable("dbo.Addresses");
            DropTable("dbo.ServiceCompanies");
            DropTable("dbo.Services");
            DropTable("dbo.AbonentServices");
            DropTable("dbo.Credits");
            DropTable("dbo.AbonentCredits");
            DropTable("dbo.Abonents");
        }
    }
}
