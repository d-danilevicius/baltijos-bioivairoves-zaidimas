using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Project.Api.Data;
using Project.Api.Entities;
using Project.Api.Services;

namespace Project.UnitTests.Classes;

public class ClassDeletionRaceTests
{
    private sealed class DeleteOtherCopyBeforeSaveInterceptor : SaveChangesInterceptor
    {
        private readonly DbContextOptions<ApplicationDbContext> _otherOptions;
        private readonly Guid _classId;

        public DeleteOtherCopyBeforeSaveInterceptor(DbContextOptions<ApplicationDbContext> otherOptions, Guid classId)
        {
            _otherOptions = otherOptions;
            _classId = classId;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            await using var other = new ApplicationDbContext(_otherOptions);
            other.Classes.Remove(await other.Classes.SingleAsync(c => c.Id == _classId, cancellationToken));
            await other.SaveChangesAsync(cancellationToken);
            return result;
        }
    }

    [Fact]
    public async Task DeleteAsync_WhenClassIsDeletedByAnotherRequestBeforeSave_ReturnsNotFound()
    {
        var databaseName = Guid.NewGuid().ToString();
        var plainOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName).Options;
        var classId = Guid.NewGuid();
        await using (var seed = new ApplicationDbContext(plainOptions))
        {
            seed.Classes.Add(new SchoolClass { Id = classId, Name = "7a", Code = "AAAAAA", OwnerId = "teacher-1", CreatedAtUtc = DateTime.UtcNow });
            await seed.SaveChangesAsync();
        }

        var racingOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .AddInterceptors(new DeleteOtherCopyBeforeSaveInterceptor(plainOptions, classId))
            .Options;
        await using var db = new ApplicationDbContext(racingOptions);
        var service = new ClassService(db, new ClassCodeGenerator(), TimeProvider.System);

        var result = await service.DeleteAsync("teacher-1", classId, CancellationToken.None);

        Assert.Equal(ClassOutcome.NotFound, result.Outcome);
    }
}
