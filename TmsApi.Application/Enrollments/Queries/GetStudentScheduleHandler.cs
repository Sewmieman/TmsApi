using MediatR;
using TmsApi.Application.Interfaces;
namespace TmsApi.Application.Enrollments.Queries;public class GetStudentScheduleHandler(IEnrollmentService repo)
: IRequestHandler<GetStudentScheduleQuery, ScheduleDto>
{
public async Task<ScheduleDto> Handle(
GetStudentScheduleQuery query, CancellationToken ct)
{
var enrollments = await repo.GetByStudentIdAsync(query.StudentId, ct);
var items = enrollments
    .Select(e =>
    {
        dynamic enrollment = e;
        return new ScheduleItemDto(
            enrollment.Course.Code,
            enrollment.Course.Title,
            "TBD");
    })
    .ToList();
return new ScheduleDto(query.StudentId, items);
}
}