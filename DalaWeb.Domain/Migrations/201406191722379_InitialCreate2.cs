namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate2 : DbMigration
    {
        public override void Up()
        {
            RenameTable(name: "dbo.Abonent", newName: "Abonents");
            RenameTable(name: "dbo.Credit", newName: "Credits");
            RenameTable(name: "dbo.AbonentCredit", newName: "AbonentCredits");
            AddColumn("dbo.Credits", "PaymentForMonth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.AbonentCredits", "DateWhereCreated", c => c.DateTime(nullable: false));
            AddColumn("dbo.AbonentCredits", "PaidForTheEntirePeriod", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.AbonentCredits", "PaidMonths", c => c.Int(nullable: false));
            AddColumn("dbo.AbonentCredits", "FullyPaid", c => c.Boolean(nullable: false));
            AddColumn("dbo.AbonentCredits", "Comment", c => c.String());
            DropColumn("dbo.Credits", "DateWhereCreated");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Credits", "DateWhereCreated", c => c.DateTime(nullable: false));
            DropColumn("dbo.AbonentCredits", "Comment");
            DropColumn("dbo.AbonentCredits", "FullyPaid");
            DropColumn("dbo.AbonentCredits", "PaidMonths");
            DropColumn("dbo.AbonentCredits", "PaidForTheEntirePeriod");
            DropColumn("dbo.AbonentCredits", "DateWhereCreated");
            DropColumn("dbo.Credits", "PaymentForMonth");
            RenameTable(name: "dbo.AbonentCredits", newName: "AbonentCredit");
            RenameTable(name: "dbo.Credits", newName: "Credit");
            RenameTable(name: "dbo.Abonents", newName: "Abonent");
        }
    }
}
