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
    public class productsController : Controller
    {
        private BikeStoresEntities1 db = new BikeStoresEntities1();

        // GET: products
        public async Task<ActionResult> Index(string brandFilter, string categoryFilter)
        {
            var viewModel = new ProductsViewModel
            {
                Products = await GetFilteredProductsAsync(brandFilter, categoryFilter),
                NewProduct = new products(),
                Brands = await db.brands.ToListAsync(),
                Categories = await db.categories.ToListAsync()
            };

            ViewBag.SelectedBrand = brandFilter;
            ViewBag.SelectedCategory = categoryFilter;

            return View(viewModel);
        }

        private async Task<List<ProductViewModel>> GetFilteredProductsAsync(string brandFilter, string categoryFilter)
        {
            var query = db.products.AsQueryable();

            if (!string.IsNullOrEmpty(brandFilter))
            {
                query = query.Where(p => p.brands.brand_name == brandFilter);
            }

            if (!string.IsNullOrEmpty(categoryFilter))
            {
                query = query.Where(p => p.categories.category_name == categoryFilter);
            }

            return await query
                .Select(p => new ProductViewModel
                {
                    ProductId = p.product_id,
                    ProductName = p.product_name,
                    BrandName = p.brands.brand_name,
                    CategoryName = p.categories.category_name,
                    ModelYear = p.model_year,
                    ListPrice = p.list_price
                })
                .ToListAsync();
        }

        // POST: Create Product
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create(products newProduct)
        {
            if (ModelState.IsValid)
            {
                db.products.Add(newProduct);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            // If validation fails, reload the page with errors
            var viewModel = new ProductsViewModel
            {
                Products = await GetFilteredProductsAsync(null, null),
                NewProduct = newProduct,
                Brands = await db.brands.ToListAsync(),
                Categories = await db.categories.ToListAsync()
            };

            return View("Index", viewModel);
        }

        // GET: products/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = await db.products.FindAsync(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // GET: products/Create
        public ActionResult Create()
        {
            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name");
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name");
            return View();
        }


        // GET: products/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = await db.products.FindAsync(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name", products.brand_id);
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", products.category_id);
            return View(products);
        }

        // POST: products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "product_id,product_name,brand_id,category_id,model_year,list_price")] products products)
        {
            if (ModelState.IsValid)
            {
                db.Entry(products).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewBag.brand_id = new SelectList(db.brands, "brand_id", "brand_name", products.brand_id);
            ViewBag.category_id = new SelectList(db.categories, "category_id", "category_name", products.category_id);
            return View(products);
        }

        // GET: products/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            products products = await db.products.FindAsync(id);
            if (products == null)
            {
                return HttpNotFound();
            }
            return View(products);
        }

        // POST: products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            products products = await db.products.FindAsync(id);
            db.products.Remove(products);
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
