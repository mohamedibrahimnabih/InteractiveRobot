using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InteractiveRobot.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    public class SpecialtiesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public SpecialtiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_context.Specialties.ToList());
        }

        [HttpPost]
        public IActionResult Create([FromQuery] string name)
        {
            if (_context.Specialties.Any(s => s.Name == name))
                return BadRequest("Specialty already exists");

            _context.Specialties.Add(new Specialty { Name = name });
            _context.SaveChanges();

            return CreatedAtAction(nameof(GetAll), new { name }, null);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var specialty = _context.Specialties.Find(id);
            if (specialty is null) return NotFound();

            _context.Specialties.Remove(specialty);
            _context.SaveChanges();

            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromQuery] string name)
        {
            var specialty = _context.Specialties.Find(id);
            if (specialty is null) return NotFound();

            specialty.Name = name;
            _context.SaveChanges();

            return NoContent();
        }
    }
}
