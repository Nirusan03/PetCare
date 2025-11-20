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
    public class CareLogsController : ControllerBase
    {
        private readonly AddDbContext _context;

        public CareLogsController(AddDbContext context)
        {
            _context = context;
        }

        // GET: api/carelogs/pet/5
        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<IEnumerable<CareLog>>> GetLogs(int petId)
        {
            var logs = await _context.CareLogs
                .FromSqlRaw("EXEC sp_GetCareLogsByPet @PetId", new SqlParameter("@PetId", petId))
                .ToListAsync();

            return Ok(logs);
        }

        // POST: api/carelogs
        [HttpPost]
        public async Task<IActionResult> AddLog(CreateCareLogDto dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_AddCareLog @PetId, @ActivityType, @Notes, @LogDate",
                new SqlParameter("@PetId", dto.PetId),
                new SqlParameter("@ActivityType", dto.ActivityType),
                new SqlParameter("@Notes", dto.Notes ?? (object)DBNull.Value),
                new SqlParameter("@LogDate", dto.LogDate)
            );

            return Ok(new { Message = "Care log saved." });
        }
    }
}