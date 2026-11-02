using Microsoft.EntityFrameworkCore;
using OnlineLearningPlatform.Data.Configurations;
using OnlineLearningPlatform.Models;
namespace OnlineLearningPlatform.Data;
public class AppDbContext : DbContext
{
 public DbSet<Instructor> Instructors => Set<Instructor>();
 public DbSet<Category> Categories => Set<Category>();
 public DbSet<Course> Courses => Set<Course>();
 public DbSet<Student> Students => Set<Student>();
 public DbSet<Enrollment> Enrollments => Set<Enrollment>();
 public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
 protected override void OnModelCreating(ModelBuilder modelBuilder)
 {
  modelBuilder.ApplyConfiguration(new InstructorConfiguration());
  modelBuilder.ApplyConfiguration(new CategoryConfiguration());
  modelBuilder.ApplyConfiguration(new CourseConfiguration());
  modelBuilder.ApplyConfiguration(new StudentConfiguration());
  modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
 }
}
