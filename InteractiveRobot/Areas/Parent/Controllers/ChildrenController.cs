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
    public class ChildrenController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ChildrenController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetMyChildren()
        {
            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var children = await _context.Children
                .Where(c => c.ParentId == parentId)
                .ToListAsync();

            return Ok(children);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetChildById(int id)
        {
            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var child = await _context.Children
                .Include(e => e.Diagnoses)
                .FirstOrDefaultAsync(c => c.Id == id && c.ParentId == parentId);

            if (child == null)
                return NotFound();

            return Ok(child);
        }

        [HttpPost]
        public async Task<IActionResult> AddChild([FromBody] ChildRequest request)
        {
            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (parentId == null)
                return NotFound();

            var child = new Child
            {
                Name = request.Name,
                Age = request.Age,
                Condition = request.Condition,
                ParentId = parentId
            };

            _context.Children.Add(child);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetChildById), new { id = child.Id }, child);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateChild(int id, [FromBody] ChildRequest request)
        {
            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var child = await _context.Children
                .FirstOrDefaultAsync(c => c.Id == id && c.ParentId == parentId);

            if (child == null)
                return NotFound();

            child.Name = request.Name;
            child.Age = request.Age;
            child.Condition = request.Condition;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteChild(int id)
        {
            var parentId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var child = await _context.Children
                .FirstOrDefaultAsync(c => c.Id == id && c.ParentId == parentId);

            if (child == null)
                return NotFound();

            _context.Children.Remove(child);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
