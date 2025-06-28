using API_Day2.Context;
using API_Day2.DTOs;
using Microsoft.EntityFrameworkCore;

namespace API_Day2.Repository
{
    public class CoursesRepository : ICoursesRepository
    {
        public ApplicationDbContext _context { get; set; }

        public CoursesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CoursesDto>> GetAllCoursesAsync()
        {
            return await _context.Courses
                .Select(c => new CoursesDto
                {
                    Id = c.Id,
                    Crs_Name = c.Crs_Name,
                    Crs_Description = c.Crs_Description,
                    Duration = c.Duration,
                    DepartmentId = c.DepartmentId,
                    DepartmentName = c.Department.Name
                })
                .ToListAsync();
        }
    }
}
