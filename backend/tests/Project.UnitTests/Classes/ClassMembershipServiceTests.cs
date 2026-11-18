using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.DTOs.Requests;
using Project.Api.Entities;
using Project.Api.Services;
using Project.UnitTests.Registration;

namespace Project.UnitTests.Classes;

public class ClassMembershipServiceTests
{
    private static ClassMembershipService CreateService(ApplicationDbContext db)
    {
        return new ClassMembershipService(db, TimeProvider.System);
    }

    private static async Task AddClassAsync(ApplicationDbContext db, string name, string code)
    {
        db.Classes.Add(new SchoolClass { Id = Guid.NewGuid(), Name = name, Code = code, OwnerId = "teacher-1" });
        await db.SaveChangesAsync();
    }

    private static JoinClassRequest Request(string code, bool confirmChange = false)
    {
        return new JoinClassRequest { Code = code, ConfirmChange = confirmChange };
    }

    [Fact]
    public async Task JoinAsync_WithExistingCode_StartsActiveMembership()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await AddClassAsync(db, "7a", "AAAAAA");
        var service = CreateService(db);

        var result = await service.JoinAsync("student-1", Request("AAAAAA"), CancellationToken.None);

        Assert.Equal(JoinClassOutcome.Joined, result.Outcome);
        Assert.Equal("7a", result.ClassName);
        var membership = Assert.Single(await db.ClassMemberships.ToListAsync());
        Assert.Equal("student-1", membership.StudentId);
        Assert.Null(membership.EndedAtUtc);
    }

    [Fact]
    public async Task JoinAsync_WithUnknownCode_ReturnsClassNotFoundAndStoresNothing()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await AddClassAsync(db, "7a", "AAAAAA");
        var service = CreateService(db);

        var result = await service.JoinAsync("student-1", Request("BBBBBB"), CancellationToken.None);

        Assert.Equal(JoinClassOutcome.ClassNotFound, result.Outcome);
        Assert.Empty(await db.ClassMemberships.ToListAsync());
    }

    [Fact]
    public async Task JoinAsync_WhenStudentIsAlreadyInThisClass_ReturnsAlreadyMemberAndChangesNothing()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await AddClassAsync(db, "7a", "AAAAAA");
        var service = CreateService(db);
        await service.JoinAsync("student-1", Request("AAAAAA"), CancellationToken.None);

        var result = await service.JoinAsync("student-1", Request("AAAAAA"), CancellationToken.None);

        Assert.Equal(JoinClassOutcome.AlreadyMember, result.Outcome);
        var membership = Assert.Single(await db.ClassMemberships.ToListAsync());
        Assert.Null(membership.EndedAtUtc);
    }

    [Fact]
    public async Task JoinAsync_WhenStudentIsInAnotherClassWithoutConfirmation_ReturnsConfirmationRequiredWithBothNames()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await AddClassAsync(db, "7b", "BBBBBB");
        await AddClassAsync(db, "7a", "AAAAAA");
        var service = CreateService(db);
        await service.JoinAsync("student-1", Request("BBBBBB"), CancellationToken.None);

        var result = await service.JoinAsync("student-1", Request("AAAAAA"), CancellationToken.None);

        Assert.Equal(JoinClassOutcome.ConfirmationRequired, result.Outcome);
        Assert.Equal("7a", result.ClassName);
        Assert.Equal("7b", result.CurrentClassName);
        var membership = Assert.Single(await db.ClassMemberships.ToListAsync());
        Assert.Null(membership.EndedAtUtc);
    }

    [Fact]
    public async Task JoinAsync_WhenStudentIsInAnotherClassWithConfirmation_EndsPreviousMembershipAndStartsNew()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await AddClassAsync(db, "7b", "BBBBBB");
        await AddClassAsync(db, "7a", "AAAAAA");
        var service = CreateService(db);
        await service.JoinAsync("student-1", Request("BBBBBB"), CancellationToken.None);

        var result = await service.JoinAsync("student-1", Request("AAAAAA", confirmChange: true), CancellationToken.None);

        Assert.Equal(JoinClassOutcome.Joined, result.Outcome);
        Assert.Equal("7a", result.ClassName);
        var memberships = await db.ClassMemberships.ToListAsync();
        Assert.Equal(2, memberships.Count);
        Assert.Single(memberships, m => m.EndedAtUtc is null);
        Assert.Single(memberships, m => m.EndedAtUtc is not null);
    }

    [Fact]
    public async Task JoinAsync_WhenTwoStudentsJoinTheSameClass_BothHaveActiveMemberships()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await AddClassAsync(db, "7a", "AAAAAA");
        var service = CreateService(db);

        await service.JoinAsync("student-1", Request("AAAAAA"), CancellationToken.None);
        await service.JoinAsync("student-2", Request("AAAAAA"), CancellationToken.None);

        Assert.Equal(2, await db.ClassMemberships.CountAsync(m => m.EndedAtUtc == null));
    }
}
