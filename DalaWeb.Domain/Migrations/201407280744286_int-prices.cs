namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class intprices : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AbonentCredits", "PrePayment", c => c.Int(nullable: false));
            AlterColumn("dbo.AbonentCredits", "PaidForTheEntirePeriod", c => c.Int(nullable: false));
            AlterColumn("dbo.AbonentCredits", "Price", c => c.Int(nullable: false));
            AlterColumn("dbo.AbonentCredits", "PaymentForMonth", c => c.Double(nullable: false));
            AlterColumn("dbo.Services", "Price", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Services", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.AbonentCredits", "PaymentForMonth", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.AbonentCredits", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.AbonentCredits", "PaidForTheEntirePeriod", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AlterColumn("dbo.AbonentCredits", "PrePayment", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
