// Create Services/ICourseService.cs:

using Microsoft.EntityFrameworkCore;
using Tms.Api.Dtos;
using TmsApi.Domain.Entities;
using Microsoft.Extensions.Logging;
using TmsApi.Infrastructure.Persistence;
using TmsApi.Application.Interfaces;

namespace TmsApi.Infrastructure.Services;

public class CourseService(TmsDbContext context, ILogger<CourseService> logger) : ICourseService
{
    public Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct)
    {
        return context.Courses
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CourseResponseDto(c.Id, c.Code, c.Title, c.MaxCapacity, c.Enrollments.Count))
            .FirstOrDefaultAsync(ct);
    }

    public Task<CourseResponseDto?> GetByCodeAsync(string code, CancellationToken ct)
    {
        return context.Courses
            .AsNoTracking()
            .Where(c => c.Code == code)
            .Select(c => new CourseResponseDto(c.Id, c.Code, c.Title, c.MaxCapacity, c.Enrollments.Count))
            .FirstOrDefaultAsync(ct);
    }
    public async Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct)
    {
        var course = new Course
        {
            Code = request.Code,
            Title = request.Title,
            MaxCapacity = request.MaxCapacity
        };

        context.Courses.Add(course);
        await context.SaveChangesAsync(ct);
        logger.LogInformation("Created course {CourseId} ({Code})", course.Id, course.Code);
        return (await GetByIdAsync(course.Id, ct))!;
    }

    public Task GetByIdAsync(object id, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public Task GetAllAsync(CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public async Task<PagedResponse<CourseResponseDto>> GetPagedAsync(PagedRequest request,
        CancellationToken ct)
    {
        throw new NotImplementedException();

        // public Task GetCoursesAsync(PagedRequest request, CancellationToken ct)
        // {
        //     throw new NotImplementedException();
        // }
    }

    public Task GetCoursesAsync(PagedRequest request, CancellationToken ct)
    {
        throw new NotImplementedException();
    }
}