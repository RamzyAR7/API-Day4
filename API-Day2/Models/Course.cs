using System.ComponentModel.DataAnnotations;

namespace API_Day2.Models
{
    public class Course
    {
        [Key]
        public int Id{ get; set; }
        [Required(ErrorMessage = "Course name is required.")]
        [MaxLength(50, ErrorMessage ="MAX Course length is 50")]
        public string Crs_Name { get; set; }

        [MaxLength(150, ErrorMessage = "MAX Course length is 150")]
        public string? Crs_Description { get; set; }
        [Required(ErrorMessage ="Duration is required")]
        public int Duration { get; set; }

        // Foreign key for Department
        public int DepartmentId { get; set; }
        // Navigation property for Department
        public Department Department { get; set; }
    }
}
