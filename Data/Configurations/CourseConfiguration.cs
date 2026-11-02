using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.Models;
namespace OnlineLearningPlatform.Data.Configurations;
public class CourseConfiguration : IEntityTypeConfiguration<Course>
{
 public void Configure(EntityTypeBuilder<Course> b) { b.HasKey(x=>x.Id); b.Property(x=>x.Title).IsRequired().HasMaxLength(150); b.Property(x=>x.Description).HasMaxLength(1000); b.Property(x=>x.Price).HasPrecision(10,2); b.Property(x=>x.Level).HasConversion<int>(); b.Property(x=>x.IsPublished).IsRequired(); b.HasOne<Instructor>().WithMany().HasForeignKey(x=>x.InstructorId).OnDelete(DeleteBehavior.Restrict); b.HasOne<Category>().WithMany().HasForeignKey(x=>x.CategoryId).OnDelete(DeleteBehavior.Restrict); b.HasData(
 new Course{Id=1,Title="C# Fundamentals",Description="Learn C# basics",Price=49.99m,Level=CourseLevel.Beginner,IsPublished=true,PublishedAt=new DateTime(2024,1,10),InstructorId=1,CategoryId=1},
 new Course{Id=2,Title="ASP.NET Core Web API",Description="Build APIs with ASP.NET Core",Price=89.99m,Level=CourseLevel.Intermediate,IsPublished=true,PublishedAt=new DateTime(2024,2,10),InstructorId=2,CategoryId=3},
 new Course{Id=3,Title="Advanced EF Core",Description="Advanced database access",Price=119.50m,Level=CourseLevel.Advanced,IsPublished=true,PublishedAt=new DateTime(2024,3,5),InstructorId=2,CategoryId=2},
 new Course{Id=4,Title="HTML and CSS",Description="Web foundations",Price=39.00m,Level=CourseLevel.Beginner,IsPublished=true,PublishedAt=new DateTime(2024,4,1),InstructorId=3,CategoryId=1},
 new Course{Id=5,Title="Clean Architecture",Description="Architecture patterns",Price=99.00m,Level=CourseLevel.Advanced,IsPublished=false,PublishedAt=null,InstructorId=1,CategoryId=2},
 new Course{Id=6,Title="UI Design Basics",Description="Design user interfaces",Price=59.00m,Level=CourseLevel.Intermediate,IsPublished=false,PublishedAt=null,InstructorId=3,CategoryId=5}); }
}
