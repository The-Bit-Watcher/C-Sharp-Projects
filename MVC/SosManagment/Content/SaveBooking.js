function saveBookingToLocalStorage() {
    const booking = {
        serviceType: document.querySelector('[name="ServiceType"]').value,
        fullName: document.querySelector('[name="FullName"]').value,
        phone: document.querySelector('[name="Phone"]').value,
        pickUpTime: document.querySelector('[name="PickUpTime"]').value,
        address: document.querySelector('[name="Address"]').value,
        reason: document.querySelector('[name="Reason"]').value,
        vehicleId: document.querySelector('[name="VehicleId"]').value,
        driverId: document.querySelector('[name="DriverId"]').value,
        bookingTime: new Date().toISOString()
    };

    localStorage.setItem("latestBooking", JSON.stringify(booking));
}


