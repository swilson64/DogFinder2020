﻿using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace DogFinder
{
    public class UserAddressesController : Controller
    {
        private DogFinder1Entities db = new DogFinder1Entities();

        // GET: UserAddresses
        public ActionResult Index()
        {
            var userAddresses = db.UserAddresses.Include(u => u.AspNetUser);
            return View(userAddresses.ToList());
        }

        // GET: UserAddresses/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAddress userAddress = db.UserAddresses.Find(id);
            if (userAddress == null)
            {
                return HttpNotFound();
            }
            return View(userAddress);
        }

        // GET: UserAddresses/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserAddresses/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "AddressID,FirstLine,SecondLine,Town,PostCode")] UserAddress userAddress)
        {
            if (ModelState.IsValid)
            {
                if (String.IsNullOrEmpty(userAddress.SecondLine))
                {
                    userAddress.SecondLine = "";
                }
                userAddress.UserID = User.Identity.GetUserId();
                db.UserAddresses.Add(userAddress);
                db.SaveChanges();
                return RedirectToAction("Index","Manage");
            }
           
            return View(userAddress);
        }

        // GET: UserAddresses/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAddress userAddress = db.UserAddresses.Find(id);
            if (userAddress == null)
            {
                return HttpNotFound();
            }
            ViewBag.UserID = new SelectList(db.AspNetUsers, "Id", "Email", userAddress.UserID);
            return View(userAddress);
        }

        // POST: UserAddresses/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "AddressID,FirstLine,SecondLine,Town,PostCode")] UserAddress userAddress)
        {
            if (ModelState.IsValid)
            {
                userAddress.UserID = User.Identity.GetUserId();
                db.Entry(userAddress).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index", "Manage");
            }
            return View(userAddress);
        }

        // GET: UserAddresses/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UserAddress userAddress = db.UserAddresses.Find(id);
            if (userAddress == null)
            {
                return HttpNotFound();
            }
            return View(userAddress);
        }

        // POST: UserAddresses/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            UserAddress userAddress = db.UserAddresses.Find(id);
            db.UserAddresses.Remove(userAddress);
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
