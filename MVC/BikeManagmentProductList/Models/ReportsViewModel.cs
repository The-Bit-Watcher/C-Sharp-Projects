using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment3.Models
{
    public class ReportsViewModel
    {
        public List<SalesReportItem> SalesReport { get; set; }
        public List<PopularProductReport> PopularProducts { get; set; }
        public List<StaffPerformanceReport> StaffPerformance { get; set; }
        public List<CustomerOrderHistory> CustomerOrderHistory { get; set; }
        public List<MonthlySalesReport> MonthlySales { get; set; }
        public List<StockStatusReport> StockStatus { get; set; }
        public List<SavedReport> SavedReports { get; set; }
        public ReportFilter Filter { get; set; }
    }

    public class SalesReportItem
    {
        public string CustomerName { get; set; }
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public decimal ListPrice { get; set; }
        public int Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public string StaffName { get; set; }
        public DateTime OrderDate { get; set; }
    }

    public class PopularProductReport
    {
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public int TotalSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class StaffPerformanceReport
    {
        public string StaffName { get; set; }
        public string StoreName { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProductsSold { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal AverageOrderValue { get; set; }
    }

    public class CustomerOrderHistory
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string City { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public decimal TotalSpent { get; set; }
        public DateTime FirstOrderDate { get; set; }
        public DateTime LastOrderDate { get; set; }
    }

    public class MonthlySalesReport
    {
        public string MonthYear { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProductsSold { get; set; }
        public decimal TotalRevenue { get; set; }
    }

    public class StockStatusReport
    {
        public string ProductName { get; set; }
        public string BrandName { get; set; }
        public string CategoryName { get; set; }
        public int QuantityInStock { get; set; }
        public int TotalSold { get; set; }
        public string StockStatus { get; set; } // "Low", "Adequate", "Overstock"
    }

    public class ReportFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? StaffId { get; set; }
        public int? StoreId { get; set; }
        public int? CategoryId { get; set; }
        public string ReportType { get; set; }
    }

    public class SavedReport
    {
        public string FileName { get; set; }
        public string FileType { get; set; }
        public DateTime CreatedDate { get; set; }
        public long FileSize { get; set; }
        public string ReportType { get; set; }
    }
}