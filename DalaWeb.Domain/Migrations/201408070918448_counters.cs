namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class counters : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Counters",
                c => new
                    {
                        CounterId = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Type = c.Int(nullable: false),
                        Abonent_AbonentId = c.Int(),
                    })
                .PrimaryKey(t => t.CounterId)
                .ForeignKey("dbo.Abonents", t => t.Abonent_AbonentId)
                .Index(t => t.Abonent_AbonentId);
            
            CreateTable(
                "dbo.Stamps",
                c => new
                    {
                        StampId = c.Int(nullable: false, identity: true),
                        CounterId = c.Int(nullable: false),
                        Name = c.String(nullable: false),
                        StartDate = c.DateTime(nullable: false),
                        FinishDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.StampId)
                .ForeignKey("dbo.Counters", t => t.CounterId, cascadeDelete: true)
                .Index(t => t.CounterId);
            
            CreateTable(
                "dbo.CounterValues",
                c => new
                    {
                        CounterValuesId = c.Int(nullable: false),
                        Date = c.DateTime(nullable: false),
                        CounterId = c.Int(nullable: false),
                        Value = c.Double(nullable: false),
                    })
                .PrimaryKey(t => new { t.CounterValuesId, t.Date })
                .ForeignKey("dbo.Counters", t => t.CounterId, cascadeDelete: true)
                .Index(t => t.CounterId);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.CounterValues", new[] { "CounterId" });
            DropIndex("dbo.Stamps", new[] { "CounterId" });
            DropIndex("dbo.Counters", new[] { "Abonent_AbonentId" });
            DropForeignKey("dbo.CounterValues", "CounterId", "dbo.Counters");
            DropForeignKey("dbo.Stamps", "CounterId", "dbo.Counters");
            DropForeignKey("dbo.Counters", "Abonent_AbonentId", "dbo.Abonents");
            DropTable("dbo.CounterValues");
            DropTable("dbo.Stamps");
            DropTable("dbo.Counters");
        }
    }
}
