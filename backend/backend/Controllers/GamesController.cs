using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Models;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace backend.Controllers;

[Route("/api/v1/[controller]")]
[ApiController]
public class GamesController(GameContext context) : ControllerBase {
  private readonly GameContext _context = context;

  private long? ParseUserId() {
    var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (long.TryParse(userIdClaim, out var userId)) {
      return userId;
    }

    return null;
  }

  // GET: api/Games
  [HttpGet]
  [Authorize]
  public async Task<ActionResult<IEnumerable<Game>>> GetGames() {
    var user = ParseUserId();
    if (user == null) {
      return BadRequest();
    }

    return await _context.Games
      .Where(g => g.UserId == user)
      .ToListAsync();
  }

  // GET: api/Games/5
  [HttpGet("{id}")]
  [Authorize]
  public async Task<ActionResult<Game>> GetGame(long id) {
    var user = ParseUserId();
    if (user == null) {
      return BadRequest();
    }

    var game = await _context.Games
      .Where(g => g.Id == id && g.UserId == user)
      .FirstOrDefaultAsync();

    if (game == null || game.UserId != user) {
      return NotFound();
    }

    return game;
  }

  // POST: api/Games
  // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
  [HttpPost]
  [Authorize]
  public async Task<ActionResult<Game>> PostGame() {
    var user = ParseUserId();
    if (user == null) {
      return BadRequest();
    }

    var newGame = new Game {
      UserId = (long)user,
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

    _context.Games.Add(newGame);
    await _context.SaveChangesAsync();

    return CreatedAtAction(nameof(GetGame), new { id = newGame.Id }, newGame);
  }

  // DELETE: api/Games/5
  [HttpDelete("{id}")]
  [Authorize]
  public async Task<IActionResult> DeleteGame(long id) {
    var user = ParseUserId();
    if (user == null) {
      return BadRequest();
    }

    var game = await _context.Games
      .Where(g => g.Id == id && g.UserId == user)
      .FirstOrDefaultAsync();

    if (game == null) {
      return NotFound();
    }

    _context.Games.Remove(game);
    await _context.SaveChangesAsync();

    return NoContent();
  }

  [HttpPost]
  [Route("[action]/{id}")]
  [Authorize]
  public async Task<ActionResult<SetCheckResult>> CheckSet(
    long id,
    [FromBody] ushort[] cardIndices
  ) {
    var user = ParseUserId();
    if (user == null) {
      return BadRequest();
    }

    var game = await _context.Games
      .Where(g => g.Id == id && g.UserId == user)
      .FirstOrDefaultAsync();

    if (game == null) {
      return NotFound();
    }

    var res = game.IsSet(cardIndices);

    if (res == null) {
      return BadRequest();
    }

    await _context.SaveChangesAsync();

    return res;
  }

  [HttpGet]
  [Route("[action]/{id}")]
  [Authorize]
  public async Task<ActionResult<List<int[]>>> SetsInHand(long id) {
    var user = ParseUserId();
    if (user == null) {
      return BadRequest();
    }

    var game = await _context.Games
      .Where(g => g.Id == id && g.UserId == user)
      .FirstOrDefaultAsync();

    if (game == null) {
      return NotFound();
    }

    var res = game.GetIndicesOfSet();

    if (res == null) {
      return BadRequest();
    }

    return res;
  }
}