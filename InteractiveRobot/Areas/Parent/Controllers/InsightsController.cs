using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InteractiveRobot.Areas.Parent.Controllers
{
    [Area("Parent")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Parent")]
    public class InsightsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public InsightsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Parent/Insights/Diagnoses
        [HttpGet("Diagnoses")]
        public async Task<IActionResult> GetMyChildrenDiagnoses()
        {
            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var diagnoses = await _context.Diagnoses
                .Include(d => d.Child)
                .Where(d => d.Child!.ParentId == parentId)
                .Select(d => new
                {
                    d.Id,
                    d.Title,
                    d.CreatedAt,
                    ChildName = d.Child!.Name
                })
                .ToListAsync();

            return Ok(diagnoses);
        }

        // GET: api/Parent/Insights/GameSuggestions
        [HttpGet("GameSuggestions")]
        public async Task<IActionResult> GetMyChildrenGameSuggestions(
    [FromQuery] string? condition,
    [FromQuery] string? doctorId,
    [FromQuery] DateTime? from,
    [FromQuery] DateTime? to)
        {
            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var query = _context.ChildGameSuggestions
                .Include(s => s.Child)
                .Include(s => s.SuggestedGame)
                .Include(s => s.Doctor)
                .Where(s => s.Child!.ParentId == parentId)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(condition))
                query = query.Where(s => s.SuggestedGame!.Condition.ToLower().Contains(condition.ToLower()));

            if (!string.IsNullOrWhiteSpace(doctorId))
                query = query.Where(s => s.DoctorId == doctorId);

            if (from.HasValue)
                query = query.Where(s => s.SuggestedAt >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(s => s.SuggestedAt <= to.Value.Date.AddDays(1).AddTicks(-1));

            var suggestions = await query.ToListAsync();

            var grouped = suggestions
                .GroupBy(s => new { s.ChildId, s.Child!.Name })
                .Select(g => new
                {
                    ChildId = g.Key.ChildId,
                    ChildName = g.Key.Name,
                    Suggestions = g.Select(s => new
                    {
                        GameTitle = s.SuggestedGame!.Title,
                        Condition = s.SuggestedGame.Condition,
                        s.SuggestedAt,
                        DoctorName = s.Doctor!.Name
                    }).ToList()
                })
                .ToList();

            return Ok(grouped);
        }

    }
}
