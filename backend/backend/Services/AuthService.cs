using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Backend.Interfaces;
using Backend.Models;
using Microsoft.IdentityModel.Tokens;

namespace Backend.Services;

public class AuthService(IUserRepository userRepository) : IAuthService {
    private readonly IUserRepository _userRepository = userRepository;

  public User? ValidateUser(UserLogin credentials) {
    var salt = Environment.GetEnvironmentVariable("MD5_SALT") ?? "";
    var passwordHash = CreateMD5(credentials.Password + salt);
    
    return _userRepository.ValidateUser(credentials.Username, passwordHash);
  }

  public string GenerateJwtToken(string userId) {
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

  private static string CreateMD5(string input) {
    byte[] inputBytes = Encoding.ASCII.GetBytes(input);
    byte[] hashBytes = System.Security.Cryptography.MD5.HashData(inputBytes);
    return Convert.ToHexString(hashBytes);
  }
}
