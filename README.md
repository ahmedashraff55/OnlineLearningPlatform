# Online Learning Platform — EF Core Assignment 3

A .NET 8 Console Application using EF Core 8 and SQL Server. The project contains the entities, enums, separate Fluent API configuration classes, two migrations, seed data, and the requested CRUD/query/bulk operations.

## Requirements
- Visual Studio 2022 (or newer) with the .NET 8 SDK
- SQL Server LocalDB (installed with the Visual Studio data-storage workload) or a SQL Server instance

## Open and run
1. Extract the ZIP file.
2. Open `OnlineLearningPlatform.csproj` in Visual Studio.
3. Restore NuGet packages.
4. Check `appsettings.json`. The default connection uses SQL Server LocalDB:
   `Server=(localdb)\MSSQLLocalDB;Database=OnlineLearningPlatformDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True`
5. If your SQL Server instance is different, replace the connection string.
6. Run with **Ctrl + F5**. The application applies both migrations automatically, then seeds students/enrollments only when their tables are empty, and runs each assignment operation in order.

## Migrations
Two migrations are included:
- `202611020001_InitialCreate`: creates the schema and relationships.
- `202611020002_SeedData`: inserts the 3 instructors, 5 categories and 6 courses configured with `HasData`.

To manage migrations from Package Manager Console:
```powershell
Update-Database
```
Or with the .NET CLI:
```bash
dotnet ef database update
```

The application also calls `Database.MigrateAsync()` on startup, so a separate update command is not required for the normal run.

## Important implementation notes
- `Enrollment` uses the composite primary key `(StudentId, CourseId)`.
- Category parent-child deletion uses `DeleteBehavior.Restrict`.
- Course-to-enrollment deletion uses cascade; the course-delete method prints enrollment counts before and after and comments on why dependents disappear.
- The assignment asks for the progress range 0–100. The update method rejects values outside this range and marks progress 100 as `Completed`.
- EF Core `ExecuteUpdateAsync` and `ExecuteDeleteAsync` implement the two single-statement bulk operations.
- No `Include` or lazy loading is used.

## Notes
The database seed for students and enrollments is done in `Program.cs`, guarded by `Any()` checks. It creates five students and eight enrollments spanning Active, Completed and Dropped. The three students inserted in the Create section are separate test records, as required by that operation.
