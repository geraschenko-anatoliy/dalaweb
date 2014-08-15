namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class isOffForStamp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Stamps", "isOff", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Stamps", "isOff");
        }
    }
}
