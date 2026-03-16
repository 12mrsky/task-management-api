using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;
using TaskManagementAPI.DTOs;
using BCrypt.Net;

namespace TaskManagementAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // REGISTER USER
        [HttpPost("register")]
        public IActionResult Register(RegisterDTO dto)
        {
            var existingUser = _context.Users
                .FirstOrDefault(x => x.Email == dto.Email);

            if (existingUser != null)
            {
                return BadRequest("Email already exists");
            }

            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),

                // Accept role from request
                Role = string.IsNullOrEmpty(dto.Role) ? "Employee" : dto.Role
            };

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(new
            {
                message = "User registered successfully",
                user.Role
            });
        }

        // LOGIN USER
        [HttpPost("login")]
        public IActionResult Login(LoginDTO dto)
        {

            var user = _context.Users
                .FirstOrDefault(x => x.Email == dto.Email);

            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            bool passwordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.Password);

            if (!passwordValid)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(new
            {
                token = "fake-jwt-token",
                userId = user.UserId,
                name = user.Name,
                email = user.Email,
                role = user.Role
            });
        }

        // GET EMPLOYEES
        [HttpGet("employees")]
        public IActionResult GetEmployees()
        {

            var employees = _context.Users
                .Where(x => x.Role == "Employee")
                .Select(x => new
                {
                    x.UserId,
                    x.Name,
                    x.Email,
                    x.Role
                })
                .ToList();

            return Ok(employees);
        }

        // GET ALL USERS
        [HttpGet("users")]
        public IActionResult GetUsers()
        {

            var users = _context.Users
                .Select(u => new
                {
                    u.UserId,
                    u.Name,
                    u.Email,
                    u.Role
                })
                .ToList();

            return Ok(users);
        }

        // RESET PASSWORD
        [HttpPut("reset-password/{id}")]
        public IActionResult ResetPassword(int id, [FromBody] ResetPasswordDto dto)
        {

            var user = _context.Users.FirstOrDefault(u => u.UserId == id);

            if (user == null)
                return NotFound("User not found");

            if (string.IsNullOrEmpty(dto.NewPassword))
                return BadRequest("Password cannot be empty");

            user.Password = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);

            _context.SaveChanges();

            return Ok("Password reset successfully");
        }

        // DELETE USER
        [HttpDelete("users/{id}")]
        public IActionResult DeleteUser(int id)
        {

            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound("User not found");

            _context.Users.Remove(user);
            _context.SaveChanges();

            return Ok("User deleted successfully");
        }

    }
}