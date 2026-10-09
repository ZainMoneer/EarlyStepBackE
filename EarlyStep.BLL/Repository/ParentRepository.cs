using EarlyStep.BLL.Interfaces;
using EarlyStep.BLL.Utility;
using EarlyStep.Contracts.DTOS.Requests.ChildRequests;
using EarlyStep.Contracts.DTOS.Responses.ChildResponses;
using EarlyStep.DAL.Data;
using EarlyStep.DAL.Model;
using Microsoft.EntityFrameworkCore;

namespace EarlyStep.BLL.Repository;

public class ParentRepository : IParentRepository
{
    private readonly ApplicationDbContext _context;

    public ParentRepository(ApplicationDbContext context)
    {
        _context = context;
    }


    public async Task<ChildResponse> CreateChildAsync(
        Guid parentId,
        CreateChildRequest request,
        CancellationToken cancellationToken = default)
    {
        var child = new Child
        {
            Id = Guid.NewGuid(),
            FullName = request.FullName,
            Age = request.Age,
            Gender = request.Gender!.Value,
            ParentId = parentId
        };

        _context.Children.Add(child);

        await _context.SaveChangesAsync(cancellationToken);

        return ToResponse(child);
    }


    public async Task<List<ChildResponse>> GetMyChildrenAsync(
        Guid parentId,
        CancellationToken cancellationToken = default)
    {
        var children = await _context.Children
            .AsNoTracking()
            .Where(c => c.ParentId == parentId)
            .OrderBy(c => c.FullName)
            .ToListAsync(cancellationToken);

        return children
      .Select(child => ToResponse(child))
      .ToList();
    }


    public async Task<ChildResponse?> UpdateChildInfoAsync(
        Guid childId,
        Guid parentId,
        UpdateChildRequest request,
        CancellationToken cancellationToken = default)
    {
        var child = await _context.Children
            .FirstOrDefaultAsync(
                c => c.Id == childId,
                cancellationToken);

        if (child is null || child.ParentId != parentId)
            return null;

        if (!string.IsNullOrWhiteSpace(request.FullName))
            child.FullName = request.FullName;

        if (request.Age.HasValue)
            child.Age = request.Age.Value;

        if (request.Gender.HasValue)
            child.Gender = request.Gender.Value;

        await _context.SaveChangesAsync(cancellationToken);

        return ToResponse(child);
    }


    public async Task<bool> DeleteChildAsync(
        Guid childId,
        Guid parentId,
        CancellationToken cancellationToken = default)
    {
        var child = await _context.Children
            .FirstOrDefaultAsync(
                c => c.Id == childId,
                cancellationToken);

        if (child is null || child.ParentId != parentId)
            return false;

        _context.Children.Remove(child);

        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    // هون لما نضعط على طفل برجع كل بياناته

    public async Task<ChildDetailsResponse?> GetByIdAsync(
        Guid childId,
        Guid parentId,
        CancellationToken cancellationToken = default)
    {
        var child = await _context.Children
            .AsNoTracking()
            .FirstOrDefaultAsync(
                c => c.Id == childId &&
                     c.ParentId == parentId,
                cancellationToken);

        if (child is null)
            return null;

        var gamesCount = await _context.TrainingPlanActivities
            .CountAsync(
                tpa =>
                    tpa.TrainingPlan.ChildId == childId &&
                    tpa.Activity.Type == ActivityType.ألعاب,
                cancellationToken);

        var testsCount = await _context.TrainingPlanActivities
            .CountAsync(
                tpa =>
                    tpa.TrainingPlan.ChildId == childId &&
                    tpa.Activity.Type == ActivityType.اختبارات,
                cancellationToken);

        var reportsCount = await _context.Reports
            .CountAsync(
                r => r.ChildId == childId,
                cancellationToken);


        return new ChildDetailsResponse
        {
            Id = child.Id,
            FullName = child.FullName,
            Age = child.Age,
            Gender = child.Gender,

            GamesCount = gamesCount,
            TestsCount = testsCount,

            ReportsCount = reportsCount
        };
    }

    private static ChildResponse ToResponse(Child child)
    {
        return new ChildResponse
        {
            Id = child.Id,
            FullName = child.FullName,
            Age = child.Age,
            Gender = child.Gender,
            ParentId = child.ParentId
        };
    }

}
