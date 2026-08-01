// Create Services/ICourseService.cs:

using TmsApi.Application.Dtos;
using TmsApi.Application.Interfaces;
using Microsoft.Extensions.Logging;
using TmsApi.Domain.Entities;
using TmsApi.Infrastructure.Persistence;

namespace TmsApi.Infrastructure.Services;

public class EnrollmentService(TmsDbContext context, ILogger<CourseService> logger) : IEnrollmentService
{
    public async Task AddAsync(Enrollment enrollment, CancellationToken ct)
    {
        await context.Enrollments.AddAsync(enrollment, ct);
        await context.SaveChangesAsync(ct);
    }


    public Task<object> GetByStudentIdAsync(object studentI)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<object>> GetByStudentIdAsync(int studentId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    // public Task<bool> ExistsAsync(int studentId, string courseCode, CancellationToken ct)
    // {
    //     throw new NotImplementedException();
    // }

    Task<EnrollmentRecord> IEnrollmentService.EnrollAsync(string studentId, string courseCode)
    {
        throw new NotImplementedException();
    }

    Task<EnrollmentRecord?> IEnrollmentService.GetByIdAsync(string id)
    {
        throw new NotImplementedException();
    }

    // Task<EnrollmentRecord?> IEnrollmentService.ExistsAsync(string id)
    // {
    //     throw new NotImplementedException();
    // }

    Task<IReadOnlyList<EnrollmentRecord>> IEnrollmentService.GetAllAsync()
    {
        throw new NotImplementedException();
    }

    Task<bool> IEnrollmentService.DeleteAsync(string id)
    {
        throw new NotImplementedException();
    }

    Task<object> IEnrollmentService.GetByStudentIdAsync(object studentI)
    {
        throw new NotImplementedException();
    }

    Task<IEnumerable<object>> IEnrollmentService.GetByStudentIdAsync(int studentId, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    Task<bool> IEnrollmentService.ExistsAsync(int studentId, string courseCode, CancellationToken ct)
    {
        throw new NotImplementedException();
    }




}