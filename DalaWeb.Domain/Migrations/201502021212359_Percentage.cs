namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Percentage : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PaymentQueues", "Percentage", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PaymentQueues", "Percentage");
        }
    }
}
