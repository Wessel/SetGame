using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Backend.Models;
using Backend.Interfaces;

namespace backend.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class GamesController(IGameService gameService) : ControllerBase {
  private readonly IGameService _gameService = gameService;

  private long? ParseUserId() {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    return long.TryParse(userIdClaim, out var userId) ? userId : null;
  }

  // GET: api/v1/Games
  [HttpGet]
  [Authorize]
  public async Task<ActionResult<IEnumerable<Game>>> GetGames() {
    var userId = ParseUserId();
    if (userId == null) {
      return BadRequest();
    }

    var games = await _gameService.GetPlayableGamesAsync(userId.Value);
    return Ok(games);
  }

  // GET: api/v1/Games/5
  [HttpGet("{id}")]
  [Authorize]
  public async Task<ActionResult<Game>> GetGame(long id) {
    var userId = ParseUserId();
    if (userId == null) {
      return BadRequest("Invalid user ID");
    }

    var game = await _gameService.GetGameAsync(id, userId.Value);
    
    if (game == null) {
      return NotFound();
    }
        
    return game;
  }

  // POST: api/v1/Games
  [HttpPost]
  [Authorize]
  public async Task<ActionResult<Game>> PostGame() {
    var userId = ParseUserId();
    if (userId == null) {
      return BadRequest();
    }

    var game = await _gameService.CreateGameAsync(userId.Value);
    return CreatedAtAction(nameof(GetGame), new { id = game.Id }, game);
  }

  // DELETE: api/v1/Games/5
  [HttpDelete("{id}")]
  [Authorize]
  public async Task<IActionResult> DeleteGame(long id) {
      var userId = ParseUserId();
      if (userId == null) {
        return BadRequest();
      }

      var result = await _gameService.DeleteGameAsync(id, userId.Value);
      
      if (!result) {
        return NotFound();
      }

      return NoContent();
  }

  // POST: api/v1/Games/CheckSet/5
  [HttpPost]
  [Route("[action]/{id}")]
  [Authorize]
  public async Task<ActionResult<SetCheckResult>> CheckSet(
    long id,
    [FromBody] ushort[] cardIndices
  ) {
    var userId = ParseUserId();
    if (userId == null) {
      return BadRequest();
    }

    var result = await _gameService.CheckSetAsync(id, userId.Value, cardIndices);
    
    if (result == null) {
      return BadRequest();
    }

    return result;
  }

  // GET: api/v1/Games/SetsInHand/5
  [HttpGet]
  [Route("[action]/{id}")]
  [Authorize]
  public async Task<ActionResult<List<int[]>>> SetsInHand(long id) {
    var userId = ParseUserId();
    if (userId == null) {
      return BadRequest();
    }

    var result = await _gameService.GetSetsInHandAsync(id, userId.Value);
    
    if (result == null) {
      return NotFound();
    }

    return result;
  }

  // GET: api/v1/Games/Hint/5
  [HttpGet]
  [Route("[action]/{id}")]
  [Authorize]
  public async Task<ActionResult<List<ushort>>> Hint(long id) {
    var userId = ParseUserId();
    if (userId == null) {
        return BadRequest();
    }

    var result = await _gameService.GetHintAsync(id, userId.Value);
    
    if (result == null) {
      return BadRequest();
    }

    return result;
  }
}