namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datecounter5 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Counters", "StartDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.Counters", "FinishDate", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Counters", "FinishDate");
            DropColumn("dbo.Counters", "StartDate");
        }
    }
}
