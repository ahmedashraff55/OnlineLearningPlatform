using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.Models;
namespace OnlineLearningPlatform.Data.Configurations;
public class StudentConfiguration : IEntityTypeConfiguration<Student>
{
 public void Configure(EntityTypeBuilder<Student> b) { b.HasKey(x=>x.Id); b.Property(x=>x.FullName).IsRequired().HasMaxLength(100); b.Property(x=>x.Email).IsRequired().HasMaxLength(150); b.HasIndex(x=>x.Email).IsUnique(); b.Property(x=>x.DateOfBirth).IsRequired(); }
}
