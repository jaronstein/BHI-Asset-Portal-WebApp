using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using BHI_Asset_Portal_WebApp.Models;

namespace BHI_Asset_Portal_WebApp
{
    public class RealtorDotComsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: RealtorDotComs
        public ActionResult Index()
        {
            List<RealtorDotCom> list = new List<RealtorDotCom>();
            var items = db.CreativeSets.ToList();
            foreach(var i in items)
            {
                if (i.GetType() == typeof(RealtorDotCom))
                {
                    list.Add((RealtorDotCom)i);
                }
            }
            return View();
        }

        // GET: RealtorDotComs/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealtorDotCom realtorDotCom = (RealtorDotCom)db.CreativeSets.Find(id);
            if (realtorDotCom == null)
            {
                return HttpNotFound();
            }
            return View(realtorDotCom);
        }

        // GET: RealtorDotComs/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: RealtorDotComs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "CreativeSetID,SpecialInstructions,Valid,ScreenShotUrl,CreativeImageURL,CreativeLandingPageURL")] RealtorDotCom realtorDotCom)
        {
            if (ModelState.IsValid)
            {
                db.CreativeSets.Add(realtorDotCom);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(realtorDotCom);
        }

        // GET: RealtorDotComs/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealtorDotCom realtorDotCom = (RealtorDotCom)db.CreativeSets.Find(id);
            if (realtorDotCom == null)
            {
                return HttpNotFound();
            }
            return View(realtorDotCom);
        }

        // POST: RealtorDotComs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "CreativeSetID,SpecialInstructions,Valid,ScreenShotUrl,CreativeImageURL,CreativeLandingPageURL")] RealtorDotCom realtorDotCom)
        {
            if (ModelState.IsValid)
            {
                db.Entry(realtorDotCom).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(realtorDotCom);
        }

        // GET: RealtorDotComs/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RealtorDotCom realtorDotCom = (RealtorDotCom)db.CreativeSets.Find(id);
            if (realtorDotCom == null)
            {
                return HttpNotFound();
            }
            return View(realtorDotCom);
        }

        // POST: RealtorDotComs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            RealtorDotCom realtorDotCom = (RealtorDotCom)db.CreativeSets.Find(id);
            db.CreativeSets.Remove(realtorDotCom);
            db.SaveChanges();
            return RedirectToAction("Index");
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
