using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.Models;
namespace OnlineLearningPlatform.Data.Configurations;
public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
 public void Configure(EntityTypeBuilder<Instructor> b) { b.HasKey(x=>x.Id); b.Property(x=>x.FullName).IsRequired().HasMaxLength(100); b.Property(x=>x.Email).IsRequired().HasMaxLength(150); b.HasIndex(x=>x.Email).IsUnique(); b.Property(x=>x.Bio).HasMaxLength(500); b.Property(x=>x.JoinedAt).IsRequired(); b.HasData(
 new Instructor{Id=1,FullName="John Smith",Email="john@learn.com",Bio="Software engineer and instructor.",JoinedAt=new DateTime(2023,1,10)},
 new Instructor{Id=2,FullName="Sarah Johnson",Email="sarah@learn.com",Bio="Backend development instructor.",JoinedAt=new DateTime(2023,3,15)},
 new Instructor{Id=3,FullName="Michael Brown",Email="michael@learn.com",Bio="Design and programming instructor.",JoinedAt=new DateTime(2024,2,1)}); }
}
