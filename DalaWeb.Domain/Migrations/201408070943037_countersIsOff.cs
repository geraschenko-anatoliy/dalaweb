namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class countersIsOff : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Counters", "isOff", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Counters", "isOff");
        }
    }
}
