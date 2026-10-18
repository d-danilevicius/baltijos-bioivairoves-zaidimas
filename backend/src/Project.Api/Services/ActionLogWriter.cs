using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Project.Api.Data;
using Project.Api.Entities;

namespace Project.Api.Services;

public interface IActionLogWriter
{
    Task WriteAsync(string? userId, string action, DateTime occurredAtUtc, CancellationToken cancellationToken);
}

public class ActionLogWriter : IActionLogWriter
{
    private readonly ApplicationDbContext _db;
    private readonly ILogger<ActionLogWriter> _logger;

    public ActionLogWriter(ApplicationDbContext db, ILogger<ActionLogWriter> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task WriteAsync(string? userId, string action, DateTime occurredAtUtc, CancellationToken cancellationToken)
    {
        var entry = new ActionLog
        {
            UserId = userId,
            Action = action,
            OccurredAtUtc = occurredAtUtc
        };

        _db.ActionLogs.Add(entry);
        await _db.SaveChangesAsync(cancellationToken);

        _logger.LogInformation("ActionLog recorded: {Action} for UserId={UserId}", action, userId ?? "<null>");
    }
}
