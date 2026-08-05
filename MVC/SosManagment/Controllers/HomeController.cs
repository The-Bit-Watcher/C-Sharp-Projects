using HAS01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static HAS01.Models.Booking;

namespace HAS01.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult History()
        {
            return View();
        }

        public ActionResult Manage()
        {
            return View();
        }

        public ActionResult SelectService()
        {
            return View();
        }


        public ActionResult EmergencyBooking()
        {
            var randomDriver = BookingRepository.GetRandomDriver();
            var randomVehicle = BookingRepository.GetRandomVehicle();

            var booking = new Bookings
            {
                BookingType = "Emergency",
                Driver = randomDriver,
                Vehicle = randomVehicle,
                BookingTime = DateTime.Now,
                Status = "Confirmed"
            };

            BookingRepository.AddBooking(booking);

            ViewBag.ID = booking.Id;//send booking id. Can then on page display all the details of the booking

            return RedirectToAction("Confirmed", new { id = booking.Id });//goes to bookingConfirmed page with the booking id
        }

        public ActionResult Confirmed(int id)
        {
            var booking = BookingRepository.Bookings.FirstOrDefault(b => b.Id == id);

            if (booking == null)
            {
                return HttpNotFound("Booking not found.");
            }

            return View(booking); // Pass the booking model to the view
        }
    }
}