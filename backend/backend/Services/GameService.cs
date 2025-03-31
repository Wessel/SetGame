using Backend.Models;
using Backend.Interfaces;

namespace Backend.Services;
public class GameService(IGameRepository gameRepository) : IGameService {
  private readonly IGameRepository _gameRepository = gameRepository;

  public async Task<IEnumerable<Game>> GetPlayableGamesAsync(long userId) {
    return await _gameRepository.GetGamesByUserIdAsync(userId);
  }

  public async Task<Game?> GetGameAsync(long gameId, long userId) {
    return await _gameRepository.GetGameByIdAsync(gameId, userId);
  }

  public async Task<Game> CreateGameAsync(long userId) {
    var newGame = new Game {
      UserId = userId,
      StartedAt = DateTime.UtcNow,
      Deck = [.. 
        from shape in Enum.GetValues<CardShape>().Cast<CardShape>()
        from color in Enum.GetValues<CardColor>().Cast<CardColor>()
        from count in Enum.GetValues<CardCount>().Cast<CardCount>()
        from shade in Enum.GetValues<CardShade>().Cast<CardShade>()
        select new Card {
          Shape = shape,
          Color = color,
          Count = count,
          Shade = shade
        }.ToUshort()
      ]
    };

    newGame.ShuffleDeck();
    newGame.DealHand();

    return await _gameRepository.CreateGameAsync(newGame);
  }

  public async Task<bool> DeleteGameAsync(long gameId, long userId) {
    var game = await _gameRepository.GetGameByIdAsync(gameId, userId);
    
    if (game == null) {
      return false;
    }
        
    await _gameRepository.DeleteGameAsync(game);
    return true;
  }

  public async Task<SetCheckResult?> CheckSetAsync(long gameId, long userId, ushort[] cardIndices) {
    var game = await _gameRepository.GetGameByIdAsync(gameId, userId);
    
    if (game == null) {
      return null;
    }
        
    var result = game.IsSet(cardIndices);
    
    if (result != null) {
      await _gameRepository.UpdateGameAsync(game);
    }
        
    return result;
  }

  public async Task<List<int[]>> GetSetsInHandAsync(long gameId, long userId) {
    var game = await _gameRepository.GetGameByIdAsync(gameId, userId);
      
    if (game == null) {
      return [];
    }
          
    return game.GetIndicesOfSet();
  }

  public async Task<List<ushort>> GetHintAsync(long gameId, long userId) {
    var game = await _gameRepository.GetGameByIdAsync(gameId, userId);
      
    if (game == null) {
      return [];
    }

    var sets = game.GetIndicesOfSet();
    
    if (sets == null || sets.Count == 0) {
      return [];
    }
        
    game.Hints++;
    await _gameRepository.UpdateGameAsync(game);

    return [.. sets[0].Take(2).Select(i => (ushort)i)];
  }
}