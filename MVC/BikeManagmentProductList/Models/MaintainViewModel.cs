using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment3.Models
{
    public class MaintainViewModel
    {
        public List<StaffViewModel> Staffs { get; set; }
        public List<CustomerViewModel> Customers { get; set; }
        public List<ProductViewModel> Products { get; set; }
        public staffs CurrentStaff { get; set; }
        public customers CurrentCustomer { get; set; }
        public products CurrentProduct { get; set; }
        public List<stores> Stores { get; set; }
        public List<staffs> Managers { get; set; }
        public List<brands> Brands { get; set; }
        public List<categories> Categories { get; set; }
    }

    public class StaffViewModel
    {
        public int StaffId { get; set; }
        public string StaffName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public bool Active { get; set; }
        public string StoreName { get; set; }
        public string ManagerName { get; set; }
    }

    public class CustomerViewModel
    {
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
    }
}