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
    public class DoctorSelectionsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DoctorSelectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("AvailableDoctors")]
        public async Task<IActionResult> GetAvailableDoctors([FromQuery] string condition)
        {
            var doctors = await _context.Users
                .Where(u => u.UserType == UserType.Doctor &&
                            u.DoctorSpecialties!.Any(ds => ds.Specialty!.Name.Contains(condition)))
                .Select(d => new
                {
                    d.Id,
                    d.Name,
                    d.Email,
                    Ratings = _context.DoctorRatings
                                .Where(r => r.DoctorId == d.Id)
                                .Include(r => r.Parent)
                                .Select(r => new
                                {
                                    r.Stars,
                                    r.Comment,
                                    ParentName = r.Parent!.Name ?? "Unknown",
                                    r.Date
                                })
                                .ToList(),
                    AvgRating = _context.DoctorRatings
                                .Where(r => r.DoctorId == d.Id)
                                .Select(r => r.Stars)
                                .DefaultIfEmpty()
                                .Average()
                })
                .ToListAsync();

            return Ok(doctors);
        }


        [HttpPost("AssignDoctor")]
        public async Task<IActionResult> AssignDoctor([FromBody] AssignDoctorRequest request)
        {
            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var child = await _context.Children.FirstOrDefaultAsync(c => c.Id == request.ChildId && c.ParentId == parentId);

            if (child is null)
                return NotFound("Child not found or not owned by this parent");

            var doctor = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.DoctorId && u.UserType == UserType.Doctor);

            if (doctor is null)
                return NotFound("Doctor not found");

            child.DoctorId = request.DoctorId;
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
