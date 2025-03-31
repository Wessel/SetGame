using Microsoft.EntityFrameworkCore;
using Backend.Models;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Backend.Interfaces;
using Backend.Services;
using Backend.Repositories;

// Load .env file
Env.Load();

// Set up config and builder, load env into it
var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration
  .AddEnvironmentVariables()
  .Build();

builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
  .AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters {
      ValidateIssuer = string.IsNullOrEmpty(config["JWT_ISSUER"]) ? false : true,
      ValidateAudience = string.IsNullOrEmpty(config["JWT_AUDIENCE"]) ? false : true,
      ValidateLifetime = true,
      ValidateIssuerSigningKey = true,
      ValidIssuer = config["JWT_ISSUER"],
      ValidAudience = config["JWT_AUDIENCE"],
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT_SECRET"] ?? ""))
    };
  });
builder.Services.AddAuthorization();

// Set up database connection
var postgres_connection_string =
  $@"User ID={config["DB_USER"]};
     Password={config["DB_PASS"]};
     Host={config["DB_HOST"]};
     Port={config["DB_PORT"]};
     Database={config["DB_NAME"]};
     Connection Lifetime=0;
  ";

builder.Services.AddDbContext<GameContext>(opt => opt.UseNpgsql(postgres_connection_string));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IGameService, GameService>();

var app = builder.Build();

// Disable CORS
app.UseCors(builder =>  {
  builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});

// Enable Swagger UI
if (app.Environment.IsDevelopment()) {
  app.MapOpenApi();
  app.UseSwaggerUi(options => options.DocumentPath = "/openapi/v1.json");
}

// Enable Middleware
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Start the server
app.Run();
