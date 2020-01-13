using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;


namespace BHI_Asset_Portal_WebApp.Models
{
    public class NativeAds: CreativeSet
    {
        [MaxLength (500)]
        [Display (Name = "Logo Image 130 x 45")]
        public string LogoImageURL { get; set; }

        [MaxLength(500)]
        [Display(Name = "Hero Image 1200 x 628")]
        public string HeroImageURL { get; set; }

        [Display(Name = "Header Copy")]
        [MaxLength(45)]
        public string HeaderCopy { get; set; }

        [Display(Name = "Body Copy")]
        [MaxLength(90)]
        public string BodyCopy { get; set; }

        [MaxLength(20)]
        [Display(Name = "Call To Action Copy")]
        public string CallToActionCopy { get; set; }

        [MaxLength(500)]
        [Display(Name = "Landing Page URL/Click Tracker URL")]
        public string LandingURL { get; set; }

        [MaxLength(500)]
        [Display(Name = "Impression Tracker URL")]
        public string ImpressionTrackerURL { get; set; }


        public NativeAds()
        {
            this.SortOrder = 2;
            this.AdTypeText = AdTypTextType.NativeAds;
        }


    }
}