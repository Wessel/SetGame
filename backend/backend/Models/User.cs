using Microsoft.EntityFrameworkCore;

namespace Backend.Models;

[Index(nameof(Username), IsUnique = true)]
public class User {
  public required long Id { get; set; }
  public required string Email { get; set; }
  public required string Username { get; set; }
  public required string PasswordHash { get; set; }
  public DateTime CreatedAt { get; set; }
  public ICollection<Game>? Games { get; set; }

}