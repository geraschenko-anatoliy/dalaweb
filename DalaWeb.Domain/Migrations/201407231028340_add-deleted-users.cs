namespace DalaWeb.Domain.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddeletedusers : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Abonents", "isDeleted", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Abonents", "isDeleted");
        }
    }
}
