using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BHI_Asset_Portal_WebApp.Models
{
    public class FacebookAds: CreativeSet
    {
        [Display (Name = "Hero Image URL 1200x628")]
        public string FacebookHeroImageURL { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Headline Copy")]
        public string FacebookHeadlineCopy { get; set; }
        [DataType(DataType.MultilineText)]
        [Display(Name = "Body Copy")]
        public string FacebookBodyCopy { get; set; }
        [Display(Name = "Landing Page URL/Click Tracker URL")]
        public string FacebookLandingURL { get; set; }

        [Display(Name ="Call To Action Copy")]
        [MaxLength(20)]
        public string FacebookCallToActionCopy { get; set; }

        public FacebookAds()
        {
            this.SortOrder = 4;
            this.AdTypeText = AdTypTextType.SingleAds;
        }
    }
}