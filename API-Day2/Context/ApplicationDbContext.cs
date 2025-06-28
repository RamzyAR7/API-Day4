using API_Day2.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace API_Day2.Context
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public DbSet<Course> Courses { get; set; }
        public DbSet<Department> Departments { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<Course>(e =>
            {
                e.HasOne(c => c.Department)
                    .WithMany(d => d.Courses)
                    .HasForeignKey(c => c.DepartmentId)
                    .OnDelete(DeleteBehavior.Cascade);
            });
        }
    }
}
