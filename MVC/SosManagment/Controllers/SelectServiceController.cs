using HAS01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HAS01.Models.Booking;

namespace HAS01.Controllers
{
    public class SelectServiceController : Controller
    {
        //get the type of service from the post request. Then sends it to the Confirmed or to be Booking View. And redirescts to the Booking Page
        public ActionResult BookingForm(string serviceType)
        {
            Console.WriteLine("Selected Service: " + serviceType);
            ViewBag.ServiceType = serviceType;

            var model = new BookingFormViewModel
            {
                ServiceType = serviceType,
                AvDrivers = BookingRepository.Drivers.ToList(),
                AvVehicles = BookingRepository.Vehicles.ToList()
            };

            return View(model);
        }

        public ActionResult SubmitBooking(BookingFormViewModel model)
        {
            var driver = BookingRepository.Drivers.FirstOrDefault(d => d.Id == model.DriverId);
            var vehicle = BookingRepository.Vehicles.FirstOrDefault(v => v.Id == model.VehicleId);

            if (!ModelState.IsValid)
            {
                model.AvDrivers = BookingRepository.Drivers.ToList();
                model.AvVehicles = BookingRepository.Vehicles.ToList();
                ViewBag.ServiceType = model.ServiceType;

                return View("BookingForm", model);
            }

            var booking = new Booking.Bookings
            {
                BookingType = "Regular",
                ServiceType = model.ServiceType,
                Driver = driver,
                Vehicle = vehicle,
                DriverId = driver.Id,
                BookingTime = DateTime.Now,
                Status = "Confirmed"
            };

            BookingRepository.AddBooking(booking);
            return RedirectToAction("Confirmed", new { id = booking.Id });
        }

        public ActionResult Confirmed(int id)
        {
            ViewBag.AllDrivers = BookingRepository.Drivers;
            ViewBag.AllVehicles = BookingRepository.Vehicles;
            return View("Confirmed");
        }

    }
}