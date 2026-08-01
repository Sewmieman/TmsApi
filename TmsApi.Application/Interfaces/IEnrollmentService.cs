//--- The contract--
using TmsApi.Application.Dtos;
using TmsApi.Domain.Entities;

namespace TmsApi.Application.Interfaces;

public interface IEnrollmentService
{
    
    Task<EnrollmentRecord> EnrollAsync(string studentId, string courseCode);
    Task<EnrollmentRecord?> GetByIdAsync(string id);
    //Task<EnrollmentRecord?> ExistsAsync(string id);
    Task<IReadOnlyList<EnrollmentRecord>> GetAllAsync();

    Task<bool> DeleteAsync(string id);
    Task<object> GetByStudentIdAsync(object studentI);
    Task<IEnumerable<object>> GetByStudentIdAsync(int studentId, CancellationToken ct);
    Task<bool> ExistsAsync(int studentId, string courseCode, CancellationToken ct);
    Task AddAsync(Enrollment enrollment, CancellationToken ct);
}
//--- The in-memory implementation--
