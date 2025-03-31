using Backend.Models;

namespace Backend.Interfaces;

public interface IGameRepository {
  Task<IEnumerable<Game>> GetGamesByUserIdAsync(long userId);
  Task<Game?> GetGameByIdAsync(long id, long userId);
  Task<Game> CreateGameAsync(Game game);
  Task<Game> UpdateGameAsync(Game game);
  Task DeleteGameAsync(Game game);
}