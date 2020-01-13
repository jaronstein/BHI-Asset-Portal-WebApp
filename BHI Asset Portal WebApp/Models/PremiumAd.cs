using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BHI_Asset_Portal_WebApp.Models
{
    public class PremiumAd: CreativeSet
    {
        public  BannerAds BannerAds { get; set; }
        public  NativeAds NativAds { get; set; }

        public PremiumAd()
        {
            this.SortOrder = 1;
            this.AdTypeText = AdTypTextType.PremiumAds;
        }
    }
}