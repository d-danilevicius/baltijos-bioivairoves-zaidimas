using System;
using System.Threading;
using System.Threading.Tasks;

namespace Project.Api.Services;

public interface IActionLogWriter
{
    Task WriteAsync(string? userId, string action, DateTime occurredAtUtc, CancellationToken cancellationToken);
}
