using Backend.Models;

namespace Backend.Interfaces;
public interface IGameService {
  Task<IEnumerable<Game>> GetPlayableGamesAsync(long userId);
  Task<Game?> GetGameAsync(long gameId, long userId);
  Task<Game> CreateGameAsync(long userId);
  Task<bool> DeleteGameAsync(long gameId, long userId);
  Task<SetCheckResult?> CheckSetAsync(long gameId, long userId, ushort[] cardIndices);
  Task<List<int[]>> GetSetsInHandAsync(long gameId, long userId);
  Task<List<ushort>> GetHintAsync(long gameId, long userId);
}