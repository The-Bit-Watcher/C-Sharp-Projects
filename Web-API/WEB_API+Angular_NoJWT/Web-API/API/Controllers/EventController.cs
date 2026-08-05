using HAS01.API;
using HAS01.API.Data;
using HAS01.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HAS01.API.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
	public class EventController : ControllerBase
	{

		private readonly AppDbContext _context;
		public EventController(AppDbContext context)
		{
			_context = context;
		}

		[HttpGet]
		public async Task<ActionResult<Events>> getEvents()
		{
			var events = await _context.Events.ToListAsync();
			return Ok(events);
		}

		[HttpGet("{id}")]
		public async Task<ActionResult<Events>> getEvent(int id)
		{
			var ev = await _context.Events.FindAsync(id);
			if (ev == null)
			{
				return NotFound();
			}

			return Ok(ev);
		}

        [HttpDelete("{id}")]
		public async Task<IActionResult> deleteEvent(int id)
		{
			var ev = await _context.Events.FindAsync(id);
			if (ev == null)
			{
				return NotFound();
			}

			_context.Events.Remove(ev);
			await _context.SaveChangesAsync();

			return NoContent();
		}

        [HttpPut("{id}")]
		public async Task<IActionResult> updateEvent(int id, Events updatedEvent)
		{
			if (id != updatedEvent.Id)
			{
				return BadRequest();
			}

			var ev = await _context.Events.FindAsync(id);
			if (ev == null)
			{
				return NotFound();
			}

			ev.EventName = updatedEvent.EventName;
			ev.Location = updatedEvent.Location;
			ev.TicketPrice = updatedEvent.TicketPrice;

			_context.Entry(ev).State = EntityState.Modified;
			await _context.SaveChangesAsync();

			return NoContent();
		}

		[HttpPost]
		public async Task<IActionResult> addEvent(Events newEvent)
		{
            if (newEvent == null)
                return BadRequest("Event data is required");

            if (string.IsNullOrWhiteSpace(newEvent.EventName))
                return BadRequest("Event name is required");

            if (string.IsNullOrWhiteSpace(newEvent.Location))
                return BadRequest("Location is required");

            if (newEvent.TicketPrice < 0)
                return BadRequest("Ticket price must be positive");

            var ev = new Events
            {
                EventName = newEvent.EventName,
                Location = newEvent.Location,
                TicketPrice = newEvent.TicketPrice
            };

            _context.Events.Add(ev);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(getEvent), new { id = ev.Id }, ev);
        }
    }

}
