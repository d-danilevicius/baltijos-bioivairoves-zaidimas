using Microsoft.Extensions.DependencyInjection;
using Project.Api.Data;
using Project.Api.DTOs.Requests;
using Project.Api.Entities;
using Project.Api.Services;
using Project.UnitTests.Registration;

namespace Project.UnitTests.Classes;

public class ClassServiceTests
{
    private sealed class SequenceCodeGenerator : IClassCodeGenerator
    {
        private readonly Queue<string> _codes;

        public SequenceCodeGenerator(params string[] codes) => _codes = new Queue<string>(codes);

        public string Generate() => _codes.Dequeue();
    }

    private static ClassService CreateService(ApplicationDbContext db, params string[] codes)
    {
        return new ClassService(db, new SequenceCodeGenerator(codes), TimeProvider.System);
    }

    [Fact]
    public async Task CreateAsync_WithNameWithSpaces_StoresTrimmedName()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var service = CreateService(db, "AAAAAA");

        var response = await service.CreateAsync("teacher-1", new CreateClassRequest { Name = "  7a klasė  " }, CancellationToken.None);

        Assert.Equal("7a klasė", response.Name);
    }

    [Fact]
    public async Task CreateAsync_WithValidRequest_StoresClassForGivenOwnerWithGeneratedCode()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var service = CreateService(db, "AB12CD");

        var response = await service.CreateAsync("teacher-1", new CreateClassRequest { Name = "7a" }, CancellationToken.None);

        var stored = db.Classes.Single(c => c.Id == response.Id);
        Assert.Equal("teacher-1", stored.OwnerId);
        Assert.Equal("AB12CD", stored.Code);
    }

    [Fact]
    public async Task CreateAsync_WhenGeneratedCodeAlreadyExists_UsesAnotherCode()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var service = CreateService(db, "AAAAAA", "AAAAAA", "BBBBBB");
        await service.CreateAsync("teacher-1", new CreateClassRequest { Name = "7a" }, CancellationToken.None);

        var response = await service.CreateAsync("teacher-2", new CreateClassRequest { Name = "8b" }, CancellationToken.None);

        Assert.Equal("BBBBBB", response.Code);
    }

    [Fact]
    public async Task ListAsync_WithClassesOfTwoOwners_ReturnsOnlyClassesOfGivenOwner()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var service = CreateService(db, "AAAAAA", "BBBBBB");
        await service.CreateAsync("teacher-1", new CreateClassRequest { Name = "7a" }, CancellationToken.None);
        await service.CreateAsync("teacher-2", new CreateClassRequest { Name = "8b" }, CancellationToken.None);

        var list = await service.ListAsync("teacher-1", CancellationToken.None);

        var item = Assert.Single(list);
        Assert.Equal("7a", item.Name);
    }

    [Fact]
    public async Task GetAsync_WhenClassBelongsToOwner_ReturnsSucceeded()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var service = CreateService(db, "AAAAAA");
        var created = await service.CreateAsync("teacher-1", new CreateClassRequest { Name = "7a" }, CancellationToken.None);

        var result = await service.GetAsync("teacher-1", created.Id, CancellationToken.None);

        Assert.Equal(ClassOutcome.Succeeded, result.Outcome);
        Assert.Equal("7a", result.Class!.Name);
    }

    [Fact]
    public async Task GetAsync_WhenClassBelongsToAnotherTeacher_ReturnsForbidden()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var service = CreateService(db, "AAAAAA");
        var created = await service.CreateAsync("teacher-1", new CreateClassRequest { Name = "7a" }, CancellationToken.None);

        var result = await service.GetAsync("teacher-2", created.Id, CancellationToken.None);

        Assert.Equal(ClassOutcome.Forbidden, result.Outcome);
        Assert.Null(result.Class);
    }

    [Fact]
    public async Task GetAsync_WhenClassDoesNotExist_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var service = CreateService(db);

        var result = await service.GetAsync("teacher-1", Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(ClassOutcome.NotFound, result.Outcome);
    }

    [Fact]
    public async Task DeleteAsync_WhenClassBelongsToOwner_RemovesClass()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var service = CreateService(db, "AAAAAA");
        var created = await service.CreateAsync("teacher-1", new CreateClassRequest { Name = "7a" }, CancellationToken.None);

        var result = await service.DeleteAsync("teacher-1", created.Id, CancellationToken.None);

        Assert.Equal(ClassOutcome.Succeeded, result.Outcome);
        Assert.Empty(db.Classes);
    }

    [Fact]
    public async Task DeleteAsync_WhenClassBelongsToAnotherTeacher_ReturnsForbiddenAndKeepsClass()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var service = CreateService(db, "AAAAAA");
        var created = await service.CreateAsync("teacher-1", new CreateClassRequest { Name = "7a" }, CancellationToken.None);

        var result = await service.DeleteAsync("teacher-2", created.Id, CancellationToken.None);

        Assert.Equal(ClassOutcome.Forbidden, result.Outcome);
        Assert.Single(db.Classes);
    }

    [Fact]
    public async Task DeleteAsync_WhenClassDoesNotExist_ReturnsNotFound()
    {
        using var factory = await TestServiceFactory.CreateAsync();
        using var scope = factory.ServiceProvider.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var service = CreateService(db);

        var result = await service.DeleteAsync("teacher-1", Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(ClassOutcome.NotFound, result.Outcome);
    }
}
