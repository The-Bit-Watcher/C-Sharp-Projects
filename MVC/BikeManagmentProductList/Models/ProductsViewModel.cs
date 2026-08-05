using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment3.Models
{
    public class ProductsViewModel
    {
        public ProductsViewModel()
        {
            Products = new List<ProductViewModel>();
            Brands = new List<brands>();
            Categories = new List<categories>();
            NewProduct = new products();
        }

        public List<ProductViewModel> Products { get; set; }
        public List<brands> Brands { get; set; }
        public List<categories> Categories { get; set; }
        public products NewProduct { get; set; }
    }
}