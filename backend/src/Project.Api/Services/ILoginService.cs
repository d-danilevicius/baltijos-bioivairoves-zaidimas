using System.Threading;
using System.Threading.Tasks;
using Project.Api.DTOs.Requests;

namespace Project.Api.Services;

public interface ILoginService
{
    Task<LoginResult> LoginAsync(LoginRequest request, CancellationToken cancellationToken);

    Task LogoutAsync(string userId, CancellationToken cancellationToken);
}
