using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;
using PetCare.Shared;
using PetCare.Shared.DTOs;

namespace PetCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class RemindersController : ControllerBase
    {
        private readonly AddDbContext _context;

        public RemindersController(AddDbContext context)
        {
            _context = context;
        }

        // GET: api/reminders/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Reminder>>> GetReminders(int userId)
        {
            var reminders = await _context.Reminders
                .FromSqlRaw("EXEC sp_GetRemindersByUser @UserId", new SqlParameter("@UserId", userId))
                .ToListAsync();

            return Ok(reminders);
        }

        // POST: api/reminders
        [HttpPost]
        public async Task<IActionResult> AddReminder(CreateReminderDto dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_AddReminder @UserId, @PetId, @Title, @ReminderDate, @IsRecurring",
                new SqlParameter("@UserId", dto.UserId),
                new SqlParameter("@PetId", dto.PetId),
                new SqlParameter("@Title", dto.Title),
                new SqlParameter("@ReminderDate", dto.ReminderDate),
                new SqlParameter("@IsRecurring", dto.IsRecurring)
            );

            return Ok(new { Message = "Reminder set." });
        }

        // POST: api/reminders/mark-sent/10
        [HttpPost("mark-sent/{reminderId}")]
        public async Task<IActionResult> MarkAsSent(int reminderId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_MarkReminderSent @ReminderId",
                new SqlParameter("@ReminderId", reminderId)
            );

            return Ok(new { Message = "Reminder marked as sent." });
        }
    }
}