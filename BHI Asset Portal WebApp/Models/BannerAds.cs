using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHI_Asset_Portal_WebApp.Models
{
    public class BannerAds : CreativeSet
    {
        [Display(Name = "Leaderboard Image URL")]
        public string LeaderboardImageURL { get; set; }
        [Display(Name = "Skyscraper Image URL")]
        public string SkyscraperImageURL { get; set; }
        [Display(Name = "Medium Rectangle Image URL")]
        public string MediumRectangleImageURL { get; set; }
        [Display(Name = "Half Page Image URL")]
        public string HalfPageImageURL { get; set; }
        [Display(Name = "Mobile Leaderboard Image URL")]
        public string MobileLeaderboardImageURL { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Leaderboard Landing Page URL/Click Tracker URL")]
        public string LeaderboardLandingURL { get; set; }
        [Display(Name = "Skyscraper Landing Page URL/Click Tracker URL")]
        public string SkyscraperLandingURL { get; set; }
        [Display(Name = "Medium Rectangle Landing Page URL/Click Tracker URL")]
        public string MediumRectangleLandingURL { get; set; }
        [Display(Name = "Half Page Landing Page URL/Click Tracker URL")]
        public string HalfPageLandingURL { get; set; }
        [Display(Name = "Mobile Leaderboard Landing Page URL/Click Tracker URL")]
        public string MobileLeaderboardLandingURL { get; set; }

        public BannerAds()
        {
            this.SortOrder = 3;
            this.AdTypeText = AdTypTextType.BannerAds;
        }


    }
}