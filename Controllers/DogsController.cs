using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using QRCoder;
using System.IO;
using System.Drawing;

namespace DogFinder
{
    public class DogsController : Controller
    {
        private DogFinder1Entities db = new DogFinder1Entities();

        private static Byte[] BitmapToBytes(Bitmap img)
        {
            using (MemoryStream stream = new MemoryStream())
            {
                img.Save(stream, System.Drawing.Imaging.ImageFormat.Png);
                return stream.ToArray();
            }
        }

        public ActionResult QRCode(int id)
        {
            var thisDog = db.Dogs.First(x => x.DogID == id);
            var allergies = thisDog.Allergies.Length > 0 ? "My allergies are: " + thisDog.Allergies : "I have no allergies";
            var userAddressRecord = db.UserAddresses.FirstOrDefault(x => x.UserID == thisDog.OwnerID);
            var addressString = "";
            if (userAddressRecord != null)
            {
                addressString = "My Owner's Address is " + userAddressRecord.FirstLine + " " + userAddressRecord.SecondLine + " " + userAddressRecord.Town + " " + userAddressRecord.PostCode;
            }
            else
            {
                addressString = "My Owner hasn't logged his address.";
            }
            
            var qrString = "This dog is called: " + thisDog.Name + ". My Owner's Email is " + thisDog.AspNetUser.Email + ". I like to eat " + thisDog.FavFood + ". " + allergies + ". " + addressString + ".";
            ViewBag.DogName = thisDog.Name;
            ViewBag.qrString = qrString;
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(qrString, QRCodeGenerator.ECCLevel.Q);
            QRCode qrCode = new QRCode(qrCodeData);
            Bitmap qrCodeImage = qrCode.GetGraphic(20);
            ViewBag.QRCode = BitmapToBytes(qrCodeImage);
            return View();
        }

        // GET: Dogs
        public async Task<ActionResult> Index()
        {
            var dogs = db.Dogs.Include(d => d.AspNetUser).Include(d=>d.AspNetUser.UserAddresses).Include(d => d.DogStatu).Include(d => d.DogSize).Include(d => d.DogType);
            return View(await dogs.ToListAsync());
        }
        public async Task<ActionResult> MyDogs()
        {
            var thisUser = User.Identity.GetUserId();
            var dogs = db.Dogs.Where(x=>x.AspNetUser.Id == thisUser).Include(d => d.AspNetUser).Include(d => d.AspNetUser.UserAddresses).Include(d => d.DogStatu).Include(d => d.DogSize).Include(d => d.DogType);
            return View(await dogs.ToListAsync());
        }

        // GET: Dogs/Details/5
        public async Task<ActionResult> Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dog dog = await db.Dogs.FindAsync(id);
            if (dog == null)
            {
                return HttpNotFound();
            }
            return View(dog);
        }

        // GET: Dogs/Create
        [Authorize]
        public ActionResult Create()
        {
            ViewBag.StatusID = new SelectList(db.DogStatus, "StatusID", "StatusName");
            ViewBag.SizeID = new SelectList(db.DogSizes, "SizeID", "SizeName");
            ViewBag.TypeID = new SelectList(db.DogTypes.OrderBy(x=>x.TypeName), "TypeID", "TypeName");
            return View();
        }

        // POST: Dogs/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "TypeID,Age,Sex,Name,StatusID,Description,FavFood,SizeID,Allergies,IsDangerous")] Dog dog)
        {
            if (ModelState.IsValid)
            {
                dog.OwnerID = User.Identity.GetUserId();
                db.Dogs.Add(dog);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            ViewBag.StatusID = new SelectList(db.DogStatus, "StatusID", "StatusName", dog.StatusID);
            ViewBag.SizeID = new SelectList(db.DogSizes, "SizeID", "SizeName", dog.SizeID);
            ViewBag.TypeID = new SelectList(db.DogTypes.OrderBy(x => x.TypeName), "TypeID", "TypeName", dog.TypeID);
            return View(dog);
        }

        // GET: Dogs/Edit/5
        public async Task<ActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dog dog = await db.Dogs.FindAsync(id);
            if (dog == null)
            {
                return HttpNotFound();
            }
            ViewBag.StatusID = new SelectList(db.DogStatus, "StatusID", "StatusName", dog.StatusID);
            ViewBag.SizeID = new SelectList(db.DogSizes, "SizeID", "SizeName", dog.SizeID);
            ViewBag.TypeID = new SelectList(db.DogTypes.OrderBy(x => x.TypeName), "TypeID", "TypeName", dog.TypeID);
            return View(dog);
        }

        // POST: Dogs/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "DogID,TypeID,Age,Sex,Name,StatusID,Description,FavFood,SizeID,Allergies,IsDangerous")] Dog dog)
        {
            if (ModelState.IsValid)
            {
                dog.OwnerID = User.Identity.GetUserId();
                db.Entry(dog).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.StatusID = new SelectList(db.DogStatus, "StatusID", "StatusName", dog.StatusID);
            ViewBag.SizeID = new SelectList(db.DogSizes, "SizeID", "SizeName", dog.SizeID);
            ViewBag.TypeID = new SelectList(db.DogTypes.OrderBy(x => x.TypeName), "TypeID", "TypeName", dog.TypeID);
            return View(dog);
        }

        // GET: Dogs/Delete/5
        public async Task<ActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Dog dog = await db.Dogs.FindAsync(id);
            if (dog == null)
            {
                return HttpNotFound();
            }
            return View(dog);
        }

        // POST: Dogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(long id)
        {
            Dog dog = await db.Dogs.FindAsync(id);
            db.Dogs.Remove(dog);
            await db.SaveChangesAsync();
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
