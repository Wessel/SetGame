using Microsoft.EntityFrameworkCore;

namespace Backend.Models;

public class GameContext : DbContext
{
  public GameContext(DbContextOptions<GameContext> options) : base(options) {
  }

  public DbSet<Game> Games { get; set; } = null!;
  public DbSet<User> Users { get; set; }

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity<Game>()
      .Property(b => b.StartedAt)
      .HasDefaultValueSql("CURRENT_TIMESTAMP");
  
    modelBuilder.Entity<User>()
      .Property(b => b.CreatedAt)
      .HasDefaultValueSql("CURRENT_TIMESTAMP");
  }
}