namespace API_Day2.Repository
{
    public interface ICoursesRepository
    {
        Task<IEnumerable<DTOs.CoursesDto>> GetAllCoursesAsync();
    }
}
