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
    public class RatingsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public RatingsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/Doctor/Ratings/MyRatings
        [HttpGet("MyRatings")]
        public async Task<IActionResult> GetMyRatings()
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (doctorId is null)
                return Unauthorized();

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

        // GET: api/Doctor/Ratings/MyAverage
        [HttpGet("MyAverage")]
        public async Task<IActionResult> GetMyAverageRating()
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (doctorId is null)
                return Unauthorized();

            var avgRating = await _context.DoctorRatings
                .Where(r => r.DoctorId == doctorId)
                .AverageAsync(r => (double?)r.Stars) ?? 0;

            var count = await _context.DoctorRatings.CountAsync(r => r.DoctorId == doctorId);

            return Ok(new
            {
                DoctorId = doctorId,
                AverageStars = Math.Round(avgRating, 2),
                Count = count
            });
        }
    }
}
