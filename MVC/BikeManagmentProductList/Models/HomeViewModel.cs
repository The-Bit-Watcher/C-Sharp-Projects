using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment3.Models
{
    public class HomeViewModel
    {
        public HomeViewModel()
        {
            ProductsData = new ProductsViewModel();
            CustomersData = new CombinedPurchaseCreateViewModel();
            StaffData = new SoldCombinedViewModel();
        }

        public ProductsViewModel ProductsData { get; set; }
        public CombinedPurchaseCreateViewModel CustomersData { get; set; }
        public SoldCombinedViewModel StaffData { get; set; }
    }
}