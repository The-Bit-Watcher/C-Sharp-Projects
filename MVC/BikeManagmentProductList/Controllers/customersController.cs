using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Assignment3.Models;

namespace Assignment3.Controllers
{
    public class customersController : Controller
    {
        private BikeStoresEntities1 db = new BikeStoresEntities1();

        // GET: customers
        public async Task<ActionResult> Index()
        {
            return View(await db.customers.ToListAsync());
        }

        // GET: customers/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customers customers = await db.customers.FindAsync(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // GET: customers/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: customers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "customer_id,first_name,last_name,phone,email,street,city,state,zip_code")] customers customers)
        {
            if (ModelState.IsValid)
            {
                db.customers.Add(customers);
                await db.SaveChangesAsync();
                return RedirectToAction("CustomerPurchases");
            }

            // If validation fails, return to CustomerPurchases with the form data
            var purchaseData = GetPurchaseData();
            var viewModel = new CombinedPurchaseCreateViewModel
            {
                CustomerPurchases = purchaseData,
                NewCustomer = customers //preserve the form data with validation errors
            };

            return View("CustomerPurchases", viewModel);
        }

        private List<PurchaseViewModel> GetPurchaseData()
        {
            return (from c in db.customers
                    select new PurchaseViewModel
                    {
                        CustomerId = c.customer_id,
                        CustomerName = c.first_name + " " + c.last_name,
                        Email = c.email,
                        Purchases = (from o in c.orders
                                     from oi in o.order_items
                                     select new PurchaseItemViewModel
                                     {
                                         ProductName = oi.products.product_name,
                                         ListPrice = oi.list_price,
                                         OrderDate = o.order_date
                                     }).ToList()
                    }).ToList();
        }

        // GET: customers/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customers customers = await db.customers.FindAsync(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // POST: customers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "customer_id,first_name,last_name,phone,email,street,city,state,zip_code")] customers customers)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customers).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(customers);
        }

        // GET: customers/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            customers customers = await db.customers.FindAsync(id);
            if (customers == null)
            {
                return HttpNotFound();
            }
            return View(customers);
        }

        // POST: customers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            customers customers = await db.customers.FindAsync(id);
            db.customers.Remove(customers);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        public ActionResult CustomerPurchases()
        {
            var data = (from c in db.customers
                        select new PurchaseViewModel
                        {
                            CustomerId = c.customer_id,
                            CustomerName = c.first_name + " " + c.last_name,
                            Email = c.email,
                            Purchases = (from o in c.orders
                                         from oi in o.order_items
                                         select new PurchaseItemViewModel
                                         {
                                             ProductName = oi.products.product_name,
                                             ListPrice = oi.list_price,
                                             OrderDate = o.order_date
                                         }).ToList()
                        }).ToList();
            var ViewModel = new CombinedPurchaseCreateViewModel
            {
                CustomerPurchases = data,
                NewCustomer = new customers()
            };//added viewModel to make it possible to create new customer on same page
            //when using the modalpopup.
            return View(ViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateFromModal(customers newCustomer)
        {
            if (ModelState.IsValid)
            {
                db.customers.Add(newCustomer);
                await db.SaveChangesAsync();
                return RedirectToAction("CustomerPurchases");
            }

            // If validation fails, return to the same page with errors
            var purchaseData = (from c in db.customers
                                select new PurchaseViewModel
                                {
                                    CustomerId = c.customer_id,
                                    CustomerName = c.first_name + " " + c.last_name,
                                    Email = c.email,
                                    Purchases = (from o in c.orders
                                                 from oi in o.order_items
                                                 select new PurchaseItemViewModel
                                                 {
                                                     ProductName = oi.products.product_name,
                                                     ListPrice = oi.list_price,
                                                     OrderDate = o.order_date
                                                 }).ToList()
                                }).ToList();

            var viewModel = new CombinedPurchaseCreateViewModel
            {
                CustomerPurchases = purchaseData,
                NewCustomer = newCustomer // This will preserve the form data with validation errors
            };

            return View("CustomerPurchases", viewModel);
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
