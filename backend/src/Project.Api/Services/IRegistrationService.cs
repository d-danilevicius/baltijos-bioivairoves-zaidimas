using System.Threading.Tasks;
using Project.Api.DTOs.Requests;

namespace Project.Api.Services;

public interface IRegistrationService
{
    Task<RegistrationResult> RegisterAsync(RegisterRequest request);
}
