namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class a1 : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.CounterValues", name: "Counter_CounterId", newName: "CounterId");
        }
        
        public override void Down()
        {
            RenameColumn(table: "dbo.CounterValues", name: "CounterId", newName: "Counter_CounterId");
        }
    }
}
