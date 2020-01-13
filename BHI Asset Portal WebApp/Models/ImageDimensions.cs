using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Collections;

namespace BHI_Asset_Portal_WebApp.Models
{

    public class ImageDimension
    {

        private static Dictionary<string, Dictionary<Tuple<int, int>, string>> imageTypes = new Dictionary<string, Dictionary<Tuple<int, int>, string >>()
        {
            {"BannerAds", new Dictionary<Tuple<int,int>, string>()
                {
                    { new Tuple<int, int>(50,50), "Medium" }
                }
            },
            {"NativeAds", new Dictionary<Tuple<int, int>, string>()
                {
                    {new Tuple<int, int>(130,45), "LogoImageURL" },
                    {new Tuple<int, int>(1200,628), "HeroImageURL" }
                }
            }
        };
        

        public static string RetrieveColumn(string SetType, int Width, int Height)
        {
            var key = new Tuple<int, int>(Width, Height);
            return ImageDimension.imageTypes[SetType][key] ?? "";
        }
           
    }
}