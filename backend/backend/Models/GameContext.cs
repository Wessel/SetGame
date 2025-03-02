using Microsoft.EntityFrameworkCore;

namespace backend.Models;

public class GameContext : DbContext
{
  public GameContext(DbContextOptions<GameContext> options) : base(options) {
  }

  public DbSet<Game> Games { get; set; } = null!;

  protected override void OnModelCreating(ModelBuilder modelBuilder) {
    modelBuilder.Entity<Game>()
      .Property(b => b.StartedAt)
      .HasDefaultValueSql("CURRENT_TIMESTAMP");
  }
}