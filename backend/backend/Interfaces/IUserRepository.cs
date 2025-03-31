using Backend.Models;

namespace Backend.Interfaces;

public interface IUserRepository {
  User? GetUserByUsername(string username);
  User? ValidateUser(string username, string passwordHash);
  Task<User> CreateUserAsync(User user);
}
