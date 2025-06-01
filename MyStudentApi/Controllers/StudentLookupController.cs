﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyStudentApi.Data;
using MyStudentApi.Models;
using System.Linq;
using System.Threading.Tasks;
using YourNamespace.Models;

namespace MyStudentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentLookupController : ControllerBase
    {
        private readonly AppDbContext _context;

        public StudentLookupController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/StudentLookup/{id}
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentLookup>> GetStudent(int id)
        {
            var student = await _context.StudentLookups
                .FirstOrDefaultAsync(s => s.Student_ID == id);

            if (student == null)
                return NotFound();

            return student;
        }

        // POST: api/StudentLookup
        [HttpPost]
        public async Task<ActionResult<StudentLookup>> CreateStudent([FromBody] StudentLookup newStudent)
        {
            _context.StudentLookups.Add(newStudent);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetStudent), new { id = newStudent.Student_ID }, newStudent);
        }

        // PUT: api/StudentLookup/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateStudent(int id, [FromBody] StudentLookup updatedStudent)
        {
            if (id != updatedStudent.Student_ID)
                return BadRequest("Student ID mismatch.");

            _context.Entry(updatedStudent).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.StudentLookups.Any(s => s.Student_ID == id))
                    return NotFound();
                else
                    throw;
            }

            return NoContent();
        }
    }
}
