using API_Day2.Models;
using API_Day2.Context;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using API_Day2.Repository;
using Microsoft.AspNetCore.Authorization;

namespace API_Day2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        public ApplicationDbContext _context;
        public ICoursesRepository _coursesRepository;
        public CourseController(ApplicationDbContext context, ICoursesRepository coursesRepository)
        {
            _context = context;
            _coursesRepository = coursesRepository;
        }

        [HttpGet]
        public async Task<IActionResult> get()
        {
            var courses = await  _coursesRepository.GetAllCoursesAsync();
            if (courses == null || !courses.Any())
            {
                return NotFound("No courses found.");
            }
            return Ok(courses);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> deleteCourse(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound($"Course with ID {id} not found.");
            }
            _context.Courses.Remove(course);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> put(int id, [FromBody] Course course)
        {
            if (id != course.Id)
            {
                return BadRequest("Course ID mismatch.");
            }
            var Course = await _context.Courses.FindAsync(id);
            if (Course == null)
            {
                return NotFound($"Course with ID {id} not found.");
            }

            Course.Crs_Name = course.Crs_Name;
            Course.Crs_Description = course.Crs_Description;
            Course.Duration = course.Duration;
            _context.Courses.Update(Course); 
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> post([FromBody] Course course)
        {
            if (course == null)
            {
                return BadRequest("Course data is null.");
            }
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return CreatedAtAction(nameof(getById), new { id = course.Id }, course); 
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> getById(int id)
        {
            var course = await _context.Courses.FindAsync(id);
            if (course == null)
            {
                return NotFound($"Course with ID {id} not found.");
            }
            return Ok(course);
        }
        [HttpGet("name/{name}")]
        public async Task<IActionResult> courseByName(string name)
        {
            var course = await _context.Courses.FirstOrDefaultAsync(c => c.Crs_Name.ToLower() == name.ToLower());
            if (course == null)
            {
                return NotFound($"Course with name {name} not found.");
            }
            return Ok(course);
        }

        [HttpGet("AdminStats")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAdminStats()
        {
            var totalCourses = await _context.Courses.CountAsync();
            var totalDepartments = await _context.Departments.CountAsync();
            
            return Ok(new
            {
                message = "Admin Statistics",
                totalCourses = totalCourses,
                totalDepartments = totalDepartments,
                timestamp = DateTime.Now,
                adminUser = User.Identity?.Name
            });
        }
    }
}
