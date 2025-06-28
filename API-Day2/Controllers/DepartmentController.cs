using API_Day2.Context;
using API_Day2.DTOs;
using API_Day2.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API_Day2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        public ApplicationDbContext _dbContext;
        public IDepartmentRepository _departmentRepository;
        public DepartmentController(ApplicationDbContext dbContext, IDepartmentRepository departmentRepository)
        {
            _dbContext = dbContext;
            _departmentRepository = departmentRepository;
        }

        // GET: api/Department using Customize Serialization
        [HttpGet("Customize Serialization")]
        public async Task<IActionResult> GetDepartments1()
        {
            var departments = await _dbContext.Departments
                .Select(d => new
                {
                    d.Name,
                    NumberOfCourses = d.Courses.Count
                }).ToListAsync();

            if (departments == null || !departments.Any())
            {
                return NotFound("No departments found.");
            }

            return Ok(departments);
        }

        // GET: api/Department using IncludeDto
        [HttpGet("Using Dto")]
        public async Task<IActionResult> GetDepartments2()
        {
            var departments = await _departmentRepository.GetDepartmentsAsync();

            if (departments == null || !departments.Any())
            {
                return NotFound("No departments found.");
            }
            return Ok(departments);
        }

        [HttpGet()]
        public async Task<IActionResult> GetDepartments()
        {
            var departments = await _departmentRepository.GetAllDepartmentsAsync();

            if (departments == null || !departments.Any())
            {
                return NotFound("No departments found.");
            }
            return Ok(departments);
        }
    }
}
