using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RaceDayAPI.Data;
using RaceDayAPI.DTOs;
using RaceDayAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace RaceDayAPI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly RaceDayContext _context;
        private readonly IConfiguration _configuration;
        private readonly PasswordHasher<User> _hasher = new();

        public AuthController(
            RaceDayContext context,
            IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public IActionResult Register(
            RegisterRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Complete all required fields.");
            }

            if (request.Password.Length < 8)
            {
                return BadRequest(
                    "Password must contain at least 8 characters.");
            }

            string role = request.Role.Trim();

            if (role != "Organiser" && role != "Participant")
            {
                return BadRequest(
                    "Registration role must be Organiser or Participant.");
            }

            string email = request.Email.Trim().ToLowerInvariant();

            bool emailExists = _context.Users
                .Any(u => u.Email == email);

            if (emailExists)
            {
                return Conflict("Email is already registered.");
            }

            var user = new User
            {
                FirstName = request.FirstName.Trim(),
                LastName = request.LastName.Trim(),
                Email = email,
                Role = role,
                CreatedAt = DateTime.Now
            };

            user.PasswordHash = _hasher.HashPassword(
                user, request.Password);

            _context.Users.Add(user);

            _context.SaveChanges();

            if (role == "Organiser")
            {
                if (string.IsNullOrWhiteSpace(
                    request.OrganisationName))
                {
                    _context.Users.Remove(user);
                    _context.SaveChanges();

                    return BadRequest(
                        "Organisation name is required.");
                }

                _context.Organisers.Add(new Organiser
                {
                    UserID = user.UserID,
                    OrganisationName = request.OrganisationName.Trim()
                });
            }
            else
            {
                _context.Participants.Add(new Participant
                {
                    UserID = user.UserID,
                    Phone = request.Phone
                });
            }

            _context.SaveChanges();

            return StatusCode(201, new
            {
                user.UserID,
                user.FirstName,
                user.LastName,
                user.Email,
                user.Role
            });
        }

        [HttpPost("login")]
        public IActionResult Login(
            LoginRequest request)
        {
            string email = request.Email.Trim().ToLowerInvariant();

            var user = _context.Users
                .FirstOrDefault(u => u.Email == email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            var result = _hasher.VerifyHashedPassword(
                user, user.PasswordHash, request.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password.");
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier,
                    user.UserID.ToString()),
                new Claim(ClaimTypes.Name,
                    user.Email),
                new Claim(ClaimTypes.Role,
                    user.Role)
            };

            string key = _configuration["Jwt:Key"]!;

            var signingKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(key));

            var credentials = new SigningCredentials(
                signingKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: credentials);

            string tokenString =
                new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                message = "Login successful",
                token = tokenString,
                user.UserID,
                user.Email,
                user.Role
            });
        }
    }
}