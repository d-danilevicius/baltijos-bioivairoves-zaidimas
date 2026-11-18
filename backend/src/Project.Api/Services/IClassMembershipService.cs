using Project.Api.DTOs.Requests;

namespace Project.Api.Services;

public interface IClassMembershipService
{
    Task<JoinClassResult> JoinAsync(string studentId, JoinClassRequest request, CancellationToken cancellationToken);
}
