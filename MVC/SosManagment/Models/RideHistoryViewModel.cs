using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAS01.Models
{
    public class RideHistoryViewModel
    {       
            public string BookingId { get; set; }
            public string Date { get; set; }
            public string DriverName { get; set; }
            public string AmbulanceType { get; set; }
            public string Address { get; set; }
            public bool IsSOS { get; set; }       
    }
}