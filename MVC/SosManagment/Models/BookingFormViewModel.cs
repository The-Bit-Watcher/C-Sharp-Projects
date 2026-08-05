using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace HAS01.Models
{
    public class BookingFormViewModel
    {
        [Required]
        public string ServiceType { get; set; }

        [Required]
        public string FullName { get; set; }

        [Required]
        public string Phone { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public DateTime PickUpTime { get; set; }

        [Required]
        public string Reason { get; set; }

        [Required]
        public int VehicleId { get; set; }

        [Required]
        public int DriverId { get; set; }

        public List<Booking.Vehicle> AvVehicles { get; set; }
        public List<Booking.Driver> AvDrivers { get; set; }
    }
    }