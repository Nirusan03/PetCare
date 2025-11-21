using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using PetCare.API.Data;
using PetCare.Shared;
using PetCare.Shared.DTOs;
using System.Security.Claims;

namespace PetCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentsController : Controller
    {
        private readonly AddDbContext _context;

        public AppointmentsController(AddDbContext context)
        {
            _context = context;
        }

        // GET: api/appointments/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByUser(int userId)
        {
            var appointments = await _context.Appointments
                .FromSqlRaw("EXEC sp_GetAppointmentsByUser @UserId", new SqlParameter("@UserId", userId))
                .ToListAsync();
            return Ok(appointments);
        }

        // POST: api/appointments
        [HttpPost]
        public async Task<IActionResult> AddAppointment(CreatAppointmentDto dto)
        {
            // 1. GET THE REAL USER ID FROM THE TOKEN
            // The [Authorize] attribute ensures the token is valid.
            // We look for the "NameIdentifier" claim where the ID is stored.
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = 1;

            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int parseId))
            {
                userId = parseId;
            }

            // 2. Default EndTime logic
            DateTime endTime = dto.AppointmentTime.AddHours(1);

            // 3. MAP DTO TO SQL PARAMETERS (The Critical Step)
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_AddAppointment @PetId, @UserId, @Provider, @Type, @StartDateTime, @EndDateTime, @Notes, @Status",

                new SqlParameter("@PetId", dto.PetId),
                new SqlParameter("@UserId", userId),

                // --- MAPPING LOGIC ---
                new SqlParameter("@Provider", dto.Location ?? ""), // Maps Location -> Provider
                new SqlParameter("@Type", dto.Title ?? ""),        // Maps Title -> Type
                new SqlParameter("@StartDateTime", dto.AppointmentTime),
                new SqlParameter("@EndDateTime", endTime),
                new SqlParameter("@Notes", dto.Description ?? ""), // Maps Description -> Notes
                new SqlParameter("@Status", "Scheduled")           // Default Status
            );

            return Ok(new { Message = "Appointment scheduled successfully." });
        }

        // POST: api/appointments/status
        [HttpPut("status")]
        public async Task<IActionResult> UpdateStatus(UpdateAppointmentStatusDto dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateAppointmentStatus @AppointmentId, @IsCompleted",
                new SqlParameter("@AppointmentId", dto.AppointmentId),
                new SqlParameter("@IsCompleted", dto.IsCompleted)
            );

            return Ok(new { Message = "Status updated." });
        }

        // DELETE: api/appointments/5
        [HttpDelete("{appointmentId}")]
        public async Task<IActionResult> DeleteAppointment (int appointmentId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "DELETE FROM Appointment WHERE AppointmentId = @Id",
                new SqlParameter("@Id", appointmentId)
            );
            return Ok(new { Message = "Appointment deleted." });
        }
    }
}
