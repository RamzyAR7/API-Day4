using API_Day2.Context;
using API_Day2.Controllers;
using API_Day2.DTOs;
using API_Day2.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace API_Day2.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly ApplicationDbContext _context;

        public DepartmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<DepartmentDto>> GetDepartmentsAsync()
        {
            var departmentDtos = await _context.Departments.Include(d => d.Courses)
                           .Select(d => new DepartmentDto
                           {
                               Name = d.Name,
                               NumberOfCourses = d.Courses.Count
                           }).ToListAsync();
            return departmentDtos;
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }
    }
}
