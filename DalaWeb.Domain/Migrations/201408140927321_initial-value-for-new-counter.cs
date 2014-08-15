namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initialvaluefornewcounter : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Counters", "InitialValue", c => c.Double(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Counters", "InitialValue");
        }
    }
}
