namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class a1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Counters", "StartDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Counters", "FinishDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Counters", "FinishDate");
            DropColumn("dbo.Counters", "StartDate");
        }
    }
}
