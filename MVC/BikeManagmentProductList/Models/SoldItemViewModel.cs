using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment3.Models
{
    public class SoldItemViewModel
    {
        public string ProductName { get; set; }
        public decimal ListPrice { get; set; }
        public DateTime OrderDate { get; set; }
    }
}