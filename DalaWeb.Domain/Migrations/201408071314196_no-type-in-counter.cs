namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class notypeincounter : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Counters", "Type");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Counters", "Type", c => c.Int(nullable: false));
        }
    }
}
