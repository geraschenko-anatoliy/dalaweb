namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class abonentcounterrel : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Counters", "Abonent_AbonentId", "dbo.Abonents");
            DropIndex("dbo.Counters", new[] { "Abonent_AbonentId" });
            RenameColumn(table: "dbo.Counters", name: "Abonent_AbonentId", newName: "AbonentId");
            AddForeignKey("dbo.Counters", "AbonentId", "dbo.Abonents", "AbonentId", cascadeDelete: true);
            CreateIndex("dbo.Counters", "AbonentId");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Counters", new[] { "AbonentId" });
            DropForeignKey("dbo.Counters", "AbonentId", "dbo.Abonents");
            RenameColumn(table: "dbo.Counters", name: "AbonentId", newName: "Abonent_AbonentId");
            CreateIndex("dbo.Counters", "Abonent_AbonentId");
            AddForeignKey("dbo.Counters", "Abonent_AbonentId", "dbo.Abonents", "AbonentId");
        }
    }
}
