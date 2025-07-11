using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InteractiveRobot.Areas.Parent.Controllers
{
    [Area("Parent")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Parent")]
    public class RatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddRating([FromBody] AddRatingRequest ratingRequest)
        {
            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (parentId is null)
                return Unauthorized();

            var hasDoctorTreatedMyChild = await _context.Children
                .AnyAsync(c =>
                    c.DoctorId == ratingRequest.DoctorId &&
                    c.ParentId == parentId
                );

            if (!hasDoctorTreatedMyChild)
                return BadRequest("You can only rate doctors who treated your children");

            var existingRating = await _context.DoctorRatings
                .FirstOrDefaultAsync(r => r.ParentId == parentId && r.DoctorId == ratingRequest.DoctorId);

            if (existingRating is not null)
                return BadRequest("You already rated this doctor");

            var newRating = new DoctorRating
            {
                DoctorId = ratingRequest.DoctorId,
                ParentId = parentId,
                Stars = ratingRequest.Stars,
                Comment = ratingRequest.Comment,
                Date = DateTime.UtcNow
            };

            _context.DoctorRatings.Add(newRating);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetRatingsForDoctor), new { doctorId = ratingRequest.DoctorId }, null);
        }



        [HttpGet("Doctor/{doctorId}")]
        public async Task<IActionResult> GetRatingsForDoctor(string doctorId)
        {
            var ratings = await _context.DoctorRatings
                .Where(r => r.DoctorId == doctorId)
                .Select(r => new
                {
                    r.Stars,
                    r.Comment,
                    ParentName = r.Parent!.Name ?? "Unknown",
                    r.Date
                })
                .ToListAsync();

            return Ok(ratings);
        }

        [HttpGet("MyRatings")]
        public async Task<IActionResult> GetMyRatings()
        {
            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (parentId is null)
                return Unauthorized();

            var myRatings = await _context.DoctorRatings
                .Where(r => r.ParentId == parentId)
                .Select(r => new
                {
                    r.Id,
                    r.DoctorId,
                    DoctorName = r.Doctor!.Name,
                    r.Stars,
                    r.Comment,
                    r.Date
                })
                .ToListAsync();

            return Ok(myRatings);
        }

        // GET: api/Parent/Ratings/FilterMyRatings?minStars=4&from=2025-07-01&to=2025-07-10
        [HttpGet("FilterMyRatings")]
        public async Task<IActionResult> FilterMyRatings([FromQuery] int? minStars, [FromQuery] DateTime? from, [FromQuery] DateTime? to)
        {
            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (parentId is null)
                return Unauthorized();

            var query = _context.DoctorRatings
                .Where(r => r.ParentId == parentId);

            if (minStars.HasValue)
                query = query.Where(r => r.Stars >= minStars.Value);

            if (from.HasValue)
                query = query.Where(r => r.Date >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(r => r.Date <= to.Value.Date.AddDays(1).AddTicks(-1));

            var filteredRatings = await query
                .Select(r => new
                {
                    r.Id,
                    r.DoctorId,
                    DoctorName = r.Doctor!.Name,
                    r.Stars,
                    r.Comment,
                    r.Date
                })
                .ToListAsync();

            return Ok(filteredRatings);
        }

    }
}
