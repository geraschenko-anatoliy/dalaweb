namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dbgennone : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.CounterValues", "CounterValuesId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.CounterValues", "CounterValuesId", c => c.Int(nullable: false, identity: true));
        }
    }
}
