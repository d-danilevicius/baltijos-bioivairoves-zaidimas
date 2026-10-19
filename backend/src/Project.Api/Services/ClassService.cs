using Microsoft.EntityFrameworkCore;
using Project.Api.Data;
using Project.Api.DTOs.Requests;
using Project.Api.DTOs.Responses;
using Project.Api.Entities;

namespace Project.Api.Services;

public class ClassService : IClassService
{
    private const int MaxCodeAttempts = 10;

    private readonly ApplicationDbContext _dbContext;
    private readonly IClassCodeGenerator _codeGenerator;
    private readonly TimeProvider _timeProvider;

    public ClassService(ApplicationDbContext dbContext, IClassCodeGenerator codeGenerator, TimeProvider timeProvider)
    {
        _dbContext = dbContext;
        _codeGenerator = codeGenerator;
        _timeProvider = timeProvider;
    }

    public async Task<ClassResponse> CreateAsync(string ownerId, CreateClassRequest request, CancellationToken cancellationToken)
    {
        var schoolClass = new SchoolClass
        {
            Id = Guid.NewGuid(),
            Name = request.Name.Trim(),
            Code = await GenerateUniqueCodeAsync(cancellationToken),
            OwnerId = ownerId,
            CreatedAtUtc = _timeProvider.GetUtcNow().UtcDateTime
        };

        _dbContext.Classes.Add(schoolClass);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ToResponse(schoolClass);
    }

    public async Task<IReadOnlyList<ClassResponse>> ListAsync(string ownerId, CancellationToken cancellationToken)
    {
        var classes = await _dbContext.Classes
            .Where(c => c.OwnerId == ownerId)
            .OrderByDescending(c => c.CreatedAtUtc)
            .ToListAsync(cancellationToken);

        return classes.Select(ToResponse).ToList();
    }

    public async Task<ClassResult> GetAsync(string ownerId, Guid id, CancellationToken cancellationToken)
    {
        var schoolClass = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (schoolClass is null)
        {
            return ClassResult.NotFound();
        }

        return schoolClass.OwnerId == ownerId
            ? ClassResult.Succeeded(ToResponse(schoolClass))
            : ClassResult.Forbidden();
    }

    public async Task<ClassResult> DeleteAsync(string ownerId, Guid id, CancellationToken cancellationToken)
    {
        var schoolClass = await _dbContext.Classes.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (schoolClass is null)
        {
            return ClassResult.NotFound();
        }

        if (schoolClass.OwnerId != ownerId)
        {
            return ClassResult.Forbidden();
        }

        _dbContext.Classes.Remove(schoolClass);
        await _dbContext.SaveChangesAsync(cancellationToken);

        return ClassResult.Succeeded();
    }

    private async Task<string> GenerateUniqueCodeAsync(CancellationToken cancellationToken)
    {
        for (var attempt = 0; attempt < MaxCodeAttempts; attempt++)
        {
            var code = _codeGenerator.Generate();
            if (!await _dbContext.Classes.AnyAsync(c => c.Code == code, cancellationToken))
            {
                return code;
            }
        }

        throw new InvalidOperationException("Could not generate a unique class code.");
    }

    private static ClassResponse ToResponse(SchoolClass schoolClass) => new()
    {
        Id = schoolClass.Id,
        Name = schoolClass.Name,
        Code = schoolClass.Code,
        CreatedAtUtc = schoolClass.CreatedAtUtc
    };
}
