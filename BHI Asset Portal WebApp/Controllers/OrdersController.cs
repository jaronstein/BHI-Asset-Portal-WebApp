using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Net.Mail;
using BHI_Asset_Portal_WebApp.Models;
using System.Configuration;
using System.ComponentModel.DataAnnotations;
using System.IO.Compression;
using System.IO;
using System.Reflection;  // reflection namespace


namespace BHI_Asset_Portal_WebApp.Controllers
{
    public class OrdersController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Orders
        [Authorize]
        public ActionResult Index()
        {
            
            var currentUserID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            
            return View(db.Orders.Where(o => o.Advertiser.Id == currentUserID && o.Complete != true).ToList());
        }

        // GET: Orders/Details/5
        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order orderModel = db.Orders.Where(m => m.ID == id).Include(o => o.Advertiser).Include(o => o.SalesPerson).Include(o => o.AccountManager).FirstOrDefault();
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            ViewBag.View = "TraffickerView";
            return View("Edit", orderModel);
            
        }

        // GET: Orders/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Orders/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "BHIOrderNumber,AccountID,CreatedDate,CampaignName,SalesPerson,AccountManager,ContractStartDate,ContractEndDate,LastSubmittedDate,LastSavedDate,ScreenShotUrl,Comments")] Order orderModel)
        {
            if (ModelState.IsValid)
            {
                db.Orders.Add(orderModel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(orderModel);
        }

        // GET: Orders/Edit/5
        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                ViewData["Message"] = "The order you are looking for does not exist. Ensure you have the correct Order ID, and try again.";
                return View("Error");
            }
            try
            {
                Order orderModel = db.Orders.Where(m => m.ID == id).Include(o => o.Advertiser).Include(o => o.SalesPerson).Include(o => o.AccountManager).FirstOrDefault();
                var currentUserID = User.Identity.GetUserId();
                if (orderModel.AgentName is null)
                {
                    var user = db.Users.Where(u => u.Id == currentUserID).FirstOrDefault();
                    orderModel.AgentName = user.Name;
                    orderModel.AgentEmail = user.Email;
                }
                orderModel.LineItems = orderModel.LineItems.OrderBy(l => l.Creative.Select(c => c.SortOrder).FirstOrDefault()).ToArray();
                foreach (var l in orderModel.LineItems)
                {
                    foreach (var e in l.Creative)
                    {
                        if (e.GetType() == typeof(PremiumAd))
                        {
                            PremiumAd p = db.CreativeSets.OfType<PremiumAd>().Where(c => c.CreativeSetID == e.CreativeSetID).Include(a => a.BannerAds).Include(a => a.NativAds).FirstOrDefault();
                            ((PremiumAd)e).BannerAds = p.BannerAds;
                            ((PremiumAd)e).NativAds = p.NativAds;


                        }
                    }
                }

                if (orderModel == null)
                {
                    ViewData["Message"] = "The order you are looking for does not exist. Ensure you have the correct Order ID, and try again.";
                    return View("Error");
                }
                return View(orderModel);
            }
            catch(Exception ex)
            {
                ViewData["Message"] = "The order you are looking for does not exist. Ensure you have the correct Order ID, and try again.";
                return View("Error");
            }
        }
        private bool SendEmail(string To, string Subject, string Body)
        {
            string from = ConfigurationManager.AppSettings["FromEmail"];
            
            MailMessage mail = new MailMessage(from, To, Subject, Body);
            mail.IsBodyHtml = true;
            using (var smtp = new SmtpClient())
            {
                var credential = new NetworkCredential
                {
                    UserName = ConfigurationManager.AppSettings["FromUN"],  // replace with valid value
                    Password = ConfigurationManager.AppSettings["FromPW"]
                };
                smtp.Credentials = credential;
                smtp.Host = ConfigurationManager.AppSettings["SMTPServer"];
                smtp.Port = 587;
                smtp.EnableSsl = true;
                
                smtp.Send(mail);
                return true;
            }
        }
        [HttpPost]
        public ActionResult SaveOrderData(int OrderID, string ColumnName, string NewValue)
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
                
                var order = context.Orders.Where(c => c.ID == OrderID).First();
                order.GetType().GetProperty(ColumnName).SetValue(order, ValueToSet);
                context.SaveChanges();
                return Json(new { success = true, response = "Value Saved" }
               );
            }
            catch (Exception ex)
            {
                return Json(new { success = false, response = "There was a problem deleiting your file." }
                );
            }
        }

        [HttpPost]
        public ActionResult UpsertOrder(APIOrders order, string secret)
        {
            try
            {
                ValidationContext vc = new ValidationContext(order); // The simplest form of validation context. It contains only a reference to the object being validated.

                ICollection<ValidationResult> results = new List<ValidationResult>(); // Will contain the results of the validation
                bool isValid = Validator.TryValidateObject(order, vc, results, true);
                if(!isValid)
                {
                    string message = "The following fields are either missing or not in the correct format:";
                    foreach(var r in results)
                    {
                        foreach (var m in r.MemberNames)
                            message += r.MemberNames + ",";
                    }
                    message.TrimEnd(new char[] { ','});
                    return Json(new { succes = false, ErrorMessage = message });

                }
                ApplicationDbContext context = new ApplicationDbContext();
                var newOrder = false;
           
                if (secret == ConfigurationManager.AppSettings["APISecret"])
                {
                    var o = context.Orders.Where(c => c.BHIOrderNumber == order.workOrderNumber).FirstOrDefault();
                    if (o == null)
                    {
                        o = new Order();
                        context.Orders.Add(o);
                        newOrder = true;
                    }
                    o.BHIOrderNumber = order.workOrderNumber;
                    o.OrderName = order.orderName;
                    o.ContractStartDate = order.contractStartDate;
                    o.ContractEndDate = order.contractEndDate;


                    foreach (var p in order.products)
                    {
                        var line = new LineItem();
                        if (!newOrder)
                        {
                            line = context.LineItems.Where(c => c.Order.BHIOrderNumber == o.BHIOrderNumber && c.Taxonomy == p.campaignName).FirstOrDefault();

                        }
                        if (line == null)
                        {
                            line = new LineItem();
                            context.LineItems.Add(line);
                        }


                        line.Taxonomy = p.campaignName;
                        line.Order = o;
                        line.LocationSublocation = p.advertisingLocation + "/" + p.advertisingSubLocation;
                        line.LocationSublocation += p.advertisingCommunity == null ? "" : "/" + p.advertisingCommunity;
                        line.StartDate = p.applicableStartDate;
                        line.EndDate = p.applicableEndDate;
                        line.Product = p.productName;


                        if (line.Creative == null || line.Creative.Count == 0)
                        {
                            CreativeSet set;
                            if (p.productName.Contains("BDX Premium"))
                            {
                                set = new PremiumAd();
                            }
                            else if (p.productName.Contains("BDX Native"))
                            {
                                set = new NativeAds();
                            }
                            else
                            {
                                set = new NativeAds();
                            }
                            set.SortOrder = 0;
                            set.SetName = p.campaignName + "_Set_1";
                            
                            set.Line = line;
                            context.CreativeSets.Add(set);
                        }


                    }

                    context.SaveChanges();


                    return Json(new { success = true, AssetPortalID = o.ID });
                }
                else
                {
                    return Json(new { succes = false, ErrorMessage = "Secret Key is invalid" });
                }
            }
            catch(Exception ex)
            {
                return Json(new { succes = false, message = ex.Message });

            }
        }
        
        [HttpPost]
        public ActionResult SendEmailToAM(int OrderID, string AgentName, string AgentEmail)
        {
            try {
                var order = db.Orders.Where(o => o.ID == OrderID).Include(o=>o.AccountManager).Include(o=>o.LineItems).First();
                
                var subject = $"Order {order.BHIOrderNumber} has been edited in the digital asset portal and is ready for your review";
                var message = $"{AgentName} has sumitted information into the digital asset portal for Order Number: {order.ID}." +
                    $" You can view all of the order details <a href='{ Request.Url.GetLeftPart(UriPartial.Authority)}/Orders/Edit/{OrderID}'>here.</a>";
                if (order.LastSubmittedDate != null)
                {
                    message += $"<p>The following line items have been edited:</p>";
                    message += $"<ul>";
                    foreach(var l in order.LineItems.Where(l=> l.LastUpdatedDate > order.LastSubmittedDate))
                    {
                        message += $"<li> {l.Product}:{l.LocationSublocation}:{l.StartDate.Value.ToShortDateString()}";
                    }
                    message += "</ul>";
                }
                order.AgentEmail = AgentEmail;
                order.AgentName = AgentName;
                order.LastSubmittedDate = DateTime.Now;
                db.SaveChangesAsync();
                if (order.AccountManager != null)
                {
                    var toEmail = order.AccountManager.Email;
                    SendEmail(toEmail, subject, message);
                }
                SendEmail(AgentEmail, subject, message);
                return Json(new
                {
                    success = true,
                    response = DateTime.Now.ToString()
                });
            }
            catch(Exception ex)
            {
                return Json(new
                {
                    success = false,
                    response = "Email Not Sent."
                });
            }

        }

        // POST: Orders/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "OrderID,BHIOrderNumber,AccountID,CreatedDate,CampaignName,SalesPerson,AccountManager,ContractStartDate,ContractEndDate,LastSubmittedDate,LastSavedDate,ScreenShotUrl,Comments")] Order orderModel)
        {
            if (ModelState.IsValid)
            {
                db.Entry(orderModel).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(orderModel);
        }

        // GET: Orders/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Order orderModel = db.Orders.Find(id);
            if (orderModel == null)
            {
                return HttpNotFound();
            }
            return View(orderModel);
        }

        // POST: Orders/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Order orderModel = db.Orders.Find(id);
            db.Orders.Remove(orderModel);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [ HttpGet, ActionName("ZipFiles")]
        public FileResult ZipFiles(int OrderID)
        {
            var order = db.Orders.Where(o => o.ID == OrderID).Include(o => o.AccountManager).Include(o => o.LineItems).First();
            var lines = order.LineItems;
            var fullFileStream = new MemoryStream();

            using (fullFileStream)

            using (var majorArchive = new ZipArchive(fullFileStream, ZipArchiveMode.Create, false))
            {


                foreach (var l in lines)
                {
                    var majorEntry = majorArchive.CreateEntry(l.Taxonomy); 

                    var compressedFileStream = new MemoryStream();

                    using (compressedFileStream)

                    //compressedFileStream.Seek(0, SeekOrigin.Begin);

                    using (var zipArchive = new ZipArchive(compressedFileStream, ZipArchiveMode.Create, false))
                    {

                        foreach (var c in l.Creative)
                        {
                            if (c.GetType() != typeof(PremiumAd))
                            {
                                //  zipArchive = ExtractImages(((PremiumAd)c).NativAds, zipArchive);
                            
                            Type type = c.GetType();
                            // get all public static properties of MyClass type
                            var propertyInfos = type.GetProperties();
                                foreach (var p in propertyInfos)
                                {
                                    if (p.Name.Contains("Image"))
                                    {
                                        //Create a zip entry for each attachment
                                        var zipEntry = zipArchive.CreateEntry(c.SetName + p.Name);
                                        string name = p.Name;
                                        if (c.GetType().GetProperty(p.Name).GetValue(c) != null)
                                        {
                                            var url = c.GetType().GetProperty(p.Name).GetValue(c).ToString();

                                            var webClient = new WebClient();
                                            byte[] imageBytes = webClient.DownloadData(url);
                                            //Get the stream of the attachment
                                            using (var originalFileStream = new MemoryStream(imageBytes))
                                            using (var zipEntryStream = zipEntry.Open())
                                            {
                                                //Copy the attachment stream to the zip entry stream
                                                originalFileStream.CopyTo(zipEntryStream);
                                            }
                                        }
                                    }
                                }
                            }

                        }
                    }

                    var zipThis = compressedFileStream.ToArray();

                    using (var itemFileStream = new MemoryStream(zipThis))

                    using (var majorEntryStream = majorEntry.Open())
                    {
                        //Copy the attachment stream to the zip entry stream
                        itemFileStream.CopyTo(majorEntryStream);

                    }


                    //return File(zipthis, "zip");


                }
                try
                {
                    var majorZip = fullFileStream.ToArray();
                    return File(majorZip, "zip");
                }
                catch(Exception ex)
                {
                    return File(new byte[0], "");
                }
            }
            return File(new byte[0], "zip");
        }

        private ZipArchive ExtractImages(CreativeSet c, ZipArchive zipArchive)
        {
            Type type = c.GetType();
            // get all public static properties of MyClass type
            var propertyInfos = type.GetProperties();
            foreach (var p in propertyInfos)
            {
                if (p.Name.Contains("Image"))
                {
                    //Create a zip entry for each attachment
                    var zipEntry = zipArchive.CreateEntry(c.SetName + p.Name);
                    string name = p.Name;
                    if (c.GetType().GetProperty(p.Name).GetValue(c) != null)
                    {
                        var url = c.GetType().GetProperty(p.Name).GetValue(c).ToString();

                        var webClient = new WebClient();
                        byte[] imageBytes = webClient.DownloadData(url);
                        //Get the stream of the attachment
                        using (var originalFileStream = new MemoryStream(imageBytes))
                        using (var zipEntryStream = zipEntry.Open())
                        {
                            //Copy the attachment stream to the zip entry stream
                            originalFileStream.CopyTo(zipEntryStream);
                        }
                    }
                }
            }
            return zipArchive;
        }
        [Authorize]
        public ActionResult PastLineItems()
        {
            var currentUserID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var lineItems = db.Orders.Where(s => s.Advertiser.Id == currentUserID).SelectMany(s => s.LineItems).ToList();
            return View("LineItemList", lineItems);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
