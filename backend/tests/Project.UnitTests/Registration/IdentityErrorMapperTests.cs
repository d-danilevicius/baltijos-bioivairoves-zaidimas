using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.Registration;

public class IdentityErrorMapperTests
{
    private class CaptureLogger : ILogger
    {
        public List<string> Messages { get; } = new List<string>();
        public IDisposable BeginScope<TState>(TState state) where TState : notnull => null!;
        public bool IsEnabled(LogLevel logLevel) => true;
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            Messages.Add(formatter(state, exception));
        }
    }

    [Fact]
    public void Map_PasswordTooShort_ReturnsInvalidPasswordInvalidLength()
    {
        var errors = new[] { new IdentityError { Code = "PasswordTooShort" } };
        var logger = new CaptureLogger();

        var result = IdentityErrorMapper.Map(errors, logger);

        Assert.False(result.Success);
        Assert.False(result.IsConflict);
        Assert.NotNull(result.Errors);
        Assert.True(result.Errors.ContainsKey("password") && Array.Exists(result.Errors["password"], s => s == "InvalidLength"));
    }

    [Fact]
    public void Map_InvalidUserName_ReturnsInvalidUserNameInvalidFormat()
    {
        var errors = new[] { new IdentityError { Code = "InvalidUserName" } };
        var logger = new CaptureLogger();

        var result = IdentityErrorMapper.Map(errors, logger);

        Assert.False(result.Success);
        Assert.NotNull(result.Errors);
        Assert.True(result.Errors.ContainsKey("userName") && Array.Exists(result.Errors["userName"], s => s == "InvalidFormat"));
    }

    [Fact]
    public void Map_InvalidEmail_ReturnsInvalidEmailInvalidFormat()
    {
        var errors = new[] { new IdentityError { Code = "InvalidEmail" } };
        var logger = new CaptureLogger();

        var result = IdentityErrorMapper.Map(errors, logger);

        Assert.False(result.Success);
        Assert.NotNull(result.Errors);
        Assert.True(result.Errors.ContainsKey("email") && Array.Exists(result.Errors["email"], s => s == "InvalidFormat"));
    }

    [Fact]
    public void Map_DuplicateEmail_ReturnsConflictEmailAlreadyRegistered()
    {
        var errors = new[] { new IdentityError { Code = "DuplicateEmail" } };
        var logger = new CaptureLogger();

        var result = IdentityErrorMapper.Map(errors, logger);

        Assert.False(result.Success);
        Assert.True(result.IsConflict);
        Assert.NotNull(result.Errors);
        Assert.True(result.Errors.ContainsKey("email") && Array.Exists(result.Errors["email"], s => s == "AlreadyRegistered"));
    }

    [Fact]
    public void Map_DuplicateUserName_ReturnsConflictUserNameAlreadyInUse()
    {
        var errors = new[] { new IdentityError { Code = "DuplicateUserName" } };
        var logger = new CaptureLogger();

        var result = IdentityErrorMapper.Map(errors, logger);

        Assert.False(result.Success);
        Assert.True(result.IsConflict);
        Assert.NotNull(result.Errors);
        Assert.True(result.Errors.ContainsKey("userName") && Array.Exists(result.Errors["userName"], s => s == "AlreadyInUse"));
    }

    [Fact]
    public void Map_UnknownCode_LogsAndThrows()
    {
        var errors = new[] { new IdentityError { Code = "UnknownCodeXYZ" } };
        var logger = new CaptureLogger();

        Assert.Throws<InvalidOperationException>(() => IdentityErrorMapper.Map(errors, logger));
        Assert.Contains("UnknownCodeXYZ", logger.Messages[0]);
    }
}
