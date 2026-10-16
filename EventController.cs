using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RaceDayAPI.Data;
using RaceDayAPI.Models;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly RaceDayContext _context;

        public EventsController(RaceDayContext context)
        {
            _context = context;
        }

        // GET: api/events
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents()
        {
            var events = await _context.Events.ToListAsync();
            return Ok(events);
        }

        // GET: api/events/1
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvent(int id)
        {
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound("Event was not found.");
            }

            return Ok(eventItem);
        }

        // POST: api/events
        [HttpPost]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> CreateEvent(Event eventItem)
        {
            if (eventItem.EventDate == default ||
                string.IsNullOrWhiteSpace(eventItem.EventName) ||
                string.IsNullOrWhiteSpace(eventItem.Location))
            {
                return BadRequest(
                    "Event name, date and location are required.");
            }

            if (eventItem.Status != "Upcoming" &&
                eventItem.Status != "Open" &&
                eventItem.Status != "Completed" &&
                eventItem.Status != "Cancelled")
            {
                return BadRequest("Invalid event status.");
            }

            int userId = int.Parse(
                User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier)!
                    .Value);

            bool isAdmin = User.IsInRole("Admin");

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (!isAdmin && organiser == null)
            {
                return Forbid();
            }

            if (isAdmin && organiser == null)
            {
                return BadRequest(
                    "Create an organiser profile before creating an event.");
            }

            eventItem.OrganiserID = organiser!.OrganiserID;

            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEvent),
                new { id = eventItem.EventID },
                eventItem);
        }

        // PUT: api/events/1
        [HttpPut("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> UpdateEvent(
            int id, Event updatedEvent)
        {
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound("Event was not found.");
            }

            int userId = int.Parse(
                User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier)!
                    .Value);

            bool isAdmin = User.IsInRole("Admin");

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (!isAdmin &&
                (organiser == null ||
                 organiser.OrganiserID != eventItem.OrganiserID))
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(updatedEvent.EventName) ||
                string.IsNullOrWhiteSpace(updatedEvent.Location))
            {
                return BadRequest(
                    "Event name and location are required.");
            }

            eventItem.EventName = updatedEvent.EventName;
            eventItem.EventDate = updatedEvent.EventDate;
            eventItem.Location = updatedEvent.Location;
            eventItem.Description = updatedEvent.Description;
            eventItem.Status = updatedEvent.Status;

            await _context.SaveChangesAsync();

            return Ok(eventItem);
        }

        // DELETE: api/events/1
        [HttpDelete("{id}")]
        [Authorize(Roles = "Organiser,Admin")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.EventID == id);

            if (eventItem == null)
            {
                return NotFound("Event was not found.");
            }

            int userId = int.Parse(
                User.FindFirst(
                    System.Security.Claims.ClaimTypes.NameIdentifier)!
                    .Value);

            bool isAdmin = User.IsInRole("Admin");

            var organiser = await _context.Organisers
                .FirstOrDefaultAsync(o => o.UserID == userId);

            if (!isAdmin &&
                (organiser == null ||
                 organiser.OrganiserID != eventItem.OrganiserID))
            {
                return Forbid();
            }

            _context.Events.Remove(eventItem);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
