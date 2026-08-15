using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TmsApi.Infrastructure.Persistence;
namespace TmsApi.Api.Controllers;

[ApiController]
[Route("api/test")]
[Route("api/dashboard")]

public class TestController(TmsDbContext context) : ControllerBase
{
    [HttpGet("deferred")]
    public IActionResult TestDeferred()
    {
        Console.WriteLine("\n>>> STEP 1: Building the query object (nodatabase contact)...");
        var query = context.Students.Where(s => s.GPA >= 3.0m);
        Console.WriteLine(">>> STEP 2: Appending a sorting clause...");
        var orderedQuery = query.OrderBy(s => s.Name);
        Console.WriteLine(">>> STEP 3: Materializing query into a C# List...");
        var results = orderedQuery.ToList(); // Execution is triggered here
        Console.WriteLine(">>> STEP 4: Materialization finished. List populated.\n");

        return Ok(results);
    }
    private static bool IsHonorRoll(decimal gpa)
    {
        return gpa >= 3.5m;
    }
    [HttpGet("translation-fail")]

    public IActionResult TestTranslationFail()
    {
        Console.WriteLine("\n>>> STEP 1: Running non-translatable query...");
        try
        {
            var students = context.Students
                .Where(s => IsHonorRoll(s.GPA))
                .ToList();

            return Ok(students);
        }
        catch (Exception ex)
        {
            Console.WriteLine($">>> EXCEPTION CAUGHT: {ex.Message}\n");
            return BadRequest(new { ex.Message });
        }
    }

    [HttpGet("students")]
    public async Task<IActionResult> GetStudents(
        int page = 1,
        CancellationToken cancellationToken = default)
    {
        const int pageSize = 2;

        // if (page < 1)
        //     page = 1;


        var students = await context.Students
            .OrderBy(s => s.Name)              // IMPORTANT: stable sorting first
            .Skip((page - 1) * pageSize)       // skip previous pages
            .Take(pageSize)                    // take current page
            .ToListAsync(cancellationToken);

        return Ok(students);

    }

    [HttpGet("top-courses")]
    public async Task<IActionResult> GetTopCourses(CancellationToken cancellationToken = default)
    {
        var topCourses = await context.Enrollments
            .GroupBy(e => e.Course.Title)
            .Select(g => new
            {
                CourseName = g.Key,
                EnrollmentCount = g.Count()
            })
            .OrderByDescending(x => x.EnrollmentCount)
            .Take(5)
            .ToListAsync(cancellationToken);

        return Ok(topCourses);
    }


    [HttpGet("n-plus-anti-pattern")]
    public async Task<IActionResult> GetNPlusBug(CancellationToken cancellationToken = default)
    {
        var students = await context.Students.ToListAsync();

        foreach (var student in students)
        {
            Console.WriteLine(student.Enrollments.Count);
        }


        return Ok(students);
    }
public async Task GenerateStudentReport(CancellationToken cancellationToken)
{
    var report = await context.Students
        .AsNoTracking()
        .Select(s => new
        {
            s.Name,
            EnrollmentCount = s.Enrollments.Count
        })
        .ToListAsync(cancellationToken);

    foreach (var r in report)
    {
        Console.WriteLine( $"{r.Name}: {r.EnrollmentCount} enrollments");
    }
    var students = await context.Students
      .AsNoTracking()
      .Include(s => s.Enrollments)
      .ToListAsync(cancellationToken);
foreach (var s in students)
Console.WriteLine($"{s.Name}: {s.Enrollments.Count} enrollments");
}
}