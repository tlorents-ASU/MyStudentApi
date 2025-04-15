using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyStudentApi.Data;
using MyStudentApi.Models;
using System.Threading.Tasks;

namespace MyStudentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentClassAssignmentController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentClassAssignmentController(AppDbContext context)
        {
            _context = context;
        }

        // POST: api/StudentClassAssignment
        [HttpPost]
        public async Task<ActionResult<StudentClassAssignment>> CreateAssignment([FromBody] StudentClassAssignment assignment)
        {
            // You could add validation here if needed.

            _context.StudentClassAssignments.Add(assignment);
            await _context.SaveChangesAsync();

            // Returns a 201 Created with the location header pointing to the new assignment.
            return CreatedAtAction(nameof(GetAssignment), new { id = assignment.Id }, assignment);
        }

        // Optional GET endpoint for a created assignment
        // GET: api/StudentClassAssignment/{id}
        [HttpGet("{id:int}", Name = "GetAssignment")]
        public async Task<ActionResult<StudentClassAssignment>> GetAssignment(int id)
        {
            var assignment = await _context.StudentClassAssignments.FindAsync(id);
            if (assignment == null)
            {
                return NotFound();
            }
            return assignment;
        }

        [HttpGet("totalhours/{studentId}")]
        public async Task<ActionResult<int>> GetTotalAssignedHours(int studentId)
        {
            // Sum the WeeklyHours for assignments matching the studentId.
            // If there are no assignments, default to 0.
            var totalHours = await _context.StudentClassAssignments
                .Where(a => a.Student_ID == studentId)
                .SumAsync(a => (int?)a.WeeklyHours) ?? 0;
            return totalHours;
        }

    }
}
