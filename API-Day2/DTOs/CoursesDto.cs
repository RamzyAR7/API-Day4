namespace API_Day2.DTOs
{
    public class CoursesDto
    {
        public int Id { get; set; }
        public string Crs_Name { get; set; } = string.Empty;
        public string? Crs_Description { get; set; }
        public int Duration { get; set; }
        public int DepartmentId { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }
}
