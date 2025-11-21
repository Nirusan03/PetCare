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
    public class MedicationsController : ControllerBase
    {
        private readonly AddDbContext _context;

        public MedicationsController(AddDbContext context)
        {
            _context = context;
        }

        // GET: api/medications/pet/5
        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<IEnumerable<Medication>>> GetMeds(int petId)
        {
            // FIX 1: Corrected SP Name (Singular 'Medication')
            var meds = await _context.Medication
                .FromSqlRaw("EXEC sp_GetMedicationByPet @PetId", new SqlParameter("@PetId", petId))
                .ToListAsync();

            return Ok(meds);
        }

        // POST: api/medications
        [HttpPost]
        public async Task<IActionResult> AddMedication(CreateMedicationDto dto)
        {
            // FIX 2: Added @Notes (Passed as empty string since DTO doesn't have it)
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_AddMedication @PetId, @Name, @Dosage, @Frequency, @StartDate, @EndDate, @Notes",
                new SqlParameter("@PetId", dto.PetId),
                new SqlParameter("@Name", dto.Name),
                new SqlParameter("@Dosage", dto.Dosage),
                new SqlParameter("@Frequency", dto.Frequency),
                new SqlParameter("@StartDate", dto.StartDate),
                new SqlParameter("@EndDate", dto.EndDate ?? (object)DBNull.Value),
                new SqlParameter("@Notes", "") // <--- Added this required param
            );

            return Ok(new { Message = "Medication added." });
        }

        // FIX 3: Added Stop Endpoint (Required for the UI Stop button)
        // PUT: api/medications/stop/5
        [HttpPut("stop/{id}")]
        public async Task<IActionResult> StopMedication(int id)
        {
            // Ensures sp_StopMedication exists (we created it in the previous step)
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_StopMedication @MedicationId",
                new SqlParameter("@MedicationId", id)
            );

            return Ok(new { Message = "Medication stopped." });
        }
    }
}