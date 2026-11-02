using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;
using OnlineLearningPlatform.Data;
#nullable disable
namespace OnlineLearningPlatform.Migrations;

[DbContext(typeof(AppDbContext))]
[Migration("202611020002_SeedData")]
public partial class SeedData : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.InsertData("Instructors", new[] { "Id", "Bio", "Email", "FullName", "JoinedAt" }, new object[,] {
            {1,"Software engineer and instructor.","john@learn.com","John Smith",new DateTime(2023,1,10)},
            {2,"Backend development instructor.","sarah@learn.com","Sarah Johnson",new DateTime(2023,3,15)},
            {3,"Design and programming instructor.","michael@learn.com","Michael Brown",new DateTime(2024,2,1)} });
        migrationBuilder.InsertData("Categories", new[] { "Id", "Description", "Name", "ParentCategoryId" }, new object[,] {
            {1,"Programming topics","Programming",null}, {2,"Server-side development","Backend",1},
            {3,"Microsoft web framework","ASP.NET",2}, {4,"Design topics","Design",null}, {5,"User interface design","UI Design",4} });
        migrationBuilder.InsertData("Courses", new[] { "Id", "CategoryId", "Description", "InstructorId", "IsPublished", "Level", "Price", "PublishedAt", "Title" }, new object[,] {
            {1,1,"Learn C# basics",1,true,0,49.99m,new DateTime(2024,1,10),"C# Fundamentals"},
            {2,3,"Build APIs with ASP.NET Core",2,true,1,89.99m,new DateTime(2024,2,10),"ASP.NET Core Web API"},
            {3,2,"Advanced database access",2,true,2,119.50m,new DateTime(2024,3,5),"Advanced EF Core"},
            {4,1,"Web foundations",3,true,0,39.00m,new DateTime(2024,4,1),"HTML and CSS"},
            {5,2,"Architecture patterns",1,false,2,99.00m,null,"Clean Architecture"},
            {6,5,"Design user interfaces",3,false,1,59.00m,null,"UI Design Basics"} });
    }
    protected override void BuildTargetModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "8.0.11");
        modelBuilder.ApplyConfiguration(new OnlineLearningPlatform.Data.Configurations.InstructorConfiguration());
        modelBuilder.ApplyConfiguration(new OnlineLearningPlatform.Data.Configurations.CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new OnlineLearningPlatform.Data.Configurations.CourseConfiguration());
        modelBuilder.ApplyConfiguration(new OnlineLearningPlatform.Data.Configurations.StudentConfiguration());
        modelBuilder.ApplyConfiguration(new OnlineLearningPlatform.Data.Configurations.EnrollmentConfiguration());
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DeleteData("Courses", "Id", 1); migrationBuilder.DeleteData("Courses", "Id", 2); migrationBuilder.DeleteData("Courses", "Id", 3); migrationBuilder.DeleteData("Courses", "Id", 4); migrationBuilder.DeleteData("Courses", "Id", 5); migrationBuilder.DeleteData("Courses", "Id", 6);
        migrationBuilder.DeleteData("Categories", "Id", 1); migrationBuilder.DeleteData("Categories", "Id", 2); migrationBuilder.DeleteData("Categories", "Id", 3); migrationBuilder.DeleteData("Categories", "Id", 4); migrationBuilder.DeleteData("Categories", "Id", 5);
        migrationBuilder.DeleteData("Instructors", "Id", 1); migrationBuilder.DeleteData("Instructors", "Id", 2); migrationBuilder.DeleteData("Instructors", "Id", 3);
    }
}
