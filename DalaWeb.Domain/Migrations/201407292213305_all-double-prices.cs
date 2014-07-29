namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class alldoubleprices : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AbonentCredits", "Price", c => c.Double(nullable: false));
            AlterColumn("dbo.Services", "Price", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Services", "Price", c => c.Int(nullable: false));
            AlterColumn("dbo.AbonentCredits", "Price", c => c.Int(nullable: false));
        }
    }
}
