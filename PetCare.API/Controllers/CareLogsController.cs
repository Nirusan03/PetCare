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
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            int userId = 1;

            if(userIdClaim != null && int.TryParse(userIdClaim.Value, out int parseId))
            {
                userId = parseId;
            }

            string safeNotes = dto.Notes ?? "";
            string safePhoto = "";

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_AddCareLog @PetId, @UserId, @Type, @Timestamp, @Details, @PhotoUrl",
                new SqlParameter("@PetId", dto.PetId),
                new SqlParameter("@UserId", userId),
                new SqlParameter("@Type", dto.ActivityType ?? "General"),
                new SqlParameter("@Timestamp", dto.LogDate),
                new SqlParameter("@Details", safeNotes),
                new SqlParameter("@PhotoUrl", safePhoto)
            );

            return Ok(new { Message = "Care log saved." });
        }
    }
}