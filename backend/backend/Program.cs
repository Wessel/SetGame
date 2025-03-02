using Microsoft.EntityFrameworkCore;
using backend.Models;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration
  .AddEnvironmentVariables()
  .Build();

builder.Services.AddOpenApi();
builder.Services.AddControllers();

var postgres_connection_string = $@"User ID={config["DB_USER"]};
                                    Password={config["DB_PASS"]};
                                    Host={config["DB_HOST"]};
                                    Port={config["DB_PORT"]};
                                    Database={config["DB_NAME"]};
                                    Connection Lifetime=0;";

builder.Services.AddDbContext<GameContext>(opt => opt.UseNpgsql(postgres_connection_string));

var app = builder.Build();

// Disable CORS
app.Logger.LogInformation("Disabling CORS to allow communication with frontend");
app.UseCors(builder =>  {
  builder.AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader();
});

if (app.Environment.IsDevelopment()) {
  app.Logger.LogInformation("Enabling Swagger UI for development");
  app.MapOpenApi();
  app.UseSwaggerUi(options => options.DocumentPath = "/openapi/v1.json");
}

app.Logger.LogInformation("Creating middleware");
// app.UseHttpsRedirection();
// app.UseAuthorization();
app.MapControllers();

app.Run();