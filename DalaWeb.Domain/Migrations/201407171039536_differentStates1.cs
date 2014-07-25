namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class differentStates1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AbonentServices", "isSuspended", c => c.Boolean(nullable: false));
            AddColumn("dbo.AbonentServices", "isOff", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AbonentServices", "isOff");
            DropColumn("dbo.AbonentServices", "isSuspended");
        }
    }
}
