using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PetCare.API.Data;
using PetCare.Shared;
using PetCare.Shared.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
namespace PetCare.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly AddDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(AddDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email))
            {
                return BadRequest("Email already in use.");
            }
            var user = new User
            {
                Username = request.UserName,
                Email = request.Email,
                PasswordHash = request.Password,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return Ok(new { Message = $"{request.UserName} is regsistered to the system." });
        }

        [HttpPost("login")]
        public async Task<ActionResult<LoginResponse>> Login(LoginRequest request)
        {
            // FIX: Compare PasswordHash, not Email again
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Email == request.Email && u.PasswordHash == request.Password);

            if (user == null) return Unauthorized("Invalid credentials");

            // Generate JWT Token
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_configuration["Jwt:Key"]!);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                    new Claim(ClaimTypes.Email, user.Email)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new LoginResponse
            {
                Token = tokenHandler.WriteToken(token),
                UserId = user.UserId,
                UserName = user.Username
            });
        }
    }
}
