using System;

namespace Project.IntegrationTests.Login;

public class TestTimeProvider : System.TimeProvider
{
    private DateTimeOffset _now = DateTimeOffset.UtcNow;
    public override DateTimeOffset GetUtcNow() => _now;
    public void Advance(TimeSpan span) => _now = _now.Add(span);
}
