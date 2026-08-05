using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment3.Models
{
    public class CombinedPurchaseCreateViewModel
    {

        public CombinedPurchaseCreateViewModel()
        {
            CustomerPurchases = new List<PurchaseViewModel>();
            NewCustomer = new customers();
        }
        public List<PurchaseViewModel> CustomerPurchases { get; set; }
        public customers NewCustomer { get; set; }
    }
}