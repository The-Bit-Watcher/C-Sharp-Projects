using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HAS01.Models
{
    public class Booking
    {
        public class Bookings
        {
            public int Id { get; set; }
            public string BookingType { get; set; }
            public string ServiceType { get; set; }
            public int DriverId { get; set; }
            public Driver Driver { get; set; }
            public Vehicle Vehicle { get; set; }
            public DateTime BookingTime { get; set; }
            public string Status { get; set; }
        }

        public class Driver
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public string LicenseNumber { get; set; }

            public string Number { get; set; }
            public string DriverType { get; set; } //used at booking

            public string ImagePath { get; set; }
        }//might add status later

        public class Vehicle
        {
            public int Id { get; set; }
            public string RegistrationNumber { get; set; }
            public string VehicleType { get; set; }//used at booking
            public string ImagePath { get; set; }
        }//might add status later. For use in a more global online system that get used by many individuals. 
    }
}