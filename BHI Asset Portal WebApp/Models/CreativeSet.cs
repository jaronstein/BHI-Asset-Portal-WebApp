using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;

namespace BHI_Asset_Portal_WebApp.Models
{
    public class CreativeSet
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CreativeSetID { get; set; }
        public LineItem Line { get; set; }

        [Display(Name = "Special Instructions")]
        [DataType(DataType.MultilineText)]
        public string SpecialInstructions { get; set; }
        [DefaultValue(false)]
        public bool? Valid { get; set; }
        [Display(Name = "Screenshot URL")]
        public string ScreenShotUrl { get; set; }

        [DefaultValue(false)]
        public bool? UsingScript { get; set; }
        
        [Display(Name = "Location of Javascript")]
        [MaxLength(300)]
        public string JavaScriptURL { get; set; }

        [Display (Name = "Set Name")]
        [MaxLength(250)]
        public string SetName { get; set; }

        [NotMapped]
        public  int SortOrder { get; set; } 

        //Set in each model. No need to save to DB
        [NotMapped]
        public AdTypTextType AdTypeText { get; set; }

        public enum AdTypTextType
        {
            BannerAds,
            NativeAds,
            PremiumAds,
            SingleAds

         
        }

     

    }
}