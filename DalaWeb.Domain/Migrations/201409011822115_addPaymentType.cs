namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addPaymentType : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Payments", "Type", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Payments", "Type");
        }
    }
}
