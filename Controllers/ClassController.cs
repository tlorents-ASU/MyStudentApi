﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MyStudentApi.Data;
using MyStudentApi.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStudentApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ClassController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ClassController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Class/subjects?term=2254
        [HttpGet("subjects")]
        public async Task<ActionResult<IEnumerable<string>>> GetSubjects([FromQuery] string term)
        {
            if (string.IsNullOrEmpty(term))
                return BadRequest("Term is required.");

            // For this example, we focus on term "2254"
            if (term == "2254")
            {
                var subjects = await _context.ClassSchedule2254
                    .Where(c => c.Term == term)
                    .Select(c => c.Subject)
                    .Distinct()
                    .ToListAsync();
                return subjects;
            }
            else if (term == "2251")
            {
                var subjects = await _context.ClassLookups
                    .Where(c => c.Term == term)
                    .Select(c => c.Subject)
                    .Distinct()
                    .ToListAsync();
                return subjects;
            }
            else
            {
                return BadRequest("Invalid term value.");
            }
        }

        // GET: api/Class/catalog?term=2254&subject=XYZ
        [HttpGet("catalog")]
        public async Task<ActionResult<IEnumerable<string>>> GetCatalogNumbers([FromQuery] string term, [FromQuery] string subject)
        {
            if (string.IsNullOrEmpty(term) || string.IsNullOrEmpty(subject))
                return BadRequest("Term and Subject are required.");

            if (term == "2254")
            {
                var catalogs = await _context.ClassSchedule2254
                    .Where(c => c.Term == term && c.Subject == subject)
                    .Select(c => c.CatalogNum.ToString())
                    .Distinct()
                    .ToListAsync();
                return catalogs;
            }
            else if (term == "2251")
            {
                var catalogs = await _context.ClassLookups
                    .Where(c => c.Term == term && c.Subject == subject)
                    .Select(c => c.CatalogNum.ToString())
                    .Distinct()
                    .ToListAsync();
                return catalogs;
            }
            else
            {
                return BadRequest("Invalid term value.");
            }
        }

        // GET: api/Class/classnumbers?term=2254&subject=XYZ&catalogNum=123
        [HttpGet("classnumbers")]
        public async Task<ActionResult<IEnumerable<string>>> GetClassNumbers([FromQuery] string term, [FromQuery] string subject, [FromQuery] string catalogNum)
        {
            if (string.IsNullOrEmpty(term) || string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(catalogNum))
                return BadRequest("Term, Subject, and CatalogNum are required.");

            if (!short.TryParse(catalogNum, out short catNum))
                return BadRequest("CatalogNum must be numeric.");

            if (term == "2254")
            {
                var classNumbers = await _context.ClassSchedule2254
                    .Where(c => c.Term == term && c.Subject == subject && c.CatalogNum == catNum)
                    .Select(c => c.ClassNum)
                    .Distinct()
                    .ToListAsync();
                return classNumbers;
            }
            else if (term == "2251")
            {
                var classNumbers = await _context.ClassLookups
                    .Where(c => c.Term == term && c.Subject == subject && c.CatalogNum == catNum)
                    .Select(c => c.ClassNum)
                    .Distinct()
                    .ToListAsync();
                return classNumbers;
            }
            else
            {
                return BadRequest("Invalid term value.");
            }
        }

        // GET: api/Class/details/{classNum}?term=2254
        [HttpGet("details/{classNum}")]
        public async Task<ActionResult<object>> GetClassDetails(string classNum, [FromQuery] string term)
        {
            if (string.IsNullOrEmpty(classNum) || string.IsNullOrEmpty(term))
                return BadRequest("ClassNum and Term are required.");

            if (term == "2254")
            {
                var classInfo = await _context.ClassSchedule2254.FirstOrDefaultAsync(c => c.ClassNum == classNum && c.Term == term);
                if (classInfo == null)
                    return NotFound();
                return new
                {
                    classInfo.Session,
                    classInfo.Term,
                    classInfo.InstructorID,
                    classInfo.InstructorFirstName,
                    classInfo.InstructorLastName,
                    classInfo.InstructorEmail,
                    classInfo.Location
                };
            }
            else if (term == "2251")
            {
                var classInfo = await _context.ClassLookups.FirstOrDefaultAsync(c => c.ClassNum == classNum && c.Term == term);
                if (classInfo == null)
                    return NotFound();
                return new
                {
                    classInfo.Session,
                    classInfo.Term,
                    classInfo.InstructorID,
                    classInfo.InstructorFirstName,
                    classInfo.InstructorLastName,
                    classInfo.InstructorEmail,
                    classInfo.Location
                };
            }
            else
            {
                return BadRequest("Invalid term value.");
            }
        }
    }
}





// ------------------ Version 2/12/2025 where had Student Lookup, Add Student box then ClassLookup cascade

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MyStudentApi.Data;
//using MyStudentApi.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MyStudentApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ClassController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public ClassController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/Class/subjects
//        // Returns a list of distinct Subjects.
//        [HttpGet("subjects")]
//        public async Task<ActionResult<IEnumerable<string>>> GetSubjects()
//        {
//            var subjects = await _context.ClassLookups
//                .Select(c => c.Subject)
//                .Distinct()
//                .ToListAsync();
//            return subjects;
//        }

//        // GET: api/Class/catalog?subject={subject}
//        // Returns distinct CatalogNum values (as strings) for a given subject.
//        [HttpGet("catalog")]
//        public async Task<ActionResult<IEnumerable<string>>> GetCatalogNumbers([FromQuery] string subject)
//        {
//            if (string.IsNullOrEmpty(subject))
//                return BadRequest("Subject is required.");

//            // Convert CatalogNum to string for display purposes.
//            var catalogs = await _context.ClassLookups
//                .Where(c => c.Subject == subject)
//                .Select(c => c.CatalogNum.ToString())
//                .Distinct()
//                .ToListAsync();

//            return catalogs;
//        }

//        // GET: api/Class/classnumbers?subject={subject}&catalogNum={catalogNum}
//        // Returns distinct ClassNum values (as strings) for a given subject and catalog number.
//        [HttpGet("classnumbers")]
//        public async Task<ActionResult<IEnumerable<string>>> GetClassNumbers([FromQuery] string subject, [FromQuery] string catalogNum)
//        {
//            if (string.IsNullOrEmpty(subject) || string.IsNullOrEmpty(catalogNum))
//                return BadRequest("Subject and CatalogNum are required.");

//            if (!int.TryParse(catalogNum, out int catNum))
//                return BadRequest("CatalogNum must be numeric.");

//            var classNumbers = await _context.ClassLookups
//                .Where(c => c.Subject == subject && c.CatalogNum == catNum)
//                .Select(c => c.ClassNum)
//                .Distinct()
//                .ToListAsync();

//            return classNumbers;
//        }

//        // GET: api/Class/details/{classNum}
//        // Returns full class details for a given ClassNum.
//        [HttpGet("details/{classNum}")]
//        public async Task<ActionResult<ClassLookup>> GetClassDetails(string classNum)
//        {
//            if (string.IsNullOrEmpty(classNum))
//                return BadRequest("ClassNum is required.");

//            var classInfo = await _context.ClassLookups.FirstOrDefaultAsync(c => c.ClassNum == classNum);
//            if (classInfo == null)
//                return NotFound();
//            return classInfo;
//        }
//    }
//}


// ----- Version from 2/10/2024 

// pulling class info into from the classNum

//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MyStudentApi.Data;
//using MyStudentApi.Models;
//using System.Threading.Tasks;

//namespace MyStudentApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ClassController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public ClassController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/Class/{id}
//        // Now accepts a string because our model's ClassNum is a string.
//        [HttpGet("{id}")]
//        public async Task<ActionResult<ClassLookup>> GetClass(string id)
//        {
//            var classInfo = await _context.ClassLookups.FirstOrDefaultAsync(c => c.ClassNum == id);
//            if (classInfo == null)
//            {
//                return NotFound();
//            }
//            return classInfo;
//        }

//        // PUT: api/Class/{id}
//        [HttpPut("{id}")]
//        public async Task<IActionResult> UpdateClass(string id, [FromBody] ClassLookup updatedClass)
//        {
//            if (id != updatedClass.ClassNum)
//            {
//                return BadRequest("Class number mismatch");
//            }

//            _context.Entry(updatedClass).State = EntityState.Modified;

//            try
//            {
//                await _context.SaveChangesAsync();
//            }
//            catch (DbUpdateConcurrencyException)
//            {
//                if (!_context.ClassLookups.Any(c => c.ClassNum == id))
//                {
//                    return NotFound();
//                }
//                else
//                {
//                    throw;
//                }
//            }

//            return NoContent();
//        }
//    }
//}







//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using MyStudentApi.Data;
//using MyStudentApi.Models;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace MyStudentApi.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class ClassController : ControllerBase
//    {
//        private readonly AppDbContext _context;

//        public ClassController(AppDbContext context)
//        {
//            _context = context;
//        }

//        // GET: api/Class/{id}
//        [HttpGet("{id}")]
//        public async Task<ActionResult<ClassLookup>> GetClass(short id)
//        {
//            var classInfo = await _context.ClassLookups.FirstOrDefaultAsync(c => c.ClassNum == id);
//            if (classInfo == null)
//            {
//                return NotFound();
//            }
//            return classInfo;
//        }

//        // GET: api/Class/subjects
//        // Returns a list of distinct Subjects.
//        [HttpGet("subjects")]
//        public async Task<ActionResult<IEnumerable<string>>> GetSubjects()
//        {
//            var subjects = await _context.ClassLookups
//                .Select(c => c.Subject)
//                .Distinct()
//                .ToListAsync();
//            return subjects;
//        }

//        // GET: api/Class/catalog?subject={subject}
//        // Returns distinct CatalogNum values for a given subject.
//        [HttpGet("catalog")]
//        public async Task<ActionResult<IEnumerable<int>>> GetCatalogNumbers([FromQuery] string subject)
//        {
//            if (string.IsNullOrEmpty(subject))
//                return BadRequest("Subject is required.");

//            var catalogs = await _context.ClassLookups
//                .Where(c => c.Subject == subject)
//                .Select(c => c.CatalogNum)
//                .Distinct()
//                .ToListAsync();

//            return catalogs;
//        }

//        // GET: api/Class/details?subject={subject}&catalogNum={catalogNum}
//        // Returns the class details (e.g. Session and ClassNum) for the given subject and catalog number.
//        [HttpGet("details")]
//        public async Task<ActionResult<IEnumerable<ClassLookup>>> GetClassDetails([FromQuery] string subject, [FromQuery] int catalogNum)
//        {
//            if (string.IsNullOrEmpty(subject))
//                return BadRequest("Subject is required.");

//            var classes = await _context.ClassLookups
//                .Where(c => c.Subject == subject && c.CatalogNum == catalogNum)
//                .ToListAsync();

//            if (classes == null || !classes.Any())
//                return NotFound("No classes found for the given criteria.");

//            return classes;
//        }

//        // (Optional: keep your existing GET endpoints as well)
//    }
//}
