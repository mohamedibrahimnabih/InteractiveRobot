using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InteractiveRobot.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SuperAdmin,Admin")]
    public class StatisticsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public StatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("Overview")]
        public IActionResult GetStatistics()
        {
            var totalParents = _context.Users.Count(u => u.UserType == UserType.Parent);
            var totalDoctors = _context.Users.Count(u => u.UserType == UserType.Doctor);

            var parentChildStats = _context.Users
                .Where(u => u.UserType == UserType.Parent)
                .Select(u => new
                {
                    ParentName = u.Name,
                    NumberOfChildren = u.Children!.Count
                }).ToList();

            var doctorCaseStats = _context.Users
                .Where(u => u.UserType == UserType.Doctor)
                .Select(u => new
                {
                    DoctorName = u.Name,
                    NumberOfCases = _context.Children.Count(c => c.DoctorId == u.Id)
                }).ToList();

            return Ok(new
            {
                TotalParents = totalParents,
                TotalDoctors = totalDoctors,
                Parents = parentChildStats,
                Doctors = doctorCaseStats
            });
        }
    }
}
