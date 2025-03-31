using Backend.Models;
using Backend.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Backend.Repositories;

public class GameRepository(GameContext context) : IGameRepository {
  private readonly GameContext _context = context;

  public async Task<IEnumerable<Game>> GetGamesByUserIdAsync(long userId) {
    return await _context.Games
      .Where(g => g.UserId == userId)
      .ToListAsync();
  }

  public async Task<Game?> GetGameByIdAsync(long id, long userId) {
    return await _context.Games
      .Where(g => g.Id == id && g.UserId == userId)
      .FirstOrDefaultAsync();
  }

  public async Task<Game> CreateGameAsync(Game game) {
    _context.Games.Add(game);
    await _context.SaveChangesAsync();
    return game;
  }

  public async Task<Game> UpdateGameAsync(Game game) {
    _context.Entry(game).State = EntityState.Modified;
    await _context.SaveChangesAsync();
    return game;
  }

  public async Task DeleteGameAsync(long id) {
    var game = await _context.Games.FindAsync(id);
    if (game != null) {
      _context.Games.Remove(game);
      await _context.SaveChangesAsync();
    }
  }

  public async Task DeleteGameAsync(Game game) {
    _context.Games.Remove(game);
    await _context.SaveChangesAsync();
  }
}