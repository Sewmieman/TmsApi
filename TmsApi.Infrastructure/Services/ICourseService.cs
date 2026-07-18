
using Tms.Api.Dtos;
namespace TmsApi.Infrastructure.Services;

public interface ICourseService
{
Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct);
Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct);
    Task GetByIdAsync(object id, CancellationToken ct);
    Task GetCoursesAsync(PagedRequest request, CancellationToken ct);
    Task<PagedResponse<CourseResponseDto>> GetPagedAsync(PagedRequest request, CancellationToken ct);
    
}
