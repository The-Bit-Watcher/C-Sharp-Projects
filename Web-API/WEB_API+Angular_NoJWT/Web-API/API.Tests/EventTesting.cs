using HAS01.API.Controllers;
using HAS01.API.Data;
using HAS01.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace HAS01.API.Tests.Controllers
{
    public class EventControllerTests 
    {
        [Fact]
        public async Task GetAllEvents()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new AppDbContext(options);

            //test data
            context.Events.AddRange(
                new Events { Id = 1, EventName = "Test Event 1", Location = "Location 1", TicketPrice = 100 },
                new Events { Id = 2, EventName = "Test Event 2", Location = "Location 2", TicketPrice = 150 }
            );
            await context.SaveChangesAsync();

            var controller = new EventController(context);

            // Act
            var result = await controller.getEvents();

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }

        [Fact]
        public async Task GetEventById()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using var context = new AppDbContext(options);

            var testEvent = new Events { Id = 1, EventName = "Test Event", Location = "Test Location", TicketPrice = 100 };
            context.Events.Add(testEvent);
            await context.SaveChangesAsync();

            var controller = new EventController(context);

            // Act
            var result = await controller.getEvent(1);

            // Assert
            Assert.NotNull(result);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.NotNull(okResult.Value);
        }
    }
}