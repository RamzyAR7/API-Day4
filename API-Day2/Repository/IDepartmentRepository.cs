using API_Day2.DTOs;
using API_Day2.Models;

namespace API_Day2.Repository
{
    public interface IDepartmentRepository
    {
        Task<List<DepartmentDto>> GetDepartmentsAsync();
        Task<List<Department>> GetAllDepartmentsAsync();
    }
}
