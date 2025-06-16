﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyStudentApi.Data;
using MyStudentApi.DTO;
using MyStudentApi.Models;
using MyStudentApi.Helpers;
using System.Threading.Tasks;
using System.Formats.Asn1;
using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using System.IO;


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

        // Optional GET endpoint for a created assignment
        // GET: api/StudentClassAssignment/{id}
        //[HttpGet("{id:int}", Name = "GetAssignment")]
        //public async Task<ActionResult<StudentClassAssignment>> GetAssignment(int id)
        //{
        //    var assignment = await _context.StudentClassAssignments.FindAsync(id);
        //    if (assignment == null)
        //    {
        //        return NotFound();
        //    }
        //    return assignment;
        //}

        // GET all
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentClassAssignment>>> GetAssignments()
            => await _context.StudentClassAssignments.ToListAsync();

        // GET by id
        [HttpGet("{id}")]
        public async Task<ActionResult<StudentClassAssignment>> GetAssignment(int id)
        {
            var a = await _context.StudentClassAssignments.FindAsync(id);
            if (a == null) return NotFound();
            return a;
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

        // PUT: api/StudentClassAssignment/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAssignment(int id, [FromBody] StudentAssignmentUpdateDto dto)
        {
            var existing = await _context.StudentClassAssignments.FindAsync(id);
            if (existing == null) return NotFound();

            // Only update what the UI is allowed to modify
            existing.Position_Number = dto.Position_Number;
            existing.I9_Sent = dto.I9_Sent;
            existing.SSN_Sent = dto.SSN_Sent;
            existing.Offer_Sent = dto.Offer_Sent;
            existing.Offer_Signed = dto.Offer_Signed;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("upload")]
        public async Task<IActionResult> UploadAssignmentsFromCsv([FromForm] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File is empty or missing.");

            var assignments = new List<StudentClassAssignment>();
            var now = DateTime.UtcNow;

            using (var reader = new StreamReader(file.OpenReadStream()))
            using (var csv = new CsvReader(reader, new CsvHelper.Configuration.CsvConfiguration(CultureInfo.InvariantCulture)
            {
                TrimOptions = TrimOptions.Trim,
                IgnoreBlankLines = true,
                HeaderValidated = null,
                MissingFieldFound = null
            }))
            {
                var records = csv.GetRecords<StudentClassAssignment>().ToList();

                foreach (var record in records)
                {
                    // Ensure calculated fields are set
                    record.CreatedAt = now;
                    record.Term = "2254";
                    record.Location = "TEMPE";
                    record.Campus = "TEMPE";
                    record.AcadCareer = "UGRD";

                    record.Compensation = AssignmentUtils.CalculateCompensation(record);
                    record.CostCenterKey = AssignmentUtils.ComputeCostCenterKey(record);

                    assignments.Add(record);
                }
            }

            _context.StudentClassAssignments.AddRange(assignments);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"{assignments.Count} records uploaded successfully." });
        }

    }
}
