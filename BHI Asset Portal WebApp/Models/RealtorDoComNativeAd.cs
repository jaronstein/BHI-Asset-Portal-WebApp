using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BHI_Asset_Portal_WebApp.Models
{
    public class RealtorDotComNativeAd: NativeAds
    {
        
        public RealtorDotComNativeAd()
        {
            this.SortOrder = 6;
            this.AdTypeText =  AdTypTextType.NativeAds;
        }

    }
}