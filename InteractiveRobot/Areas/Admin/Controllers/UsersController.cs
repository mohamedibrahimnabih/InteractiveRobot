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
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public UsersController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAllUsers()
        {
            var users = _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Name,
                    u.Email,
                    u.UserType,
                    u.IsBanned
                }).ToList();

            return Ok(users);
        }

        [HttpPatch("Ban/{id}")]
        public IActionResult BanUser(string id)
        {
            var user = _context.Users.Find(id);
            if (user is null) return NotFound();

            user.IsBanned = true;
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPatch("Unban/{id}")]
        public IActionResult UnbanUser(string id)
        {
            var user = _context.Users.Find(id);
            if (user is null) return NotFound();

            user.IsBanned = false;
            _context.SaveChanges();

            return NoContent();
        }
    }
}
