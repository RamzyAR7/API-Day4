using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace API_Day2.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage = "Department name is required.")]
        public string Name { get; set; }
        
        // Navigation property 
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
