using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Project.Api.Data;
using Project.Api.Entities;
using Project.Api.Services;
using Xunit;

namespace Project.UnitTests.TeacherTasks;

public class TeacherTaskDeletionRaceTests
{
    private sealed class DeleteOtherCopyBeforeSaveInterceptor : SaveChangesInterceptor
    {
        private readonly DbContextOptions<ApplicationDbContext> _otherOptions;
        private readonly Guid _taskId;

        public DeleteOtherCopyBeforeSaveInterceptor(DbContextOptions<ApplicationDbContext> otherOptions, Guid taskId)
        {
            _otherOptions = otherOptions;
            _taskId = taskId;
        }

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            await using var other = new ApplicationDbContext(_otherOptions);
            other.ClassTasks.Remove(await other.ClassTasks.SingleAsync(t => t.Id == _taskId, cancellationToken));
            await other.SaveChangesAsync(cancellationToken);
            return result;
        }
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskIsDeletedByAnotherRequestBeforeSave_ReturnsNotFound()
    {
        var databaseName = Guid.NewGuid().ToString();
        var plainOptions = new DbContextOptionsBuilder<ApplicationDbContext>().UseInMemoryDatabase(databaseName).Options;
        var classId = Guid.NewGuid();
        var taskId = Guid.NewGuid();
        await using (var seed = new ApplicationDbContext(plainOptions))
        {
            seed.Classes.Add(new SchoolClass { Id = classId, Name = "7a", Code = "AAAAAA", OwnerId = "teacher-1", CreatedAtUtc = DateTime.UtcNow });
            seed.ClassTasks.Add(new ClassTask
            {
                Id = taskId,
                ClassId = classId,
                Source = TaskSource.Teacher,
                Type = TaskType.TeacherTask,
                DifficultyGroup = 1,
                Question = "Klausimas",
                CorrectAnswer = "Atsakymas"
            });
            await seed.SaveChangesAsync();
        }

        var racingOptions = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName)
            .AddInterceptors(new DeleteOtherCopyBeforeSaveInterceptor(plainOptions, taskId))
            .Options;
        await using var db = new ApplicationDbContext(racingOptions);

        var result = await new TeacherTaskService(db).DeleteAsync("teacher-1", classId, taskId, CancellationToken.None);

        Assert.Equal(TeacherTaskOutcome.NotFound, result.Outcome);
    }
}
