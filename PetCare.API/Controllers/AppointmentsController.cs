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
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_AddAppointment @PetId, @AppointmentTime, @Title, @Description, @Location",
                new SqlParameter("@PetId", dto.PetId),
                new SqlParameter("@AppointmentTime", dto.AppointmentTime),
                new SqlParameter("@Title", dto.Title),
                new SqlParameter("@Description", dto.Description ?? (object)DBNull.Value),
                new SqlParameter("@Location", dto.Location ?? (object)DBNull.Value)
            );

            return Ok(new { Message = "Appointment scheduled successfully." });
        }

        // POST: api/appointments/status
        [HttpGet("status")]
        public async Task<IActionResult> UpdateStatus(UpdateAppointmentStatusDto dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_UpdateAppointmentStatus @AppointmentId, @IsCompleted",
                new SqlParameter("@AppointmentId", dto.AppointmentId),
                new SqlParameter("@IsCompleted", dto.IsCompleted)
            );

            return Ok(new { Message = "Status updated." });
        }
    }
}
