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
            var meds = await _context.Medication
                .FromSqlRaw("EXEC sp_GetMedicationsByPet @PetId", new SqlParameter("@PetId", petId))
                .ToListAsync();

            return Ok(meds);
        }

        // POST: api/medications
        [HttpPost]
        public async Task<IActionResult> AddMedication(CreateMedicationDto dto)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_AddMedication @PetId, @Name, @Dosage, @Frequency, @StartDate, @EndDate",
                new SqlParameter("@PetId", dto.PetId),
                new SqlParameter("@Name", dto.Name),
                new SqlParameter("@Dosage", dto.Dosage),
                new SqlParameter("@Frequency", dto.Frequency),
                new SqlParameter("@StartDate", dto.StartDate),
                new SqlParameter("@EndDate", dto.EndDate ?? (object)DBNull.Value)
            );

            return Ok(new { Message = "Medication added." });
        }
    }
}