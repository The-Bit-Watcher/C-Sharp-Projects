using HAS01.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HAS01.Controllers
{
    public class HistoryController : Controller
    {   
        public ActionResult Index()
        {
            var bookings = BookingRepository.Bookings;

            var viewModels = bookings.Select(b => new RideHistoryViewModel
            {
                BookingId = $"B-{b.Id:0000}-{b.BookingTime.Year}",
                Date = b.BookingTime.ToString("dd/MM/yyyy"),
                DriverName = b.Driver?.Name ?? "Unknown",
                AmbulanceType = b.Vehicle?.VehicleType ?? "Unknown",
                Address = "N/A", // You can include it from form data if you store it in Bookings
                IsSOS = b.BookingType == "Emergency"
            }).ToList();

            return View(viewModels);
        }
    }
}