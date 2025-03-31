using Backend.Interfaces;
using Backend.Models;

namespace Backend.Repositories;

public class UserRepository(GameContext context) : IUserRepository {
  private readonly GameContext _context = context;

  public User? GetUserByUsername(string username) {
    return _context.Users.FirstOrDefault(u => u.Username == username);
  }

  public User? ValidateUser(string username, string passwordHash) {
    return _context.Users
      .FirstOrDefault(u => u.Username == username && u.PasswordHash == passwordHash);
  }

  public async Task<User> CreateUserAsync(User user) {
    _context.Users.Add(user);
    await _context.SaveChangesAsync();
    return user;
  }
}
