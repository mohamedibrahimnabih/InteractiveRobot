using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InteractiveRobot.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
    public class PatientsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PatientsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyPatients()
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var doctorSpecialties = await _context.DoctorSpecialties
                .Where(s => s.DoctorId == doctorId)
                .Select(s => s.Specialty.Name)
                .ToListAsync();

            var children = await _context.Children
                .Where(c => c.DoctorId == doctorId || doctorSpecialties.Contains(c.Condition))
                .ToListAsync();

            return Ok(children);
        }

        [HttpGet("{childId}")]
        public async Task<IActionResult> GetChildDetails(int childId)
        {
            var doctorId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var child = await _context.Children
                .Include(c => c.Diagnoses)
                .FirstOrDefaultAsync(c => c.Id == childId && c.DoctorId == doctorId);

            if (child == null)
                return NotFound();

            return Ok(child);
        }
    }
}
