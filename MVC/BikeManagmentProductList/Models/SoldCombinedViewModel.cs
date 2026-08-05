using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Assignment3.Models
{
    public class SoldCombinedViewModel
    {
        public SoldCombinedViewModel()
        {
            Staffs = new List<SoldViewModel>();
            NewStaff = new staffs();
        }

        public List<SoldViewModel> Staffs { get; set; }
        public staffs NewStaff { get; set; }

        public SelectList StoreList { get; set; }
        public SelectList ManagerList { get; set; }

    }
}