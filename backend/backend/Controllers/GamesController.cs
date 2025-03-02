using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend.Controllers
{
  [Route("/api/v1/[controller]")]
  [ApiController]
  public class GamesController : ControllerBase {
    private readonly GameContext _context;

    public GamesController(GameContext context) {
      _context = context;
    }

    // GET: api/Games
    [HttpGet]
    public async Task<ActionResult<IEnumerable<Game>>> GetGames() {
      return await _context.Games.ToListAsync();
    }

    // GET: api/Games/5
    [HttpGet("{id}")]
    public async Task<ActionResult<Game>> GetGame(long id)
    {
      var game = await _context.Games.FindAsync(id);

      if (game == null)
      {
        return NotFound();
      }

      return game;
    }

    // PUT: api/Games/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    public async Task<IActionResult> PutGame(long id, Game game)
    {
      if (id != game.Id)
      {
        return BadRequest();
      }

      _context.Entry(game).State = EntityState.Modified;

      try {
        await _context.SaveChangesAsync();
      } catch (DbUpdateConcurrencyException) {
        if (!GameExists(id)) {
          return NotFound();
        } else {
          throw;
        }
      }

      return NoContent();
    }

    // POST: api/Games
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    public async Task<ActionResult<Game>> PostGame() {
      var newGame = new Game {
        Deck = (
          from shape in Enum.GetValues(typeof(CardShape)).Cast<CardShape>()
          from color in Enum.GetValues(typeof(CardColor)).Cast<CardColor>()
          from count in Enum.GetValues(typeof(CardCount)).Cast<CardCount>()
          from shade in Enum.GetValues(typeof(CardShade)).Cast<CardShade>()
          select new Card {
            Shape = shape,
            Color = color,
            Count = count,
            Shade = shade
          }.ToUshort()).ToArray()
      };

      newGame.ShuffleDeck();
      newGame.DealHand();
      
      _context.Games.Add(newGame);
      await _context.SaveChangesAsync();

      return CreatedAtAction(nameof(GetGame), new { id = newGame.Id }, newGame);
    }

    // DELETE: api/Games/5
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteGame(long id) {
      var game = await _context.Games.FindAsync(id);
      if (game == null)
      {
        return NotFound();
      }

      _context.Games.Remove(game);
      await _context.SaveChangesAsync();

      return NoContent();
    }

    private bool GameExists(long id) {
      return _context.Games.Any(e => e.Id == id);
    }

    [HttpPost]
    [Route("[action]/{id}")]
  public async Task<ActionResult<SetCheckResult>> CheckSet(
    long id, 
    [FromBody] ushort[] cardIndices
  ) {
    var game = await _context.Games.FindAsync(id);
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
  }
}
