using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.Models;
namespace OnlineLearningPlatform.Data.Configurations;
public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
{
 public void Configure(EntityTypeBuilder<Enrollment> b) { b.HasKey(x=>new{x.StudentId,x.CourseId}); b.Property(x=>x.Progress).HasPrecision(5,2); b.Property(x=>x.FinalGrade).HasPrecision(5,2); b.Property(x=>x.Status).HasConversion<int>(); b.Property(x=>x.EnrolledAt).IsRequired(); b.HasOne<Student>().WithMany().HasForeignKey(x=>x.StudentId).OnDelete(DeleteBehavior.Cascade); b.HasOne<Course>().WithMany().HasForeignKey(x=>x.CourseId).OnDelete(DeleteBehavior.Cascade); }
}
