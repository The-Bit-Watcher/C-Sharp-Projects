using Assignment3.Models;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Assignment3.Controllers
{
    public class ReportsController : Controller
    {
        // GET: Reports
        private BikeStoresEntities1 db = new BikeStoresEntities1();

        // GET: Reports
        public async Task<ActionResult> Index()
        {
            var viewModel = new ReportsViewModel
            {
                SalesReport = await GetSalesReportAsync(),
                PopularProducts = await GetPopularProductsReportAsync(),
                StaffPerformance = await GetStaffPerformanceReportAsync(),
                CustomerOrderHistory = await GetCustomerOrderHistoryAsync(),
                MonthlySales = await GetMonthlySalesReportAsync(),
                StockStatus = await GetStockStatusReportAsync(),
                SavedReports = await GetSavedReportsAsync(),
                Filter = new ReportFilter()
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> Index(ReportFilter filter)
        {
            var viewModel = new ReportsViewModel
            {
                SalesReport = await GetSalesReportAsync(filter),
                PopularProducts = await GetPopularProductsReportAsync(filter),
                StaffPerformance = await GetStaffPerformanceReportAsync(filter),
                CustomerOrderHistory = await GetCustomerOrderHistoryAsync(filter),
                MonthlySales = await GetMonthlySalesReportAsync(filter),
                StockStatus = await GetStockStatusReportAsync(),
                SavedReports = await GetSavedReportsAsync(),
                Filter = filter
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> ExportReport(ReportFilter filter, string fileName, string fileType)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = $"Report_{DateTime.Now:yyyyMMdd_HHmmss}";
            }

            var reportData = await GetReportDataAsync(filter);

            string filePath = SaveReportToFile(reportData, fileName, fileType);

            // Add to saved reports
            await SaveReportMetadataAsync(fileName, fileType, filePath);

            return RedirectToAction("Index");
        }

        public FileResult DownloadReport(string fileName)
        {
            string filePath = Path.Combine(Server.MapPath("~/App_Data/Reports"), fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
            string mimeType = GetMimeType(fileName);
            return File(fileBytes, mimeType, fileName);
        }

        [HttpPost]
        public async Task<ActionResult> DeleteReport(string fileName)
        {
            string filePath = Path.Combine(Server.MapPath("~/App_Data/Reports"), fileName);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }

            // Remove from saved reports metadata
            await RemoveReportMetadataAsync(fileName);

            return RedirectToAction("Index");
        }

        #region Report Generation Methods

        private async Task<List<SalesReportItem>> GetSalesReportAsync(ReportFilter filter = null)
        {
            var query = db.order_items
                .Include("products")
                .Include("products.brands")
                .Include("products.categories")
                .Include("orders")
                .Include("orders.customers")
                .Include("orders.staffs")
                .Include("orders.staffs.stores")
                .AsQueryable();

            if (filter != null)
            {
                if (filter.StartDate.HasValue)
                    query = query.Where(oi => oi.orders.order_date >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(oi => oi.orders.order_date <= filter.EndDate.Value);

                if (filter.StaffId.HasValue)
                    query = query.Where(oi => oi.orders.staff_id == filter.StaffId.Value);

                if (filter.CategoryId.HasValue)
                    query = query.Where(oi => oi.products.category_id == filter.CategoryId.Value);
            }

            return await query
                .Select(oi => new SalesReportItem
                {
                    CustomerName = oi.orders.customers.first_name + " " + oi.orders.customers.last_name,
                    ProductName = oi.products.product_name,
                    BrandName = oi.products.brands.brand_name,
                    CategoryName = oi.products.categories.category_name,
                    ListPrice = oi.list_price,
                    Quantity = oi.quantity,
                    TotalAmount = oi.list_price * oi.quantity,
                    StaffName = oi.orders.staffs.first_name + " " + oi.orders.staffs.last_name,
                    OrderDate = oi.orders.order_date
                })
                .OrderByDescending(x => x.OrderDate)
                .ToListAsync();
        }

        private async Task<List<PopularProductReport>> GetPopularProductsReportAsync(ReportFilter filter = null)
        {
            var query = db.order_items
                .Include("products")
                .Include("products.brands")
                .Include("products.categories")
                .Include("orders")
                .AsQueryable();

            if (filter != null)
            {
                if (filter.StartDate.HasValue)
                    query = query.Where(oi => oi.orders.order_date >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(oi => oi.orders.order_date <= filter.EndDate.Value);
            }

            return await query
                .GroupBy(oi => new { oi.product_id, oi.products.product_name, oi.products.brands.brand_name, oi.products.categories.category_name })
                .Select(g => new PopularProductReport
                {
                    ProductName = g.Key.product_name,
                    BrandName = g.Key.brand_name,
                    CategoryName = g.Key.category_name,
                    TotalSold = g.Sum(oi => oi.quantity),
                    TotalRevenue = g.Sum(oi => oi.list_price * oi.quantity)
                })
                .OrderByDescending(x => x.TotalSold)
                .Take(10)
                .ToListAsync();
        }

        private async Task<List<StaffPerformanceReport>> GetStaffPerformanceReportAsync(ReportFilter filter = null)
        {
            var query = db.orders
                .Include(o => o.staffs)
                .Include(o => o.staffs.stores)
                .Include(o => o.order_items)
                .AsQueryable();

            if (filter != null)
            {
                if (filter.StartDate.HasValue)
                    query = query.Where(o => o.order_date >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(o => o.order_date <= filter.EndDate.Value);

                if (filter.StoreId.HasValue)
                    query = query.Where(o => o.staffs.store_id == filter.StoreId.Value);
            }

            return await query
                .GroupBy(o => new { o.staff_id, o.staffs.first_name, o.staffs.last_name, o.staffs.stores.store_name })
                .Select(g => new StaffPerformanceReport
                {
                    StaffName = g.Key.first_name + " " + g.Key.last_name,
                    StoreName = g.Key.store_name,
                    TotalOrders = g.Count(),
                    TotalProductsSold = g.SelectMany(o => o.order_items).Sum(oi => oi.quantity),
                    TotalRevenue = g.SelectMany(o => o.order_items).Sum(oi => oi.list_price * oi.quantity),
                    AverageOrderValue = g.SelectMany(o => o.order_items).Sum(oi => oi.list_price * oi.quantity) / g.Count()
                })
                .OrderByDescending(x => x.TotalRevenue)
                .ToListAsync();
        }

        private async Task<List<CustomerOrderHistory>> GetCustomerOrderHistoryAsync(ReportFilter filter = null)
        {
            var query = db.orders
                .Include(o => o.customers)
                .Include(o => o.order_items)
                .AsQueryable();

            if (filter != null)
            {
                if (filter.StartDate.HasValue)
                    query = query.Where(o => o.order_date >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(o => o.order_date <= filter.EndDate.Value);
            }

            return await query
                .GroupBy(o => new { o.customer_id, o.customers.first_name, o.customers.last_name, o.customers.email, o.customers.city })
                .Select(g => new CustomerOrderHistory
                {
                    CustomerName = g.Key.first_name + " " + g.Key.last_name,
                    Email = g.Key.email,
                    City = g.Key.city,
                    TotalOrders = g.Count(),
                    TotalProducts = g.SelectMany(o => o.order_items).Sum(oi => oi.quantity),
                    TotalSpent = g.SelectMany(o => o.order_items).Sum(oi => oi.list_price * oi.quantity),
                    FirstOrderDate = g.Min(o => o.order_date),
                    LastOrderDate = g.Max(o => o.order_date)
                })
                .OrderByDescending(x => x.TotalSpent)
                .Take(15)
                .ToListAsync();
        }

        private async Task<List<MonthlySalesReport>> GetMonthlySalesReportAsync(ReportFilter filter = null)
        {
            var query = db.orders
                .Include(o => o.order_items)
                .AsQueryable();

            if (filter != null)
            {
                if (filter.StartDate.HasValue)
                    query = query.Where(o => o.order_date >= filter.StartDate.Value);

                if (filter.EndDate.HasValue)
                    query = query.Where(o => o.order_date <= filter.EndDate.Value);
            }

            var monthlyData = await query
                .GroupBy(o => new { Year = o.order_date.Year, Month = o.order_date.Month })
                .Select(g => new
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    TotalOrders = g.Count(),
                    TotalProductsSold = g.SelectMany(o => o.order_items).Sum(oi => oi.quantity),
                    TotalRevenue = g.SelectMany(o => o.order_items).Sum(oi => oi.list_price * oi.quantity)
                })
                .OrderBy(x => x.Year).ThenBy(x => x.Month)
                .ToListAsync();

            // Now format the MonthYear outside of the LINQ to Entities query
            return monthlyData.Select(g => new MonthlySalesReport
            {
                Year = g.Year,
                Month = g.Month,
                MonthYear = $"{g.Year}-{g.Month:00}", // This is now in memory, so string.Format works
                TotalOrders = g.TotalOrders,
                TotalProductsSold = g.TotalProductsSold,
                TotalRevenue = g.TotalRevenue
            }).ToList();
        }

        private async Task<List<StockStatusReport>> GetStockStatusReportAsync()
        {
            var products = await db.products
                .Include(p => p.brands)
                .Include(p => p.categories)
                .Include(p => p.stocks)
                .Include(p => p.order_items)
                .ToListAsync();

            return products.Select(p => new StockStatusReport
            {
                ProductName = p.product_name,
                BrandName = p.brands.brand_name,
                CategoryName = p.categories.category_name,
                QuantityInStock = p.stocks.Sum(s => s.quantity.GetValueOrDefault()),
                TotalSold = p.order_items.Sum(oi => oi.quantity),
                StockStatus = GetStockStatus(
                    p.stocks.Sum(s => s.quantity.GetValueOrDefault()),
                    p.order_items.Sum(oi => oi.quantity)
                )
            })
                .OrderByDescending(x => x.QuantityInStock)
                .ToList();
        }

        private string GetStockStatus(int quantityInStock, int totalSold)
        {
            if (quantityInStock == 0) return "Out of Stock";
            if (quantityInStock < 5) return "Low Stock";
            if (quantityInStock > totalSold * 2) return "Overstock";
            return "Adequate";
        }

        #endregion

        #region File Export Methods

        private string SaveReportToFile(ReportsViewModel reportData, string fileName, string fileType)
        {
            string directory = Server.MapPath("~/App_Data/Reports");
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            string filePath = Path.Combine(directory, $"{fileName}.{fileType.ToLower()}");

            switch (fileType.ToLower())
            {
                case "xlsx":
                    ExportToExcel(reportData, filePath);
                    break;
                case "pdf":
                    ExportToPdf(reportData, filePath);
                    break;
                case "csv":
                    ExportToCsv(reportData, filePath);
                    break;
            }

            return filePath;
        }

        private void ExportToExcel(ReportsViewModel reportData, string filePath)
        {
            using (var workbook = new XLWorkbook())
            {
                // Sales Report Worksheet
                var wsSales = workbook.Worksheets.Add("Sales Report");
                // Add sales data...

                // Popular Products Worksheet
                var wsPopular = workbook.Worksheets.Add("Popular Products");
                // Add popular products data...

                workbook.SaveAs(filePath);
            }
        }

        private void ExportToPdf(ReportsViewModel reportData, string filePath)
        {
            // Implement PDF export using iTextSharp
        }

        private void ExportToCsv(ReportsViewModel reportData, string filePath)
        {
            // Implement CSV export
        }

        #endregion

        #region Helper Methods

        private async Task<ReportsViewModel> GetReportDataAsync(ReportFilter filter)
        {
            return new ReportsViewModel
            {
                SalesReport = await GetSalesReportAsync(filter),
                PopularProducts = await GetPopularProductsReportAsync(filter),
                StaffPerformance = await GetStaffPerformanceReportAsync(filter),
                CustomerOrderHistory = await GetCustomerOrderHistoryAsync(filter),
                MonthlySales = await GetMonthlySalesReportAsync(filter),
                StockStatus = await GetStockStatusReportAsync(),
                Filter = filter
            };
        }

        private async Task<List<SavedReport>> GetSavedReportsAsync()
        {
            string directory = Server.MapPath("~/App_Data/Reports");
            if (!Directory.Exists(directory))
            {
                return new List<SavedReport>();
            }

            var files = Directory.GetFiles(directory)
                .Select(file => new SavedReport
                {
                    FileName = Path.GetFileName(file),
                    FileType = Path.GetExtension(file).TrimStart('.'),
                    CreatedDate = System.IO.File.GetCreationTime(file),
                    FileSize = new FileInfo(file).Length
                })
                .OrderByDescending(f => f.CreatedDate)
                .ToList();

            return await Task.FromResult(files);
        }

        private async Task SaveReportMetadataAsync(string fileName, string fileType, string filePath)
        {
            // Could save to database if needed
            await Task.CompletedTask;
        }

        private async Task RemoveReportMetadataAsync(string fileName)
        {
            // Could remove from database if needed
            await Task.CompletedTask;
        }

        private string GetMimeType(string fileName)
        {
            string extension = Path.GetExtension(fileName).ToLower();

            if (extension == ".pdf")
                return "application/pdf";
            else if (extension == ".xlsx")
                return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            else if (extension == ".csv")
                return "text/csv";
            else
                return "application/octet-stream";
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