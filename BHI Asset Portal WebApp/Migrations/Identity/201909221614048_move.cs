namespace BHI_Asset_Portal_WebApp.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class move : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.CommentModels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Author = c.String(),
                        Text = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.CreativeSets",
                c => new
                    {
                        CreativeSetID = c.Int(nullable: false, identity: true),
                        SpecialInstructions = c.String(),
                        Valid = c.Boolean(),
                        ScreenShotUrl = c.String(),
                        UsingScript = c.Boolean(),
                        JavaScriptURL = c.String(maxLength: 300),
                        SetName = c.String(maxLength: 250),
                        LeaderboardImageURL = c.String(),
                        SkyscraperImageURL = c.String(),
                        MediumRectangleImageURL = c.String(),
                        HalfPageImageURL = c.String(),
                        MobileLeaderboardImageURL = c.String(),
                        LeaderboardLandingURL = c.String(),
                        SkyscraperLandingURL = c.String(),
                        MediumRectangleLandingURL = c.String(),
                        HalfPageLandingURL = c.String(),
                        MobileLeaderboardLandingURL = c.String(),
                        FacebookHeroImageURL = c.String(),
                        FacebookHeadlineCopy = c.String(),
                        FacebookBodyCopy = c.String(),
                        FacebookLandingURL = c.String(),
                        FacebookCallToActionCopy = c.String(maxLength: 20),
                        LogoImageURL = c.String(maxLength: 500),
                        HeroImageURL = c.String(maxLength: 500),
                        HeaderCopy = c.String(maxLength: 45),
                        BodyCopy = c.String(maxLength: 90),
                        CallToActionCopy = c.String(maxLength: 20),
                        LandingURL = c.String(maxLength: 500),
                        ImpressionTrackerURL = c.String(maxLength: 500),
                        CreativeImageURL = c.String(),
                        CreativeLandingPageURL = c.String(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Line_LineItemID = c.Int(),
                        BannerAds_CreativeSetID = c.Int(),
                        NativAds_CreativeSetID = c.Int(),
                    })
                .PrimaryKey(t => t.CreativeSetID)
                .ForeignKey("dbo.LineItems", t => t.Line_LineItemID)
                .ForeignKey("dbo.CreativeSets", t => t.BannerAds_CreativeSetID)
                .ForeignKey("dbo.CreativeSets", t => t.NativAds_CreativeSetID)
                .Index(t => t.Line_LineItemID)
                .Index(t => t.BannerAds_CreativeSetID)
                .Index(t => t.NativAds_CreativeSetID);
            
            CreateTable(
                "dbo.LineItems",
                c => new
                    {
                        LineItemID = c.Int(nullable: false, identity: true),
                        Product = c.String(),
                        LocationSublocation = c.String(),
                        StartDate = c.DateTime(),
                        EndDate = c.DateTime(),
                        TotalImpressions = c.Int(),
                        LastUpdatedDate = c.DateTime(),
                        Brand = c.String(),
                        Taxonomy = c.String(maxLength: 250),
                        Order_ID = c.Int(),
                    })
                .PrimaryKey(t => t.LineItemID)
                .ForeignKey("dbo.Orders", t => t.Order_ID)
                .Index(t => t.Order_ID);
            
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        BHIOrderNumber = c.String(nullable: false),
                        CreatedDate = c.DateTime(),
                        OrderName = c.String(),
                        ContractStartDate = c.DateTime(),
                        ContractEndDate = c.DateTime(),
                        LastSubmittedDate = c.DateTime(),
                        LastSavedDate = c.DateTime(),
                        Comments = c.String(maxLength: 3000),
                        Complete = c.Boolean(),
                        JiraReferences = c.String(),
                        AgentName = c.String(maxLength: 60),
                        AgentEmail = c.String(maxLength: 60),
                        AccountManager_Id = c.String(maxLength: 128),
                        Advertiser_Id = c.String(maxLength: 128),
                        SalesPerson_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.AspNetUsers", t => t.AccountManager_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.Advertiser_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.SalesPerson_Id)
                .Index(t => t.AccountManager_Id)
                .Index(t => t.Advertiser_Id)
                .Index(t => t.SalesPerson_Id);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(),
                        Organization = c.String(),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.CreativeSets", "NativAds_CreativeSetID", "dbo.CreativeSets");
            DropForeignKey("dbo.CreativeSets", "BannerAds_CreativeSetID", "dbo.CreativeSets");
            DropForeignKey("dbo.Orders", "SalesPerson_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.LineItems", "Order_ID", "dbo.Orders");
            DropForeignKey("dbo.Orders", "Advertiser_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.Orders", "AccountManager_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.CreativeSets", "Line_LineItemID", "dbo.LineItems");
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.Orders", new[] { "SalesPerson_Id" });
            DropIndex("dbo.Orders", new[] { "Advertiser_Id" });
            DropIndex("dbo.Orders", new[] { "AccountManager_Id" });
            DropIndex("dbo.LineItems", new[] { "Order_ID" });
            DropIndex("dbo.CreativeSets", new[] { "NativAds_CreativeSetID" });
            DropIndex("dbo.CreativeSets", new[] { "BannerAds_CreativeSetID" });
            DropIndex("dbo.CreativeSets", new[] { "Line_LineItemID" });
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.Orders");
            DropTable("dbo.LineItems");
            DropTable("dbo.CreativeSets");
            DropTable("dbo.CommentModels");
        }
    }
}
