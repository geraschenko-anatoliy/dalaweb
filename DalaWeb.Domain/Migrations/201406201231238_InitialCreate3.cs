namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate3 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbonentCredits", "PrePayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.AbonentCredits", "Term", c => c.Int(nullable: false));
            AddColumn("dbo.AbonentCredits", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.AbonentCredits", "PaymentForMonth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Credits", "Term");
            DropColumn("dbo.Credits", "Price");
            DropColumn("dbo.Credits", "PaymentForMonth");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Credits", "PaymentForMonth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Credits", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Credits", "Term", c => c.Int(nullable: false));
            DropColumn("dbo.AbonentCredits", "PaymentForMonth");
            DropColumn("dbo.AbonentCredits", "Price");
            DropColumn("dbo.AbonentCredits", "Term");
            DropColumn("dbo.AbonentCredits", "PrePayment");
        }
    }
}
