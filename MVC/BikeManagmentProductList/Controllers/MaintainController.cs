using Assignment3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Assignment3.Controllers
{
    public class MaintainController : Controller
    {
        private BikeStoresEntities1 db = new BikeStoresEntities1();

        // GET: Maintain
        public async Task<ActionResult> Index()
        {
            var viewModel = new MaintainViewModel
            {
                Staffs = await GetStaffDataAsync(),
                Customers = await GetCustomerDataAsync(),
                Products = await GetProductDataAsync(),
                CurrentStaff = new staffs(),
                CurrentCustomer = new customers(),
                CurrentProduct = new products(),
                Stores = await db.stores.ToListAsync(),
                Managers = await db.staffs.ToListAsync(),
                Brands = await db.brands.ToListAsync(),
                Categories = await db.categories.ToListAsync()
            };

            return View(viewModel);
        }
        public async Task<ActionResult> DeleteProduct(int id)
        {
            var product = await db.products.FindAsync(id);
            if (product == null)
            {
                return HttpNotFound();
            }

            return PartialView("_DeleteProduct", product);
        }

        // POST: Delete Product
        [HttpPost, ActionName("DeleteProduct")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteProductConfirmed(int id)
        {
            products product = await db.products.FindAsync(id);
            db.products.Remove(product);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        #region Helper Methods

        private async Task<List<StaffViewModel>> GetStaffDataAsync()
        {
            return await db.staffs
                .Include(s => s.stores)
                .Include(s => s.staffs2)
                .Select(s => new StaffViewModel
                {
                    StaffId = s.staff_id,
                    StaffName = s.first_name + " " + s.last_name,
                    Email = s.email,
                    Phone = s.phone,
                    Active = s.active == 1,
                    StoreName = s.stores.store_name,
                    ManagerName = s.staffs2 != null ? s.staffs2.first_name + " " + s.staffs2.last_name : "No Manager"
                })
                .ToListAsync();
        }

        private async Task<List<CustomerViewModel>> GetCustomerDataAsync()
        {
            return await db.customers
                .Select(c => new CustomerViewModel
                {
                    CustomerId = c.customer_id,
                    CustomerName = c.first_name + " " + c.last_name,
                    Email = c.email,
                    Phone = c.phone,
                    Street = c.street,
                    City = c.city,
                    State = c.state,
                    ZipCode = c.zip_code
                })
                .ToListAsync();
        }

        private async Task<List<ProductViewModel>> GetProductDataAsync()
        {
            return await db.products
                .Include(p => p.brands)
                .Include(p => p.categories)
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

        private async Task<ActionResult> ReloadMaintainPageWithStaff(staffs staff)
        {
            var viewModel = new MaintainViewModel
            {
                Staffs = await GetStaffDataAsync(),
                Customers = await GetCustomerDataAsync(),
                Products = await GetProductDataAsync(),
                CurrentStaff = staff,
                CurrentCustomer = new customers(),
                CurrentProduct = new products(),
                Stores = await db.stores.ToListAsync(),
                Managers = await db.staffs.ToListAsync(),
                Brands = await db.brands.ToListAsync(),
                Categories = await db.categories.ToListAsync()
            };

            return View("Index", viewModel);
        }

        private async Task<ActionResult> ReloadMaintainPageWithCustomer(customers customer)
        {
            var viewModel = new MaintainViewModel
            {
                Staffs = await GetStaffDataAsync(),
                Customers = await GetCustomerDataAsync(),
                Products = await GetProductDataAsync(),
                CurrentStaff = new staffs(),
                CurrentCustomer = customer,
                CurrentProduct = new products(),
                Stores = await db.stores.ToListAsync(),
                Managers = await db.staffs.ToListAsync(),
                Brands = await db.brands.ToListAsync(),
                Categories = await db.categories.ToListAsync()
            };

            return View("Index", viewModel);
        }

        private async Task<ActionResult> ReloadMaintainPageWithProduct(products product)
        {
            var viewModel = new MaintainViewModel
            {
                Staffs = await GetStaffDataAsync(),
                Customers = await GetCustomerDataAsync(),
                Products = await GetProductDataAsync(),
                CurrentStaff = new staffs(),
                CurrentCustomer = new customers(),
                CurrentProduct = product,
                Stores = await db.stores.ToListAsync(),
                Managers = await db.staffs.ToListAsync(),
                Brands = await db.brands.ToListAsync(),
                Categories = await db.categories.ToListAsync()
            };

            return View("Index", viewModel);
        }

        #endregion

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