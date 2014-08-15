namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datecounter555 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Counters", "StartDate");
            DropColumn("dbo.Counters", "FinishDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Counters", "FinishDate", c => c.DateTime(nullable: false, storeType: "date"));
            AddColumn("dbo.Counters", "StartDate", c => c.DateTime(nullable: false, storeType: "date"));
        }
    }
}
