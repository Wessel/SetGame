using Backend.Models;

namespace Backend.Interfaces;

public class UserLogin {
  public required string Username { get; set; }
  public required string Password { get; set; }
}

public interface IAuthService {
  User? ValidateUser(UserLogin credentials);
  string GenerateJwtToken(string userId);
}