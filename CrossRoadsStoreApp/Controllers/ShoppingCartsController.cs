 using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CrossRoadsStoreApp.Models;
using Microsoft.AspNet.Identity;

namespace CrossRoadsStoreApp.Controllers
{
    public class ShoppingCartsController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: ShoppingCarts
        public ActionResult Index()
        {
            CartOwner();
            var ownerName = Session["CartOwnerName"].ToString();
            //Returns only the ShoppingCart items that match the user Session name.
            var shoppingCarts = db.ShoppingCarts.Where(sc => sc.Owner == ownerName)
                                                .Include(s => s.Product);
                                                
            return View(shoppingCarts.ToList());
        }

        // GET: ShoppingCarts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null || db.ShoppingCarts.Find(id).Owner != Session["CartOwnerName"].ToString())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);
            if (shoppingCart == null)
            {
                return HttpNotFound();
            }
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name");
            return View();
        }

        // POST: ShoppingCarts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Create([Bind(Include = "ShoppingCartId,ProductId,Quantity,Price,Owner")] ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                db.ShoppingCarts.Add(shoppingCart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", shoppingCart.ProductId);
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Edit/5
        [Authorize(Roles = "Admin")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);
            if (shoppingCart == null)
            {
                return HttpNotFound();
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", shoppingCart.ProductId);
            return View(shoppingCart);
        }

        // POST: ShoppingCarts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Edit([Bind(Include = "ShoppingCartId,ProductId,Quantity,Price,Owner")] ShoppingCart shoppingCart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(shoppingCart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ProductId = new SelectList(db.Products, "ProductId", "Name", shoppingCart.ProductId);
            return View(shoppingCart);
        }

        // GET: ShoppingCarts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null || db.ShoppingCarts.Find(id).Owner != Session["CartOwnerName"].ToString())
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);
            if (shoppingCart == null)
            {
                return HttpNotFound();
            }
            return View(shoppingCart);
        }

        // POST: ShoppingCarts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ShoppingCart shoppingCart = db.ShoppingCarts.Find(id);

            if (db.ShoppingCarts.Find(id).Owner == Session["CartOwnerName"].ToString())
            {
                db.ShoppingCarts.Remove(shoppingCart);
                db.SaveChanges();
            }
            
            return RedirectToAction("Index");
        }

        public ActionResult AddToCart(int id)
        {
            CartOwner();
            var ownerName = Session["CartOwnerName"].ToString();
            var item = db.ShoppingCarts.SingleOrDefault(sc => sc.Owner == ownerName
                                                              && sc.ProductId == id);
            if(item == null)
            {
                ShoppingCart cart = new ShoppingCart();
                Product product = db.Products.Find(id);

                cart.ProductId = id;
                cart.Quantity = 1;
                cart.Owner = Session["CartOwnerName"].ToString();
                cart.Price = product.Price;

                db.ShoppingCarts.Add(cart);
                db.SaveChanges();
            }
            else
            {
                item.Quantity += 1;
                db.SaveChanges();
            }

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

        //Checks/sets session variable CartOwnerName to allow anonymous shopping.
        private void CartOwner()
        {
            if(Session["CartOwnerName"] == null)
            {
                if(User.Identity.IsAuthenticated)
                {
                    Session["CartOwnerName"] = User.Identity.GetUserId();
                }
                else
                {
                    Session["CartOwnerName"] = Guid.NewGuid();
                }
            }
        }
    }
}
