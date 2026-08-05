using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Assignment3.Models
{
    public class SoldViewModel
    {
        public int StaffId{ get; set; }
        public string StaffName { get; set; }
        public string Email { get; set; }
        public List<SoldItemViewModel> Solds { get; set; }

    }
}