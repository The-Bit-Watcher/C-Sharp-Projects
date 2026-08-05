using HAS01.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HAS01.Controllers
{
    public class ManageController : Controller
    {

        public ActionResult Manage()
        {
            var model = new ManageViewModel
            {
                Drivers = BookingRepository.Drivers.ToList(),
                Vehicles = BookingRepository.Vehicles.ToList()
            };
            return View(model); 
        }

        [HttpGet]
        public ActionResult CreateDriver()
        {
            return View(new Booking.Driver());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateDriver(Booking.Driver driver, HttpPostedFileBase imageFile)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {

                        var imagePath = Server.MapPath("~/Content/DriverImages");
                        if (!Directory.Exists(imagePath))
                        {
                            Directory.CreateDirectory(imagePath);
                        }

                        var fileName = Path.GetFileName(imageFile.FileName);
                        var path = Path.Combine(imagePath, fileName);
                        imageFile.SaveAs(path);
                        driver.ImagePath = "/Content/DriverImages/" + fileName;
                    }

                    driver.Id = BookingRepository.Drivers.Any() ?
                               BookingRepository.Drivers.Max(d => d.Id) + 1 : 1;


                    BookingRepository.Drivers.Add(driver);

                    return RedirectToAction("Manage");
                }

                return View(driver);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Unable to save changes. Try again, and if the problem persists see your system administrator.");
                return View(driver);
            }
        }
        public ActionResult CreateVehicle()
        {
            return View(new Booking.Vehicle());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateVehicle(Booking.Vehicle vehicle, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.ContentLength > 0)
                {
                    var fileName = Path.GetFileName(imageFile.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/VehicleImages"), fileName);
                    imageFile.SaveAs(path);
                    vehicle.ImagePath = "/Content/VehicleImages/" + fileName;
                }

                vehicle.Id = BookingRepository.Vehicles.Count + 1;
                BookingRepository.Vehicles.Add(vehicle);
                return RedirectToAction("Manage");
            }
            return View(vehicle);
        }

        [HttpGet]
        public ActionResult EditDriver(int id)
        {
            var driver = BookingRepository.Drivers.FirstOrDefault(d => d.Id == id);
            if (driver == null)
            {
                return HttpNotFound();
            }
            return View(driver);
        }

        [HttpPost]
        public ActionResult EditDriver(Booking.Driver driver, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingDriver = BookingRepository.Drivers.FirstOrDefault(d => d.Id == driver.Id);
                if (existingDriver != null)
                {
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/DriverImages"), fileName);
                        imageFile.SaveAs(path);
                        existingDriver.ImagePath = "/Content/DriverImages/" + fileName;
                    }
                    else if (!string.IsNullOrEmpty(driver.ImagePath))
                    {
                        existingDriver.ImagePath = driver.ImagePath;
                    }

                    existingDriver.Name = driver.Name;
                    existingDriver.Number = driver.Number;
                    existingDriver.LicenseNumber = driver.LicenseNumber;
                    existingDriver.DriverType = driver.DriverType;
                }
                return RedirectToAction("Manage");
            }
            return View(driver);
        }

        [HttpGet]
        public ActionResult EditVehicle(int id)
        {
            var vehicle = BookingRepository.Vehicles.FirstOrDefault(v => v.Id == id);
            if (vehicle == null)
            {
                return HttpNotFound();
            }
            return View(vehicle);
        }

        [HttpPost]
        public ActionResult EditVehicle(Booking.Vehicle vehicle, HttpPostedFileBase imageFile)
        {
            if (ModelState.IsValid)
            {
                var existingVehicle = BookingRepository.Vehicles.FirstOrDefault(v => v.Id == vehicle.Id);
                if (existingVehicle != null)
                {
                    if (imageFile != null && imageFile.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(imageFile.FileName);
                        var path = Path.Combine(Server.MapPath("~/Content/VehicleImages"), fileName);
                        imageFile.SaveAs(path);
                        existingVehicle.ImagePath = "/Content/VehicleImages/" + fileName;
                    }
                    else if (!string.IsNullOrEmpty(vehicle.ImagePath))
                    {
                        existingVehicle.ImagePath = vehicle.ImagePath;
                    }

                    existingVehicle.VehicleType = vehicle.VehicleType;
                    existingVehicle.RegistrationNumber = vehicle.RegistrationNumber;
                }
                return RedirectToAction("Manage");
            }
            return View(vehicle);
        }

        public ActionResult ExportVehicles()
            {
                var vehicles = BookingRepository.Vehicles.ToList();
                string exportData = "ID,Type,Registration,ServiceType\n";
                exportData += string.Join("\n", vehicles.Select(v => $"{v.Id},{v.VehicleType},{v.RegistrationNumber},{v.VehicleType}"));

                return File(System.Text.Encoding.UTF8.GetBytes(exportData), "text/plain", "vehicles_export.txt");
            }

        [HttpPost]
        public JsonResult DeleteVehicle(int id)
        {
            try
            {
                var vehicle = BookingRepository.Vehicles.FirstOrDefault(v => v.Id == id);
                if (vehicle != null)
                {
                    BookingRepository.Vehicles.Remove(vehicle);
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Vehicle not found" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}

