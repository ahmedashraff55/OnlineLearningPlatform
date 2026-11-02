using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using OnlineLearningPlatform.Data;
using OnlineLearningPlatform.Models;

namespace OnlineLearningPlatform;

internal class Program
{
    private static int _newStudentId;

    private static async Task Main()
    {
        var configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(configuration.GetConnectionString("DefaultConnection")).Options;

        await using var db = new AppDbContext(options);
        try
        {
            await db.Database.MigrateAsync();
            await SeedStudentsAndEnrollments(db);
            Console.WriteLine("\n========== CREATE ==========");
            await AddInstructor(db);
            await AddCourse(db);
            await AddSeveralStudents(db);
            await EnrollStudent(db);
            Console.WriteLine("\n========== READ ==========");
            await FindCourse(db);
            await FilterCourses(db, 30m, 120m);
            await PrintSubCategories(db, 1);
            await PrintCourseRoster(db, 1);
            Console.WriteLine("\n========== UPDATE ==========");
            await ChangeCoursePrice(db, 1, 55m);
            await UpdateProgress(db, _newStudentId, 1, 100m);
            await PublishCourse(db, 5);
            Console.WriteLine("\n========== DELETE ==========");
            await UnenrollStudent(db, _newStudentId, 1);
            await TryDeleteParentCategory(db, 1);
            await SafeDeleteCategory(db, 4);
            Console.WriteLine("\n========== BULK OPERATIONS ==========");
            await BulkRaisePrices(db, CourseLevel.Beginner);
            await BulkDeleteDroppedEnrollments(db);
            Console.WriteLine("\n========== DELETE COURSE DEMO ==========");
            await DeleteCourseWithEnrollments(db, 4);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Application error: {ex.GetBaseException().Message}");
        }
        Console.WriteLine("\nFinished. Press any key to exit.");
        Console.ReadKey();
    }

    // Runtime seed: HasData is used for instructors, categories and courses in their configuration classes.
    private static async Task SeedStudentsAndEnrollments(AppDbContext db)
    {
        if (!await db.Students.AnyAsync())
        {
            var students = new List<Student>
            {
                new(){FullName="Ahmed Ali",Email="ahmed@student.com",DateOfBirth=new DateTime(2003,5,12)},
                new(){FullName="Mona Hassan",Email="mona@student.com",DateOfBirth=new DateTime(2002,8,20)},
                new(){FullName="Omar Samy",Email="omar@student.com",DateOfBirth=new DateTime(2004,1,3)},
                new(){FullName="Laila Adel",Email="laila@student.com",DateOfBirth=new DateTime(2003,11,9)},
                new(){FullName="Youssef Mostafa",Email="youssef@student.com",DateOfBirth=new DateTime(2001,4,17)}
            };
            await db.Students.AddRangeAsync(students);
            await db.SaveChangesAsync();
        }
        if (!await db.Enrollments.AnyAsync())
        {
            var ids = await db.Students.OrderBy(s=>s.Id).Select(s=>s.Id).Take(5).ToListAsync();
            var enrollments = new List<Enrollment>
            {
                new(){StudentId=ids[0],CourseId=1,EnrolledAt=new DateTime(2024,5,1),Progress=35,Status=EnrollmentStatus.Active},
                new(){StudentId=ids[1],CourseId=1,EnrolledAt=new DateTime(2024,5,2),Progress=100,Status=EnrollmentStatus.Completed,FinalGrade=95},
                new(){StudentId=ids[2],CourseId=2,EnrolledAt=new DateTime(2024,5,3),Progress=10,Status=EnrollmentStatus.Active},
                new(){StudentId=ids[3],CourseId=2,EnrolledAt=new DateTime(2024,5,4),Progress=100,Status=EnrollmentStatus.Completed,FinalGrade=88},
                new(){StudentId=ids[4],CourseId=3,EnrolledAt=new DateTime(2024,5,5),Progress=20,Status=EnrollmentStatus.Dropped},
                new(){StudentId=ids[0],CourseId=4,EnrolledAt=new DateTime(2024,5,6),Progress=0,Status=EnrollmentStatus.Active},
                new(){StudentId=ids[1],CourseId=3,EnrolledAt=new DateTime(2024,5,7),Progress=0,Status=EnrollmentStatus.Dropped},
                new(){StudentId=ids[2],CourseId=4,EnrolledAt=new DateTime(2024,5,8),Progress=100,Status=EnrollmentStatus.Completed,FinalGrade=91}
            };
            await db.Enrollments.AddRangeAsync(enrollments);
            await db.SaveChangesAsync();
        }
    }

    private static async Task AddInstructor(AppDbContext db)
    {
        try { var item=new Instructor{FullName="New Instructor",Email=$"new{DateTime.Now.Ticks}@learn.com",Bio="New instructor",JoinedAt=DateTime.Today}; db.Instructors.Add(item); await db.SaveChangesAsync(); Console.WriteLine($"Instructor added. Generated Id: {item.Id}"); }
        catch(DbUpdateException ex){Console.WriteLine($"Could not add instructor: {ex.GetBaseException().Message}");}
    }

    private static async Task AddCourse(AppDbContext db)
    {
        const int instructorId=1, categoryId=2;
        if(!await db.Instructors.AnyAsync(x=>x.Id==instructorId)){Console.WriteLine("Course not added: instructor does not exist.");return;}
        if(!await db.Categories.AnyAsync(x=>x.Id==categoryId)){Console.WriteLine("Course not added: category does not exist.");return;}
        var course=new Course{Title="EF Core Practice Course",Description="Added from Program.cs",Price=65,Level=CourseLevel.Intermediate,IsPublished=false,PublishedAt=null,InstructorId=instructorId,CategoryId=categoryId};
        db.Courses.Add(course); await db.SaveChangesAsync(); Console.WriteLine($"Course added. Id: {course.Id}");
    }

    private static async Task AddSeveralStudents(AppDbContext db)
    {
        var students=new[]{new Student{FullName="Student Added One",Email="added1@student.com",DateOfBirth=new DateTime(2003,2,1)},new Student{FullName="Student Added Two",Email="added2@student.com",DateOfBirth=new DateTime(2002,6,1)},new Student{FullName="Student Added Three",Email="added3@student.com",DateOfBirth=new DateTime(2004,9,1)}};
        try { await db.Students.AddRangeAsync(students); await db.SaveChangesAsync(); _newStudentId=students[0].Id; Console.WriteLine("Three students added. Generated IDs: "+string.Join(", ",students.Select(s=>s.Id))); }
        catch(DbUpdateException ex){Console.WriteLine($"Could not add students: {ex.GetBaseException().Message}");}
    }

    private static async Task EnrollStudent(AppDbContext db)
    {
        int studentId=_newStudentId, courseId=1;
        if(studentId==0||!await db.Students.AnyAsync(s=>s.Id==studentId)){Console.WriteLine("Enrollment refused: student does not exist.");return;}
        var course=await db.Courses.FindAsync(courseId);
        if(course is null){Console.WriteLine("Enrollment refused: course does not exist.");return;}
        if(!course.IsPublished){Console.WriteLine("Enrollment refused: course is not published.");return;}
        if(await db.Enrollments.AnyAsync(e=>e.StudentId==studentId&&e.CourseId==courseId)){Console.WriteLine("Enrollment refused: student is already enrolled.");return;}
        db.Enrollments.Add(new Enrollment{StudentId=studentId,CourseId=courseId,EnrolledAt=DateTime.Now,Progress=0,Status=EnrollmentStatus.Active});
        await db.SaveChangesAsync(); Console.WriteLine("Student enrolled successfully.");
    }

    private static async Task FindCourse(AppDbContext db)
    {
        var byId=await db.Courses.FindAsync(1); Console.WriteLine(byId is null?"Course Id 1: not found":$"Found by Id: {byId.Title}");
        var byTitle=await db.Courses.FirstOrDefaultAsync(c=>c.Title=="C# Fundamentals"); Console.WriteLine(byTitle is null?"Course title: not found":$"Found by title: {byTitle.Title}");
    }

    private static async Task FilterCourses(AppDbContext db,decimal minPrice,decimal maxPrice)
    {
        var courses=await db.Courses.Where(c=>c.Price>=minPrice&&c.Price<=maxPrice).OrderByDescending(c=>c.Price).Select(c=>new{c.Title,c.Level,c.Price}).ToListAsync();
        Console.WriteLine($"Courses priced from {minPrice} to {maxPrice}:"); foreach(var c in courses) Console.WriteLine($"{c.Title} | {c.Level} | {c.Price:C}");
    }

    private static async Task PrintSubCategories(AppDbContext db,int categoryId)
    {
        var category=await db.Categories.FindAsync(categoryId); if(category is null){Console.WriteLine("Category not found.");return;}
        var children=await db.Categories.Where(c=>c.ParentCategoryId==categoryId).Select(c=>c.Name).ToListAsync();
        Console.WriteLine($"Direct sub-categories of {category.Name}: {(children.Count==0?"None":string.Join(", ",children))}");
        var roots=await db.Categories.Where(c=>c.ParentCategoryId==null).Select(c=>c.Name).ToListAsync(); Console.WriteLine("Top-level categories: "+string.Join(", ",roots));
    }

    private static async Task PrintCourseRoster(AppDbContext db,int courseId)
    {
        var studentIds=await db.Enrollments.Where(e=>e.CourseId==courseId).Select(e=>e.StudentId).ToListAsync();
        var names=await db.Students.Where(s=>studentIds.Contains(s.Id)).Select(s=>s.FullName).ToListAsync();
        Console.WriteLine($"Course {courseId} has {studentIds.Count} enrolled student(s)."); foreach(var name in names) Console.WriteLine(" - "+name);
    }

    private static async Task ChangeCoursePrice(AppDbContext db,int courseId,decimal newPrice)
    {
        var course=await db.Courses.FindAsync(courseId); if(course is null){Console.WriteLine("Price not changed: course not found.");return;}
        if(newPrice<0){Console.WriteLine("Price must not be negative.");return;} course.Price=newPrice; await db.SaveChangesAsync(); Console.WriteLine($"Price of {course.Title} changed to {newPrice:C}.");
    }

    private static async Task UpdateProgress(AppDbContext db,int studentId,int courseId,decimal newProgress)
    {
        if(newProgress<0||newProgress>100){Console.WriteLine("Progress rejected: value must be between 0 and 100.");return;}
        var enrollment=await db.Enrollments.FindAsync(studentId,courseId); if(enrollment is null){Console.WriteLine("Enrollment not found.");return;}
        enrollment.Progress=newProgress; if(newProgress==100){enrollment.Status=EnrollmentStatus.Completed; enrollment.FinalGrade??=100m;}
        await db.SaveChangesAsync(); Console.WriteLine($"Progress updated to {newProgress}% ({enrollment.Status}).");
    }

    private static async Task PublishCourse(AppDbContext db,int courseId)
    {
        var course=await db.Courses.FindAsync(courseId); if(course is null){Console.WriteLine("Course not found; cannot publish.");return;}
        course.IsPublished=true; course.PublishedAt??=DateTime.Now; await db.SaveChangesAsync(); Console.WriteLine($"'{course.Title}' published at {course.PublishedAt}.");
    }

    private static async Task UnenrollStudent(AppDbContext db,int studentId,int courseId)
    {
        var enrollment=await db.Enrollments.FindAsync(studentId,courseId); if(enrollment is null){Console.WriteLine("Enrollment not found; nothing to delete.");return;}
        db.Enrollments.Remove(enrollment); await db.SaveChangesAsync(); Console.WriteLine("Enrollment deleted.");
    }

    private static async Task TryDeleteParentCategory(AppDbContext db,int categoryId)
    {
        var category=await db.Categories.FindAsync(categoryId); if(category is null){Console.WriteLine("Category not found.");return;}
        db.Categories.Remove(category);
        try{await db.SaveChangesAsync();Console.WriteLine("Category deleted.");}
        catch(DbUpdateException ex){db.Entry(category).State=EntityState.Unchanged;Console.WriteLine("Delete blocked because the category has sub-categories (Restrict): "+ex.GetBaseException().Message);}
    }

    private static async Task SafeDeleteCategory(AppDbContext db,int categoryId)
    {
        var category=await db.Categories.FindAsync(categoryId); if(category is null){Console.WriteLine("Category not found.");return;}
        if(await db.Categories.AnyAsync(c=>c.ParentCategoryId==categoryId)){Console.WriteLine($"Safe delete refused: '{category.Name}' still has sub-categories.");return;}
        if(await db.Courses.AnyAsync(c=>c.CategoryId==categoryId)){Console.WriteLine($"Safe delete refused: '{category.Name}' is assigned to courses.");return;}
        db.Categories.Remove(category); await db.SaveChangesAsync(); Console.WriteLine("Category safely deleted.");
    }

    private static async Task BulkRaisePrices(AppDbContext db,CourseLevel level)
    {
        var affected=await db.Courses.Where(c=>c.IsPublished&&c.Level==level).ExecuteUpdateAsync(s=>s.SetProperty(c=>c.Price,c=>c.Price*1.10m));
        Console.WriteLine($"Bulk price raise ({level}): {affected} row(s) affected.");
    }

    private static async Task BulkDeleteDroppedEnrollments(AppDbContext db)
    {
        var affected=await db.Enrollments.Where(e=>e.Status==EnrollmentStatus.Dropped).ExecuteDeleteAsync(); Console.WriteLine($"Bulk delete: {affected} dropped enrollment(s) deleted.");
    }

    private static async Task DeleteCourseWithEnrollments(AppDbContext db,int courseId)
    {
        var course=await db.Courses.FindAsync(courseId); if(course is null){Console.WriteLine("Course not found.");return;}
        var before=await db.Enrollments.CountAsync(e=>e.CourseId==courseId); Console.WriteLine($"Enrollments before deleting course: {before}");
        // The Course -> Enrollment FK uses Cascade, so SQL Server automatically removes dependent enrollments with the course.
        db.Courses.Remove(course); await db.SaveChangesAsync();
        var after=await db.Enrollments.CountAsync(e=>e.CourseId==courseId); Console.WriteLine($"Enrollments after deleting course: {after}");
    }
}
