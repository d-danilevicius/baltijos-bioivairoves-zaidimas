using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.DTOs.Requests;
using Project.Api.Entities;

namespace Project.Api.Services;

public class TeacherTaskService : ITeacherTaskService
{
    private readonly ApplicationDbContext _dbContext;

    public TeacherTaskService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<TeacherTaskResult> CreateAsync(
        string ownerId,
        Guid classId,
        CreateTeacherTaskRequest request,
        CancellationToken cancellationToken)
    {
        var denied = await CheckClassAsync(ownerId, classId, cancellationToken);
        if (denied is not null)
        {
            return TeacherTaskResult.Failed(denied.Value);
        }

        var task = new ClassTask
        {
            Id = Guid.NewGuid(),
            ClassId = classId,
            Source = TaskSource.Teacher,
            Type = TaskType.TeacherTask,
            Title = request.Title,
            Question = request.Question,
            CorrectAnswer = request.CorrectAnswer,
            DifficultyGroup = request.DifficultyGroup.GetValueOrDefault()
        };

        _dbContext.ClassTasks.Add(task);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return TeacherTaskResult.Created(task.ToResponse());
    }

    public async Task<TeacherTaskResult> DeleteAsync(string ownerId, Guid classId, Guid taskId, CancellationToken cancellationToken)
    {
        var denied = await CheckClassAsync(ownerId, classId, cancellationToken);
        if (denied is not null)
        {
            return TeacherTaskResult.Failed(denied.Value);
        }

        var task = await _dbContext.ClassTasks.FirstOrDefaultAsync(t => t.Id == taskId && t.ClassId == classId, cancellationToken);
        if (task is null)
        {
            return TeacherTaskResult.Failed(TeacherTaskOutcome.NotFound);
        }

        // The GBIF tasks cannot be deleted (FR-034)
        if (task.Source != TaskSource.Teacher)
        {
            return TeacherTaskResult.Failed(TeacherTaskOutcome.NotTeacherTask);
        }

        _dbContext.ClassTasks.Remove(task);

        try
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        catch (DbUpdateConcurrencyException)
        {
            // Another request deleted the task between the load and the save
            return TeacherTaskResult.Failed(TeacherTaskOutcome.NotFound);
        }

        return TeacherTaskResult.Deleted();
    }

    // Returns the outcome that rejects the request, or null when the class belongs to the owner
    private async Task<TeacherTaskOutcome?> CheckClassAsync(string ownerId, Guid classId, CancellationToken cancellationToken)
    {
        var schoolClass = await _dbContext.Classes.AsNoTracking().FirstOrDefaultAsync(c => c.Id == classId, cancellationToken);
        if (schoolClass is null)
        {
            return TeacherTaskOutcome.NotFound;
        }

        return schoolClass.OwnerId == ownerId ? null : TeacherTaskOutcome.Forbidden;
    }
}
