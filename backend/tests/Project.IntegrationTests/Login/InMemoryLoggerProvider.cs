using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Project.IntegrationTests.Login;

public class InMemoryLoggerProvider : ILoggerProvider
{
    private readonly List<string> _logs;
    public InMemoryLoggerProvider(List<string> logs) => _logs = logs;
    public ILogger CreateLogger(string categoryName) => new InMemoryLogger(_logs);
    public void Dispose() { }

    private class InMemoryLogger : ILogger
    {
        private readonly List<string> _logs;
        public InMemoryLogger(List<string> logs) => _logs = logs;
        public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            try
            {
                var message = formatter(state, exception);
                lock (_logs)
                {
                    _logs.Add(message);
                }
            }
            catch { }
        }
    }
}
