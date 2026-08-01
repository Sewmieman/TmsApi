using Tms.Api.Dtos;

namespace TmsApi. Application.Interfaces;

public
    interface ICachedCourseService

{
    Task< CourseResponseDto ?>GetByCodeAsync( string code, CancellationToken ct);
}