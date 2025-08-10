using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KofCWSCWebsite.Controllers
{
    [ApiController]
    [Route("")]
    public class CalendarEventsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CalendarEventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Events")]
        public async Task<IActionResult> GetEvents()
        {
            //var events = await _context.CalendarEvents.ToListAsync();
            var events = await _context.tblCAL_trxEvents
        .Select(e => new
        {
            id = e.Id,
            title = e.Title,
            start = e.AllDay
                ? e.StartDateTime.Date.ToString("yyyy-MM-dd")
                : e.StartDateTime.ToString("s"),
            end = e.AllDay && e.EndDateTime.HasValue
                ? e.EndDateTime.Value.Date.ToString("yyyy-MM-dd")
                : e.EndDateTime.HasValue
                    ? e.EndDateTime.Value.ToString("s")
                    : null,
            allDay = e.AllDay,
            description = e.Description,
            className = e.AllDay ? "fc-allday-event" : "fc-timed-event"
        })
        .ToListAsync();
            return Ok(events);
        }

        [HttpPost("Event")]
        public async Task<IActionResult> CreateEvent([FromBody] CalendarEvent ev)
        {
            _context.tblCAL_trxEvents.Add(ev);
            await _context.SaveChangesAsync();
            return Ok(ev);
        }

        [HttpPut("Event/{id}")]
        public async Task<IActionResult> UpdateEvent(int id, [FromBody] CalendarEvent ev)
        {
            var existing = await _context.tblCAL_trxEvents.FindAsync(id);
            if (existing == null) return NotFound();

            existing.Title = ev.Title;
            existing.StartDateTime = ev.StartDateTime;
            existing.EndDateTime = ev.EndDateTime;
            existing.Description = ev.Description;
            existing.AllDay = ev.AllDay;

            await _context.SaveChangesAsync();
            return Ok(existing);
        }

        [HttpDelete("Event/{id}")]
        public async Task<IActionResult> DeleteEvent(int id)
        {
            var existing = await _context.tblCAL_trxEvents.FindAsync(id);
            if (existing == null) return NotFound();

            _context.tblCAL_trxEvents.Remove(existing);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
