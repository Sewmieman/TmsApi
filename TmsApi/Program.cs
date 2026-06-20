
using Scalar.AspNetCore;
using TmsApi.Entities;
using Microsoft.EntityFrameworkCore;
using TmsApi.Data;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddAuthentication();
builder.Services.AddAuthorization();
builder.Services.AddOpenApi();
builder.Services.AddSingleton<EnrollmentWorker>();
builder.Services.AddScoped<IEnrollmentService, EnrollmentService>();
// Add services to the container.
builder.Services.AddDbContext<TmsDbContext>(options =>
options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase"))
.LogTo(Console.WriteLine, LogLevel.Information) // Log SQL to output window
.EnableSensitiveDataLogging()); // Show parameters in querylogs (dev only)
// Register TmsDbContext scoped for incoming HTTP requests
// builder.Services.AddDbContext<TmsDbContext>(options =>
// options.UseNpgsql(builder.Configuration.GetConnectionString("TmsDatabase")));

var app = builder.Build();

builder.Host.UseDefaultServiceProvider(options =>
{
options.ValidateScopes = true;
options.ValidateOnBuild = true;
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}
using (var scope = app.Services.CreateScope())
{
var context = scope.ServiceProvider.GetRequiredService<TmsDbContext>();
context.Database.Migrate(); // Applies any pending migrations; keeps migration history intact
if (!context.Students.Any())
{
var students = new List<Student>
{
new() { RegistrationNumber = "TMS-2026-0001", Name = "Alice Smith", GPA = 3.8m, IsActive = true },
    new() { RegistrationNumber = "TMS-2026-0002", Name = "Bob Jones", GPA = 2.9m, IsActive = true },
new() { RegistrationNumber = "TMS-2026-0003", Name = "Charlie Brown", GPA = 3.4m, IsActive = false },
new() { RegistrationNumber = "TMS-2026-0004", Name = "Diana Prince", GPA = 3.9m, IsActive = true },
new() { RegistrationNumber = "TMS-2026-0005", Name = "Evan Wright", GPA = 2.5m, IsActive = true }
};
context.Students.AddRange(students);
var courses = new List<Course>
{
new() { Code = "CS-101", Title = "Introduction to ComputerScience", Capacity = 30 },
new() { Code = "CS-201", Title = "Data Structures and Algorithms", Capacity = 25 },
new() { Code = "MAT-101", Title = "Calculus I", Capacity =40 }
};
context.Courses.AddRange(courses);
context.SaveChanges();
var enrollments = new List<Enrollment>
{
new() { StudentId = students[0].Id, CourseId = courses[0].Id, Grade = 4.0m },
new() { StudentId = students[0].Id, CourseId = courses[1].Id, Grade = 3.6m },
new() { StudentId = students[1].Id, CourseId = courses[0].Id, Grade = 2.8m },
new() { StudentId = students[3].Id, CourseId = courses[1].Id, Grade = 3.9m }
};
var count = await context.Students
.Where(s =>  s.GPA >= 3.0m)
.CountAsync();
var list = await context.Courses
.Select(c => new
{
c.Title,
EnrollmentCount = c.Enrollments.Count
})
.OrderByDescending(x => x.EnrollmentCount)
.ToListAsync();

context.Enrollments.AddRange(enrollments);
context.SaveChanges();
}
}

app.UseRouting();
app.UseAuthentication();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
