
using Tms.Api.Dtos;
namespace TmsApi.Application.Interfaces;

public interface ICourseService
{
Task<CourseResponseDto?> GetByIdAsync(int id, CancellationToken ct);
Task<CourseResponseDto?> GetByCodeAsync(string code, CancellationToken ct);
Task<CourseResponseDto> CreateAsync(CreateCourseRequest request, CancellationToken ct);
    Task GetByIdAsync(object id, CancellationToken ct);
    Task GetCoursesAsync(PagedRequest request, CancellationToken ct);
    Task GetAllAsync(CancellationToken ct);
    Task<PagedResponse<CourseResponseDto>> GetPagedAsync(PagedRequest request, CancellationToken ct);
    
}
