namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class nosuspendedservices : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AbonentServices", "isSuspended");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AbonentServices", "isSuspended", c => c.Boolean(nullable: false));
        }
    }
}
