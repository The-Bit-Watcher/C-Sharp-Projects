using Assignment3.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Assignment3.Controllers
{
    public class HomeController : Controller
    {
        private BikeStoresEntities1 db = new BikeStoresEntities1();

            public async Task<ActionResult> Index()
            {
                try
                {
                    var viewModel = new HomeViewModel
                    {
                    ProductsData = await GetProductsViewModel(),
                    CustomersData = await GetCustomerPurchasesViewModel(),
                    StaffData = await GetStaffSalesViewModel()
                    };

                    return View(viewModel);
                }
                catch (Exception ex)
                {
                    // Log the error
                    System.Diagnostics.Debug.WriteLine($"Error in Index: {ex.Message}");

                    // Return view with empty data
                    var viewModel = new HomeViewModel
                    {
                        ProductsData = new ProductsViewModel
                        {
                        Products = new List<ProductViewModel>(),
                        Brands = new List<brands>(),
                        Categories = new List<categories>(),
                        NewProduct = new products()
                        },
                        CustomersData = new CombinedPurchaseCreateViewModel(),
                        StaffData = new SoldCombinedViewModel()
                    };

                    return View(viewModel);
                }
            }

            public ActionResult About()
            {
                ViewBag.Message = "Your application description page.";
                return View();
            }

            public ActionResult Contact()
            {
                ViewBag.Message = "Your contact page.";
                return View();
            }

            // Products Data
            private async Task<ProductsViewModel> GetProductsViewModel()
            {
                try
                {
                    var products = await (from p in db.products
                                      join b in db.brands on p.brand_id equals b.brand_id
                                      join c in db.categories on p.category_id equals c.category_id
                                      select new ProductViewModel
                                      {
                                          ProductId = p.product_id,
                                          ProductName = p.product_name,
                                          BrandName = b.brand_name,
                                          CategoryName = c.category_name,
                                          ModelYear = p.model_year,
                                          ListPrice = p.list_price
                                      }).ToListAsync();

                    var brands = await db.brands.ToListAsync() ?? new List<brands>();
                    var categories = await db.categories.ToListAsync() ?? new List<categories>();

                    return new ProductsViewModel
                    {
                    Products = products ?? new List<ProductViewModel>(),
                    Brands = brands,
                    Categories = categories,
                    NewProduct = new products()
                    };
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in GetProductsViewModel: {ex.Message}");
                    return new ProductsViewModel(); // This will use the initialized collections
                }
            }

            // Customers Data (reusing logic from customersController)
            private async Task<CombinedPurchaseCreateViewModel> GetCustomerPurchasesViewModel()
            {
                try
                {
                    var data = await (from c in db.customers
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
                                  }).ToListAsync();

                    return new CombinedPurchaseCreateViewModel
                    {
                    CustomerPurchases = data ?? new List<PurchaseViewModel>(),
                    NewCustomer = new customers()
                    };
                }
                catch (Exception ex)
                {
                System.Diagnostics.Debug.WriteLine($"Error in GetCustomerPurchasesViewModel: {ex.Message}");
                return new CombinedPurchaseCreateViewModel(); // This will use the initialized collections
                }
            }

            // Staff Data (reusing logic from staffsController)
            private async Task<SoldCombinedViewModel> GetStaffSalesViewModel()
            {
                try
                {
                    var data = await (from s in db.staffs
                                  select new SoldViewModel
                                  {
                                      StaffId = s.staff_id,
                                      StaffName = s.first_name + " " + s.last_name,
                                      Email = s.email,
                                      Solds = (from o in s.orders
                                               from oi in o.order_items
                                               select new SoldItemViewModel
                                               {
                                                   ProductName = oi.products.product_name,
                                                   ListPrice = oi.list_price,
                                                   OrderDate = o.order_date
                                               }).ToList()
                                  }).ToListAsync();

                    var stores = await db.stores.ToListAsync() ?? new List<stores>();
                    var staffs = await db.staffs.ToListAsync() ?? new List<staffs>();

                    ViewBag.store_id = new SelectList(stores, "store_id", "store_name");
                    ViewBag.manager_id = new SelectList(staffs, "staff_id", "first_name");

                    return new SoldCombinedViewModel
                    {
                        Staffs = data ?? new List<SoldViewModel>(),
                        NewStaff = new staffs()
                    };
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error in GetStaffSalesViewModel: {ex.Message}");
                    return new SoldCombinedViewModel(); // This will use the initialized collections
                }
            }

        // Handle form submissions from the dashboard with proper error handling

        [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> CreateCustomer(CombinedPurchaseCreateViewModel model)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.customers.Add(model.NewCustomer);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    // Log and handle validation errors
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError($"NewCustomer.{validationError.PropertyName}", validationError.ErrorMessage);
                            System.Diagnostics.Debug.WriteLine($"Customer Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the customer: " + ex.Message);
                }

                // If validation fails, return to dashboard with errors
                var viewModel = new HomeViewModel
                {
                    ProductsData = await GetProductsViewModel(),
                    CustomersData = model, // Preserve the submitted customer data with errors
                    StaffData = await GetStaffSalesViewModel()
                };

                return View("Index", viewModel);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> CreateStaff(SoldCombinedViewModel model)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.staffs.Add(model.NewStaff);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    // Log and handle validation errors
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError($"NewStaff.{validationError.PropertyName}", validationError.ErrorMessage);
                            System.Diagnostics.Debug.WriteLine($"Staff Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the staff: " + ex.Message);
                }

                // If validation fails, return to dashboard with errors
                var viewModel = new HomeViewModel
                {
                    ProductsData = await GetProductsViewModel(),
                    CustomersData = await GetCustomerPurchasesViewModel(),
                    StaffData = model
                };

                return View("Index", viewModel);
            }

            [HttpPost]
            [ValidateAntiForgeryToken]
            public async Task<ActionResult> CreateProduct(ProductsViewModel model)
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        db.products.Add(model.NewProduct);
                        await db.SaveChangesAsync();
                        return RedirectToAction("Index");
                    }
                }
                catch (DbEntityValidationException ex)
                {
                    // Log and handle validation errors
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError($"NewProduct.{validationError.PropertyName}", validationError.ErrorMessage);
                            System.Diagnostics.Debug.WriteLine($"Product Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "An error occurred while saving the product: " + ex.Message);
                }

                // If validation fails, return to dashboard with errors
                var viewModel = new HomeViewModel
                {
                    ProductsData = model, // Preserve the submitted product data with errors
                    CustomersData = await GetCustomerPurchasesViewModel(),
                    StaffData = await GetStaffSalesViewModel()
                };

                return View("Index", viewModel);
            }

            [HttpPost]
            public async Task<ActionResult> ApplyProductFilters(string brandFilter, string categoryFilter)
            {
                // This method handles the product filtering from the dashboard
                var productsData = await GetProductsViewModel();

                // Apply filters if provided
                if (!string.IsNullOrEmpty(brandFilter))
                {
                    productsData.Products = productsData.Products
                        .Where(p => p.BrandName == brandFilter)
                        .ToList();
                }

                if (!string.IsNullOrEmpty(categoryFilter))
                {
                    productsData.Products = productsData.Products
                        .Where(p => p.CategoryName == categoryFilter)
                        .ToList();
                }

                var viewModel = new HomeViewModel
                {
                    ProductsData = productsData,
                    CustomersData = await GetCustomerPurchasesViewModel(),
                    StaffData = await GetStaffSalesViewModel()
                };

                ViewBag.SelectedBrand = brandFilter;
                ViewBag.SelectedCategory = categoryFilter;

                return View("Index", viewModel);
            }

            // Add these methods to pre-populate required fields or set default values
            private void SetCustomerDefaults(customers customer)
            {
                if (customer.phone == null)
                    customer.phone = "";
                if (customer.street == null)
                    customer.street = "";
                if (customer.city == null)
                    customer.city = "";
                if (customer.state == null)
                    customer.state = "";
                if (customer.zip_code == null)
                    customer.zip_code = "";
            }

            private void SetStaffDefaults(staffs staff)
            {
                if (staff.phone == null)
                    staff.phone = "";
                if (staff.active == null)
                    staff.active = 1; // Default to active
                if (staff.manager_id == null)
                    staff.manager_id = 1; // Set default manager if needed
            }

            private void SetProductDefaults(products product)
            {
                if (product.model_year == null)
                    product.model_year = (short)DateTime.Now.Year;
            }
        }
    }
