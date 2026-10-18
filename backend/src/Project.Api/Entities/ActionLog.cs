using System;

namespace Project.Api.Entities;

public class ActionLog
{
    public long Id { get; set; }

    // UTC timestamp of when the action occurred
    public DateTime OccurredAtUtc { get; set; }

    // Nullable user id; if the user is deleted the FK is set to null
    public string? UserId { get; set; }

    public string Action { get; set; } = null!;
}
