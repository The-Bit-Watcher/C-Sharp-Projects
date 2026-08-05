using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAS01.Models
{
    public class ManageViewModel
    {
        public List<Booking.Driver> Drivers { get; set; }
        public List<Booking.Vehicle> Vehicles { get; set; }
    }
}