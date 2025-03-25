using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using backend.Models;

namespace backend.Controllers;

public class UserLogin {
  public required string Username { get; set; }
  public required string Password { get; set; }
}

[ApiController]
[Route("api/v1/[controller]")]
public class AuthController(GameContext context) : ControllerBase {
  private readonly GameContext _context = context;

  [HttpPost("login")]
  public IActionResult Login([FromBody] UserLogin loginData) {
    var salt = Environment.GetEnvironmentVariable("MD5_SALT") ?? "";
    var passwordHash = CreateMD5(loginData.Password + salt);

    var user = _context
      .Users
      .Where(u => u.Username == loginData.Username && u.PasswordHash == passwordHash)
      .FirstOrDefault();

    if (user != null) {
      var token = GenerateJwtToken(user.Id.ToString());
      
      return Ok(new { token });
    }

    return Unauthorized();
  }

  private static string GenerateJwtToken(string userId) {
    var secret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "";
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var claims = new[] {
      new Claim(JwtRegisteredClaimNames.Sub, userId),
      new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
    };

    var token = new JwtSecurityToken(
      signingCredentials: creds,
      issuer: Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "",
      audience: Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "",
      claims: claims,
      expires: DateTime.Now.AddHours(6)
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }

  public static string CreateMD5(string input) {
    byte[] inputBytes = Encoding.ASCII.GetBytes(input);
    byte[] hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);

    return Convert.ToHexString(hashBytes);
  }
}
