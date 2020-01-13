using System;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHI_Asset_Portal_WebApp.Models
{
    public class RealtorDotCom : CreativeSet
    {
        [Display (Name ="Creative Image URL")]
        public string CreativeImageURL { get; set; }
        [Display (Name = "Creative Landing Page URL/Click Tracker URL")]
        public string CreativeLandingPageURL { get; set; }

        
        public RealtorDotCom()
        {
            this.SortOrder = 5;
            this.AdTypeText = AdTypTextType.SingleAds;
        }
    }
}