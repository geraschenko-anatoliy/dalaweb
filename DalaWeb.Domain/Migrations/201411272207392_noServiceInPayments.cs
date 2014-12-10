namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class noServiceInPayments : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Payments", "ServiceId");
            DropColumn("dbo.Payments", "CreditId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Payments", "CreditId", c => c.Int(nullable: false));
            AddColumn("dbo.Payments", "ServiceId", c => c.Int(nullable: false));
        }
    }
}
