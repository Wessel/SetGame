using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using backend.Models;

        public class UserLogin
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly GameContext _context;

public static string CreateMD5(string input)
{
    // Use input string to calculate MD5 hash
    using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
    {
        byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
        byte[] hashBytes = md5.ComputeHash(inputBytes);

        return Convert.ToHexString(hashBytes); // .NET 5 +

        // Convert the byte array to hexadecimal string prior to .NET 5
        // StringBuilder sb = new System.Text.StringBuilder();
        // for (int i = 0; i < hashBytes.Length; i++)
        // {
        //     sb.Append(hashBytes[i].ToString("X2"));
        // }
        // return sb.ToString();
    }
}

    public AuthController(GameContext context) {
      _context = context;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] UserLogin user) {
      var salt = Environment.GetEnvironmentVariable("MD5_SALT") ?? "";
      var passwordHash = CreateMD5(user.Password + salt);
      var dbUser = _context.Users.Where(u => u.Username == user.Username && u.PasswordHash == passwordHash).FirstOrDefault();
        if (dbUser != null) {
            var token = GenerateJwtToken(dbUser.Id.ToString());
            return Ok(new { token });
        }

        return Unauthorized();
    }

    private string GenerateJwtToken(string userId)
    {
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SECRET")));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: "wessel.gg",
            audience: "wessel.gg",
            claims: claims,
            expires: DateTime.Now.AddMinutes(30),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
