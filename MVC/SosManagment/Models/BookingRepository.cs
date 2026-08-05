using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static HAS01.Models.Booking;

namespace HAS01.Models
{
        public static class BookingRepository//add images plus urls in here later
    {
            public static List<Driver> Drivers = new List<Driver>
        {
            new Driver { Id = 1, Name = "John Smith", LicenseNumber = "DL12345", Number ="0824963527", DriverType = "Advanced Life Support", ImagePath = "~/Content/malePixels.png" },
            new Driver { Id = 2, Name = "Sarah Johnson", LicenseNumber = "DL67890", Number ="2539810527", DriverType = "Basic Life Support", ImagePath = "~/Content/femalePixels.jpg" },
            new Driver { Id = 3, Name = "Mike Brown", LicenseNumber = "DL54321", Number ="023507008", DriverType = "Air Ambulance", ImagePath = "~/Content/malePixels.jpg" }
        };

            public static List<Vehicle> Vehicles = new List<Vehicle>
        {
            new Vehicle { Id = 1, RegistrationNumber = "AMB001", VehicleType = "Advanced Life Support", ImagePath = "~/Content/advancedLifeSupport.png" },
            new Vehicle { Id = 2, RegistrationNumber = "AMB002", VehicleType = "Basic Life Support", ImagePath = "~/Content/BasicLifeSupport.jpg"},
            new Vehicle { Id = 3, RegistrationNumber = "AMB003", VehicleType = "Air Ambulance", ImagePath = "~/Content/airAmbulance.jpeg" }
        };

            public static List<Bookings> Bookings = new List<Bookings>();

            public static Driver GetRandomDriver()
            {
                var random = new Random();
                return Drivers[random.Next(Drivers.Count)];
            }

        public static Vehicle GetRandomVehicle()//need to add one for the regular booking. This works for the emergency booking.
        {
                var random = new Random();
                return Vehicles[random.Next(Vehicles.Count)];
            }

            public static void AddBooking(Bookings booking)
            {
                booking.Id = Bookings.Count + 1;
                Bookings.Add(booking);
            }
        }
    }