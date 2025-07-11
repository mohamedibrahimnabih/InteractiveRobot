using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InteractiveRobot.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]   
    public class GameSuggestionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GameSuggestionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddGame([FromBody] GameRequest request)
        {
            var game = new SuggestedGame
            {
                Title = request.Title,
                Condition = request.Condition,
                Description = request.Description
            };

            _context.SuggestedGames.Add(game);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetGameById), new { id = game.Id }, game);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllGames([FromQuery] string? condition = null)
        {
            var games = _context.SuggestedGames.AsQueryable();

            if (!string.IsNullOrEmpty(condition))
                games = games.Where(g => g.Condition.ToLower().Contains(condition.ToLower()));

            return Ok(await games.ToListAsync());
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetGameById(int id)
        {
            var game = await _context.SuggestedGames.FindAsync(id);
            if (game == null)
                return NotFound("Game not found");

            return Ok(game);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateGame(int id, [FromBody] GameRequest request)
        {
            var game = await _context.SuggestedGames.FindAsync(id);
            if (game == null)
                return NotFound("Game not found");

            game.Title = request.Title;
            game.Condition = request.Condition;
            game.Description = request.Description;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGame(int id)
        {
            var game = await _context.SuggestedGames.FindAsync(id);
            if (game == null)
                return NotFound("Game not found");

            _context.SuggestedGames.Remove(game);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("SuggestGameToChild")]
        public async Task<IActionResult> SuggestGameToChild([FromBody] SuggestGameToChildRequest request)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (doctorId is null)
                return Unauthorized();

            var suggestion = new ChildGameSuggestion
            {
                ChildId = request.ChildId,
                SuggestedGameId = request.SuggestedGameId,
                DoctorId = doctorId
            };

            _context.ChildGameSuggestions.Add(suggestion);
            await _context.SaveChangesAsync();

            return Ok("Game suggested to child successfully");
        }

    }
}
