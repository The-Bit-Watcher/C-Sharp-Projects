using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment3.Models
{
    public class PurchaseViewModel
    {            
            public int CustomerId { get; set; }
            public string CustomerName { get; set; } 
            public string Email { get; set; }
            public List<PurchaseItemViewModel> Purchases { get; set; }
    }
}