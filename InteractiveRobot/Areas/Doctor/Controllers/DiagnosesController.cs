using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InteractiveRobot.Areas.Doctor.Controllers
{
    [Area("Doctor")]
    [Route("api/[area]/[controller]")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Doctor")]
    public class DiagnosesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DiagnosesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> AddDiagnosis([FromBody] DiagnosisRequest request)
        {
            var diagnosis = new Diagnosis
            {
                Title = request.Title,
                ChildId = request.ChildId,
                CreatedAt = DateTime.UtcNow
            };

            _context.Diagnoses.Add(diagnosis);
            await _context.SaveChangesAsync();

            return Created();
        }

        [HttpGet("Child/{childId}")]
        public async Task<IActionResult> GetDiagnosesForChild(int childId)
        {
            var diagnoses = await _context.Diagnoses
                .Where(d => d.ChildId == childId)
                .ToListAsync();

            return Ok(diagnoses);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> EditDiagnosis(int id, [FromBody] DiagnosisRequest request)
        {
            var diagnosis = await _context.Diagnoses.FindAsync(id);

            if (diagnosis is null)
                return NotFound("Diagnosis not found");

            diagnosis.Title = request.Title;
            diagnosis.ChildId = request.ChildId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiagnosis(int id)
        {
            var diagnosis = await _context.Diagnoses.FindAsync(id);

            if (diagnosis is null)
                return NotFound("Diagnosis not found");

            _context.Diagnoses.Remove(diagnosis);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
