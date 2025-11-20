using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetCare.API.Data;
using PetCare.Shared;
using PetCare.Shared.DTOs;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PetCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Locks down entire controller
    public class PetsController : ControllerBase
    {
        private readonly AddDbContext _context;

        public PetsController(AddDbContext context)
        {
            _context = context;
        }

        // GET: api/pets/user/5
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<Pet>>> GetPetsByUser(int userId)
        {
            // HYBRID STRATEGY: Using EF Core to map the SP result directly to the Pet Model
            var pets = await _context.Pets
                .FromSqlRaw("EXEC sp_GetPetsByUser @UserId", new SqlParameter("@UserId", userId))
                .ToListAsync();

            return Ok(pets);
        }

        // GET: api/pets/5
        [HttpGet("{petId}")]
        public async Task<ActionResult<Pet>> GetPetDetails(int petId)
        {
            // 1. Execute the Stored Procedure asynchronously and materialize the list
            var result = await _context.Pets
                .FromSqlRaw("EXEC sp_GetPetDetails @PetId", new SqlParameter("@PetId", petId))
                .ToListAsync(); // This handles the async database call correctly

            // 2. Get the first item from the list in memory
            var pet = result.FirstOrDefault();

            if (pet == null) return NotFound();

            return Ok(pet);
        }

        // POST: api/pets
        [HttpPost]
        public async Task<IActionResult> AddPet(CreatePetDto petDto)
        {
            var parameters = new[]
            {
                new SqlParameter("@UserId", petDto.UserId),
                new SqlParameter("@Name", petDto.Name),
                new SqlParameter("@Species", petDto.Species),
                new SqlParameter("@Breed", petDto.Breed ?? (object)DBNull.Value),
                new SqlParameter("@DOB", petDto.DOB),
                new SqlParameter("@Sex", petDto.Sex ?? (object)DBNull.Value),
                new SqlParameter("@PhotoUrl", petDto.PhotoUrl ?? (object)DBNull.Value),
                new SqlParameter("@Notes", petDto.Notes ?? (object)DBNull.Value)
            };

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_AddPet @UserId, @Name, @Species, @Breed, @DOB, @Sex, @PhotoUrl, @Notes",
                parameters);

            return Ok(new { Message = "Pet added successfully" });
        }

        // DELETE: api/pets/5
        [HttpDelete("{petId}")]
        public async Task<IActionResult> DeletePet(int petId)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC sp_DeletePet @PetId",
                new SqlParameter("@PetId", petId));

            return Ok(new { Message = "Pet deleted (soft delete)" });
        }
    }
}
