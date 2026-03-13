using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.Data;
using TaskManagementAPI.Models;

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
        public IActionResult Register(User user)
        {

            var existingUser = _context.Users
                .FirstOrDefault(x => x.Email == user.Email);

            if (existingUser != null)
            {
                return BadRequest("Email already exists");
            }

            _context.Users.Add(user);
            _context.SaveChanges();

            return Ok(user);
        }

        // LOGIN USER
        [HttpPost("login")]
        public IActionResult Login(User login)
        {

            var user = _context.Users
                .FirstOrDefault(x =>
                    x.Email == login.Email &&
                    x.Password == login.Password);

            if (user == null)
            {
                return Unauthorized("Invalid email or password");
            }

            return Ok(new
            {
                token = "fake-jwt-token", // required for Angular
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
                    x.Email
                })
                .ToList();

            return Ok(employees);

        }

    }
}