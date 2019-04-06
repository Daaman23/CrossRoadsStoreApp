using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CrossRoadsStoreApp.Models;

namespace CrossRoadsStoreApp.Controllers
{
    public class ProductsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Products
        public ActionResult Index()
        {
            var products = db.Products.Include(p => p.Category);
            return View(products.ToList());
        }

        // GET: Products/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Products/Create
        //[Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "ProductId,CategoryId,Name,Description,Price,Image")] Product product)
        {
            if (ModelState.IsValid)
            {
                //Checks to see if a file was uploaded, adds file name to product and saves file.
                if (Request.Files != null)
                {
                    var img = Request.Files[0];

                    if (img.FileName != null && img.ContentLength > 0)
                    {
                        string path = Server.MapPath("~/Content/Images/" + img.FileName);
                        img.SaveAs(path);
                        product.Image = img.FileName;
                    }
                }

                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Edit/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "ProductId,CategoryId,Name,Description,Price,Image")] Product product)
        {
            if (ModelState.IsValid)
            {
                //Because we are reusing data (image name/file), had to work around certain exceptions.
                var updatedProduct = db.Products.Find(product.ProductId);
                updatedProduct.CategoryId = product.CategoryId;
                updatedProduct.Name = product.Name;
                updatedProduct.Description = product.Description;
                updatedProduct.Price = product.Price;

                //Checks post request for file, if found, adds file name to the product and saves the file.
                if (Request.Files != null)
                {
                    var img = Request.Files[0];

                    //Double checks for file.
                    if (img.FileName != null && img.ContentLength > 0)
                    {
                        //Removes the old image from server files.
                        if (updatedProduct.Image != null)
                        {
                            string oldPath = Server.MapPath("~/Content/Images/") + updatedProduct.Image;
                            FileInfo file = new FileInfo(oldPath);
                            if (file.Exists)
                            {
                                file.Delete();
                            }
                        }

                        string path = Server.MapPath("~/Content/Images/" + img.FileName);
                        img.SaveAs(path);
                        updatedProduct.Image = img.FileName;
                    }
                }

                //db.Entry(product).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CategoryId = new SelectList(db.Categories, "CategoryId", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        //[Authorize(Roles = "Admin")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            //If the product has an image, deletes the image from the server.
            if (product.Image != null)
            {
                string path = Server.MapPath("~/Content/Images/") + product.Image;
                FileInfo file = new FileInfo(path);
                if (file.Exists)
                {
                    file.Delete();
                }
            }
            db.Products.Remove(product);
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
        
        public ActionResult ListProductsByCategory(int id)
        {
            var products = db.Products.Where(p => p.CategoryId == id);
            return View(products.ToList());
        }
    }
}
