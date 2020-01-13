namespace BHI_Asset_Portal_WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NewIDs : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Orders", "AgencyName", c => c.String(maxLength: 60));
            AddColumn("dbo.Orders", "SalesRepName", c => c.String(maxLength: 60));
            AddColumn("dbo.Orders", "ADVAccountManagerName", c => c.String(maxLength: 60));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Orders", "ADVAccountManagerName");
            DropColumn("dbo.Orders", "SalesRepName");
            DropColumn("dbo.Orders", "AgencyName");
        }
    }
}
