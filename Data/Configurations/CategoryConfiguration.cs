using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineLearningPlatform.Models;
namespace OnlineLearningPlatform.Data.Configurations;
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
 public void Configure(EntityTypeBuilder<Category> b) { b.HasKey(x=>x.Id); b.Property(x=>x.Name).IsRequired().HasMaxLength(80); b.Property(x=>x.Description).HasMaxLength(300); b.HasOne<Category>().WithMany().HasForeignKey(x=>x.ParentCategoryId).OnDelete(DeleteBehavior.Restrict); b.HasData(
 new Category{Id=1,Name="Programming",Description="Programming topics",ParentCategoryId=null},
 new Category{Id=2,Name="Backend",Description="Server-side development",ParentCategoryId=1},
 new Category{Id=3,Name="ASP.NET",Description="Microsoft web framework",ParentCategoryId=2},
 new Category{Id=4,Name="Design",Description="Design topics",ParentCategoryId=null},
 new Category{Id=5,Name="UI Design",Description="User interface design",ParentCategoryId=4}); }
}
