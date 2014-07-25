namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notnulladdresshouse : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Addresses", "House", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Addresses", "House", c => c.String());
        }
    }
}
