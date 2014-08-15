namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbgencountervalues : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CounterValues", "CounterValuesId", c => c.Int(nullable: false, identity: true));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CounterValues", "CounterValuesId", c => c.Int(nullable: false));
        }
    }
}
