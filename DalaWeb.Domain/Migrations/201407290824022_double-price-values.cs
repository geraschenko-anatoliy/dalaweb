namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class doublepricevalues : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AbonentCredits", "PrePayment", c => c.Double(nullable: false));
            AlterColumn("dbo.AbonentCredits", "PaidForTheEntirePeriod", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AbonentCredits", "PaidForTheEntirePeriod", c => c.Int(nullable: false));
            AlterColumn("dbo.AbonentCredits", "PrePayment", c => c.Int(nullable: false));
        }
    }
}
