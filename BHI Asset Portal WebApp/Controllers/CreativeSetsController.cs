using System;
using System.Net;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BHI_Asset_Portal_WebApp.Models;
using BHI_Asset_Portal_WebApp.ViewModels;
using System.Configuration;
using Microsoft.AspNet.Identity;
using System.IO;
using Kraken;
using Kraken.Http;
using Kraken.Model.Azure;
using System.Drawing;
using OptimizeUploadWaitRequest = Kraken.Model.Azure.OptimizeUploadWaitRequest;
using System.Collections;
using System.Collections.Generic;
using System.IO.Compression;
using System.Data.Entity.Infrastructure;
using System.Data.Entity;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Auth;

namespace BHI_Asset_Portal_WebApp.Controllers
{
    public class CreativeSetsController : Controller
    {

        private readonly UserManager<ApplicationUser> _userManager;
       
        
        // GET: CreativeSet
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Edit(CreativeSet set)
        {
            Type thisOne = set.GetType();
           
            return View();
        }
        [HttpPost]
        public ActionResult UploadZip( HttpPostedFileBase file )
        {
            try
            {
                ZipArchive archive = new ZipArchive(file.InputStream, ZipArchiveMode.Read);
                //Dictionary<string, string> imageURLs = new Dictionary<string, string>();
                List<string> imageURLs = new List<string>();
                List<string> names = new List<string>();
                List<int> widths = new List<int>();
                List<int> heights = new List<int>();

                foreach (var item in archive.Entries)
                {
                    
                    var url = "";
                    var name = DateTime.Now.ToFileTime() + "-" + item.Name;
                    if (item.FullName.ToLower().EndsWith(".png") || item.FullName.ToLower().EndsWith(".jpg") ||
                        item.FullName.ToLower().EndsWith("gif") || item.FullName.ToLower().EndsWith("jpeg") || item.FullName.ToLower().EndsWith("webp"))
                    {
                        try
                        {

                            Stream imgStream = item.Open();
                            Image image = System.Drawing.Image.FromStream(imgStream);

                            url = SaveImage(new ApplicationDbContext(), 1, image, "", "", name, image.Width, image.Height, true);
                            imageURLs.Add(url);
                            names.Add(item.Name);
                            widths.Add(image.Width);
                            heights.Add(image.Height);


                        }
                        catch(Exception ex)
                        {

                        }
                    }

                }
                return Json(new {success = true,
                Images = imageURLs, Names = names, Widths = widths, Heights = heights});
            }
            catch(Exception ex)
            {
                return Json(new { success = false, Images = "" });

            }
        }
        [HttpPost]
        public ActionResult UploadJavaScript(string creativesetid, HttpPostedFileBase file)
        {
            try {
                ApplicationDbContext context = new ApplicationDbContext();
                var userId = User.Identity.GetUserId();
                var organization = context.Users.Where(u => u.Id == userId).First().Organization;
                var location = ConfigurationManager.AppSettings["JSDirectory"];
                int creativeSetID = int.Parse(creativesetid);

                //Directory.CreateDirectory(Server.MapPath($"~//{location}//{organization}"));

                // var jsURL = $"{location}//{organization}//{file.FileName}";
                //var rootedPath = Server.MapPath(jsURL);
                //file.SaveAs(rootedPath);
                byte[] data;
                using (Stream inputStream = file.InputStream)
                {
                    MemoryStream memoryStream = inputStream as MemoryStream;
                    if (memoryStream == null)
                    {
                        memoryStream = new MemoryStream();
                        inputStream.CopyTo(memoryStream);
                    }
                    data = memoryStream.ToArray();
                }


                CloudStorageAccount account;

                CloudStorageAccount.TryParse("DefaultEndpointsProtocol=https;AccountName=bhiimagesandscripts;AccountKey=yDBzm9z8o4HhXLDAtXNX0TKHxa42amNIId/xg7MtEY5y4wpdzMkVUL26EA+WM8a1Iujm9e66HMPAwAg1cU1qhQ==;EndpointSuffix=core.windows.net", out account);
                CloudBlobClient clieent = account.CreateCloudBlobClient();
                CloudBlobContainer container = clieent.GetContainerReference("bhi");


                CloudBlockBlob blob = container.GetBlockBlobReference(file.FileName);
                blob.UploadFromByteArray(data, 0, data.Length);

                var creativeSet = context.CreativeSets.Where(c => c.CreativeSetID == creativeSetID).First();
                creativeSet.JavaScriptURL = blob.Uri.AbsoluteUri;
                //creativeSet.JavaScriptURL = $"//{organization}//{file.FileName}";
                context.SaveChanges();

                return Json(new
                {
                    success = true,
                    file = blob.Uri.AbsoluteUri,
                    response = "File uploaded."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    response = "There was a problem uploading the file please contact BHI if the problem persists."
                });
            }
        }
        [HttpPost]
        public ActionResult UploadFile(string creativename, string creativesize, string creativesetid, string columnname, int maxSize, int width, int height, HttpPostedFileBase file)
        {
            try
            {
                Image image = Image.FromStream(file.InputStream);

                var url = "";

                ApplicationDbContext context = new ApplicationDbContext();
                var userId = User.Identity.GetUserId();
                var organization = context.Users.Where(u => u.Id == userId).First().Organization;

                url = SaveImage(context, int.Parse(creativesetid), image, organization, columnname, file.FileName, width, height, false);

                return Json(new
                {
                    success = true,
                    file = url,
                    response = "File uploaded."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    response = "There was a problem uploading the file please contact BHI if the problem persists."
                });
            }

        }
        [HttpPost]
        public JsonResult SaveImageWithURL (int CreativeSetID, string URL, string FileName, string ColumnName , int Width, int Height)
        {
            try {
                byte[] lnBuffer;
                byte[] lnFile;
                System.Net.HttpWebRequest webRequest = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(URL);
                String lsResponse = string.Empty;
                byte[] lnByte;
                using (HttpWebResponse lxResponse = (HttpWebResponse)webRequest.GetResponse())
                {
                    using (BinaryReader reader = new BinaryReader(lxResponse.GetResponseStream()))
                    {
                        lnByte = reader.ReadBytes(1 * 1024 * 1024 * 10);
                        
                    }
                }


                var url = "";

                ApplicationDbContext context = new ApplicationDbContext();
                var userId = User.Identity.GetUserId();
                var organization = context.Users.Where(u => u.Id == userId).First().Organization;


                var conn = Connection.Create("01037a9bb1b2888dca6201311ba6c581", "7f7a122d1914c41216f3f94a51d1cabd3154c877", false);
                Client client = new Client(conn);
                var k = new OptimizeWaitRequest(new Uri(URL), "bhiimagesandscripts", "qD/CJSVqb9uzff2pDhwcTJ2X49VCvQF5t/zwmy353DlEdZ12DbLe5apCN6m88xqJt9vjQ20kKRAngApdJxBJWw==", "bhi");
               
                k.Lossy = true;
                k.WebP = true;
                k.ResizeImage = new Kraken.Model.ResizeImage() { Height = Height, Width = Width, Strategy = Kraken.Model.Strategy.Exact };
                var response = client.OptimizeWait(k);
                if (response.Result.StatusCode == HttpStatusCode.OK)
                {
                    url = response.Result.Body.KrakedUrl;
                    if (ColumnName != "")
                    {
                        var creativeSet = context.CreativeSets.Where(c => c.CreativeSetID == CreativeSetID).First();
                        creativeSet.GetType().GetProperty(ColumnName).SetValue(creativeSet, url);
                        context.SaveChanges();
                    }
                }
                //var creativeSet = context.CreativeSets.Where(c => c.CreativeSetID == CreativeSetID).First();
                //creativeSet.GetType().GetProperty(ColumnName).SetValue(creativeSet, url);
                //context.SaveChanges();

                return Json(new
                {
                    success = true,
                    file = url,
                    response = "File uploaded."
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false
                });
            }

            
        }
        private string SaveImage(ApplicationDbContext context, int creativeSetID, Image image, string organization, string columnName, string fileName, int width, int height, bool forZip)
        {
            ImageConverter _imageConverter = new ImageConverter();
            byte[] imgData = (byte[])_imageConverter.ConvertTo(image, typeof(byte[]));


            var conn = Connection.Create("01037a9bb1b2888dca6201311ba6c581", "7f7a122d1914c41216f3f94a51d1cabd3154c877", false);
            Client client = new Client(conn);
            var k = new OptimizeUploadWaitRequest("bhiimagesandscripts", "qD/CJSVqb9uzff2pDhwcTJ2X49VCvQF5t/zwmy353DlEdZ12DbLe5apCN6m88xqJt9vjQ20kKRAngApdJxBJWw==", "bhi");

            k.Lossy = true;
            if (forZip)
                k.WebP = false;
            else
                k.WebP = true;
            k.ResizeImage = new Kraken.Model.ResizeImage() { Height = height, Width = width, Strategy = Kraken.Model.Strategy.Fit };
            var url = "";
            var response = client.OptimizeWait(imgData, organization + "." + columnName + "." + fileName, k);
            if (response.Result.StatusCode == HttpStatusCode.OK)
            {
                url = response.Result.Body.KrakedUrl;
                if (columnName != "")
                {
                    var creativeSet = context.CreativeSets.Where(c => c.CreativeSetID == creativeSetID).First();
                    creativeSet.GetType().GetProperty(columnName).SetValue(creativeSet, url);
                    creativeSet.Line.LastUpdatedDate = DateTime.Now;
                    context.SaveChanges();
                }
            }
            return url;
        }
        private class ImageResult
        {
            public string ColumnName { get; set; }
            public string URL { get; set; }

        }
        [HttpPost]
        public ActionResult CopySet(int OriginalSetID, int TargetSetID, string[] OriginalColumns, string[] TargetColumns )
        {
            try
            {
                

                    return Json(new { success = true, items = CopySetToList(OriginalSetID, TargetSetID, OriginalColumns, TargetColumns) });

            }
            catch (Exception e)
            {
                return Json(new { success = false });
            }
        }

        private ArrayList CopySetToList(int OriginalSetID, int TargetSetID, string[] OriginalColumns, string[] TargetColumns)
        {
            ApplicationDbContext context = new ApplicationDbContext();
            ArrayList result = new ArrayList();
            var originalSet = context.CreativeSets.Where(c => c.CreativeSetID == OriginalSetID).First();
            var originalType = originalSet.GetType();
            var targetSet = context.CreativeSets.Where(c => c.CreativeSetID == TargetSetID).First();

            var targetType = targetSet.GetType();

            if (originalType == typeof(PremiumAd))
            {
                var set = (PremiumAd)context.CreativeSets.OfType<PremiumAd>().Include(o => o.BannerAds).Include(o => o.NativAds).Where(c => c.CreativeSetID == OriginalSetID).First();


                if (targetType == typeof(BannerAds))
                {
                    originalSet = set.BannerAds;

                }
                else if(targetType == typeof(NativeAds))
                {
                    originalSet = set.NativAds;
                }
            }
            //copying to a premium
            if (targetType == typeof(PremiumAd))
            {
                var set = (PremiumAd)context.CreativeSets.OfType<PremiumAd>().Include(o => o.BannerAds).Include(o => o.NativAds).Where(c => c.CreativeSetID == TargetSetID).First();


                if (originalType == typeof(BannerAds))
                {
                    targetSet = set.BannerAds;

                }
                else if (originalType == typeof(NativeAds))
                {
                    targetSet = set.NativAds;
                }
            }

            if (targetType == typeof(PremiumAd) && originalType == typeof(PremiumAd))
            {
                result.AddRange(
                    CopySetToList(((PremiumAd)originalSet).BannerAds.CreativeSetID, ((PremiumAd)targetSet).BannerAds.CreativeSetID, OriginalColumns, TargetColumns));
                result.AddRange(
                    CopySetToList(((PremiumAd)originalSet).NativAds.CreativeSetID, ((PremiumAd)targetSet).NativAds.CreativeSetID, OriginalColumns, TargetColumns));
            }
            else
            {

                var imageLocation = "";
                int i = 0;
                foreach (string thisColumn in OriginalColumns)
                {
                    if (originalSet.GetType().GetProperty(thisColumn) != null)
                    {
                        imageLocation = $"{originalSet.GetType().GetProperty(thisColumn).GetValue(originalSet)}";
                        targetSet.GetType().GetProperty(TargetColumns[i]).SetValue(targetSet, imageLocation);
                        ImageResult item = new ImageResult() { ColumnName = TargetColumns[i], URL = imageLocation };
                        result.Add(item);
                    }
                    i++;

                }
                context.SaveChanges();
            }
            return result;

        }

        [HttpPost]
        public ActionResult DeleteFile(string CreativeSetID, string ColumnName)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var creativeSetID = int.Parse(CreativeSetID);
                var creativeSet = context.CreativeSets.Where(c => c.CreativeSetID == creativeSetID).First();
                var imageLocation = creativeSet.GetType().GetProperty(ColumnName).GetValue(creativeSet).ToString();  
                creativeSet.GetType().GetProperty(ColumnName).SetValue(creativeSet, null);

                context.SaveChanges();
                return Json(new { success = true, response = "File Deleted" }
                );
            }
            catch (Exception ex)
            {
                 return Json(new { success = false, response = "There was a problem deleiting your file." }
                );
            }
        }
        public ActionResult SaveColumn(string CreativeSetID, string  ColumnName, string NewValue)
        {
            try
            {
                object ValueToSet = NewValue;
                bool boolNewValue;

                if (bool.TryParse(NewValue, out boolNewValue))
                {
                    ValueToSet = boolNewValue;
                }
                ApplicationDbContext context = new ApplicationDbContext();
                var columnName = ColumnName.Replace("CreativeSet.", "");
                var creativeSetID = int.Parse(CreativeSetID);
                var creativeSet = context.CreativeSets.Where(c => c.CreativeSetID == creativeSetID).Include(cs => cs.Line).First();
              //  var line = context.LineItems.Where(l => l.Creative.Contains(creativeSet)).First();
                //line.LastUpdatedDate = DateTime.Now;
                creativeSet.GetType().GetProperty(columnName).SetValue(creativeSet, ValueToSet);
                context.SaveChanges();
                return Json(new { success = true, response = "Value Saved" }
               );
            }
            catch (Exception ex)
            {
                return Json(new { success = false, response = ex.Message }
                );
            }
        }

        [HttpGet]
        public ActionResult GetImageUrl(string CreativeSetID, string ColumnName)
        {
            try
            {
                ApplicationDbContext context = new ApplicationDbContext();
                var creativeSetID = int.Parse(CreativeSetID);
                var creativeSet = context.CreativeSets.Where(c => c.CreativeSetID == creativeSetID).First();
                var imageLocation = creativeSet.GetType().GetProperty(ColumnName).GetValue(creativeSet).ToString();
                //var rootedPath = Server.MapPath(imageLocation);
                return Json(new { success = true, response = imageLocation }, JsonRequestBehavior.AllowGet 
                );
            }
            catch(Exception ex)
            {
                return Json(new { success = true, respone = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        public ActionResult Add(int LineItemId, string GivenType)
        {
            CreativeSet s = (CreativeSet)Activator.CreateInstance(Type.GetType(GivenType));

            var context = new ApplicationDbContext();

            var lineItem = context.LineItems.Where(l => l.LineItemID == LineItemId).FirstOrDefault();
            var baseName = lineItem.Creative.OrderBy(m => m.CreativeSetID).FirstOrDefault().SetName;
            var index = lineItem.Creative.Count()+1;
            s.SetName = baseName.Replace("Set_1", $"Set_{index}");

            if(Type.GetType(GivenType) == typeof(PremiumAd))
            {
                BannerAds b = new BannerAds();
                NativeAds n = new NativeAds();
                ((PremiumAd)s).NativAds = n;
                ((PremiumAd)s).BannerAds = b;

            }
            lineItem.Creative.Add(s);

            context.SaveChanges();

            EditCreativeSettViewModel vm = new EditCreativeSettViewModel { CreativeSet = s, Index = index-1 };

            return PartialView("Edit", vm);
            
        }
        public bool Delete(int CreativeID)
        {
            var context = new ApplicationDbContext();
            var creativeSet = context.CreativeSets.Where(c => c.CreativeSetID == CreativeID).FirstOrDefault();
            var set = context.CreativeSets.Remove(creativeSet);
            context.SaveChanges();
            return true;
         
        }
        [HttpGet]
        public JsonResult GetCopyLists(string OrderID)
        {
            try
            {
                int orderID = int.Parse(OrderID);
                var context = new ApplicationDbContext();
                var creativeSets = context.CreativeSets.Where(c => c.Line.Order.ID == orderID).ToList();
                var nativeAds = creativeSets.Where(n => n.AdTypeText == CreativeSet.AdTypTextType.NativeAds).Select(s => new { s.SetName, s.CreativeSetID }).ToList();
                var bannerAds = creativeSets.Where(n => n.AdTypeText == CreativeSet.AdTypTextType.BannerAds).Select(s => new { s.SetName, s.CreativeSetID }).ToList();
                var singleAds = creativeSets.Where(n => n.AdTypeText == CreativeSet.AdTypTextType.SingleAds).Select(s => new { s.SetName, s.CreativeSetID }).ToList();
                var premiumAds = creativeSets.Where(n => n.AdTypeText == CreativeSet.AdTypTextType.PremiumAds).Select(s => new { s.SetName, s.CreativeSetID }).ToList();
                return Json(new { NativeAds = nativeAds, BannerAds = bannerAds, SingleAds = singleAds, PremiumAds = premiumAds }, JsonRequestBehavior.AllowGet);
            }
            catch(Exception ex) {
                return Json(new { SinleAds = "", NativeAds = "", BannerAds = "" }, JsonRequestBehavior.AllowGet);
            }
        }
        
    }
}