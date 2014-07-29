namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datedatetime : DbMigration
    {
        public override void Up()
        {
            //AlterColumn("dbo.AbonentServices", "StartDate", c => c.DateTime(nullable: false));
            //AlterColumn("dbo.AbonentServices", "FinishDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            //AlterColumn("dbo.AbonentServices", "FinishDate", c => c.DateTime(nullable: false, storeType: "date"));
            AlterColumn("dbo.AbonentServices", "StartDate", c => c.DateTime(nullable: false, storeType: "date"));
        }
    }
}
