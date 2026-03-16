using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TaskManagementAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// ----------------------
// Add Services
// ----------------------

// Controllers
builder.Services.AddControllers();

// PostgreSQL Database
builder.Services.AddDbContext<AppDbContext>(options =>
options.UseNpgsql(
builder.Configuration.GetConnectionString("DefaultConnection")
)
);

// CORS Policy (Allow Angular + Render frontend)
builder.Services.AddCors(options =>
{
options.AddPolicy("AllowAngular", policy =>
{
policy
.AllowAnyOrigin()
.AllowAnyMethod()
.AllowAnyHeader();
});
});

// JWT Authentication
builder.Services.AddAuthentication("Bearer")
.AddJwtBearer("Bearer", options =>
{
options.TokenValidationParameters = new TokenValidationParameters
{
ValidateIssuer = false,
ValidateAudience = false,
ValidateLifetime = true,
ValidateIssuerSigningKey = true,
IssuerSigningKey = new SymmetricSecurityKey(
Encoding.UTF8.GetBytes(
builder.Configuration["Jwt:Key"]!
)
)
};
});

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ----------------------
// Middleware Pipeline
// ----------------------

// Swagger (Enable in development + production for testing API)
app.UseSwagger();
app.UseSwaggerUI();

// HTTPS
app.UseHttpsRedirection();

// Enable CORS BEFORE authentication
app.UseCors("AllowAngular");

// JWT Middleware
app.UseAuthentication();
app.UseAuthorization();

// Map Controllers
app.MapControllers();

// Start Application
app.Run();
