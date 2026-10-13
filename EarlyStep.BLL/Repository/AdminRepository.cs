using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Requests.AdminRequests;
using EarlyStep.Contracts.DTOS.Requests.CategoryRequests;
using EarlyStep.Contracts.DTOS.Responses.AdminResponses;
using EarlyStep.Contracts.DTOS.Responses.CategoryResponses;
using EarlyStep.DAL.Data;
using EarlyStep.DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EarlyStep.BLL.Repository;

public class AdminRepository : IAdminRepository
    {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public AdminRepository(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager)
        {
        _context = context;
        _userManager = userManager;
        }


    public async Task<AdminDashboardResponse> GetDashboardAsync(
        CancellationToken cancellationToken = default)
        {
        var totalUsers = await _userManager.Users
            .CountAsync(cancellationToken);

        var totalReports = await _context.Reports
            .CountAsync(cancellationToken);

        var totalActivities = await _context.Activities
            .CountAsync(
                x => !x.IsDeleted,
                cancellationToken);

        var totalCategories = await _context.Categories
            .CountAsync(cancellationToken);
        var totalChildreen = await _context.Children.CountAsync(cancellationToken);

        return new AdminDashboardResponse
            {
            TotalUsers = totalUsers,
            TotalReports = totalReports,
            TotalActivities = totalActivities,
            TotalCategories = totalCategories,
            TotalChildreen = totalChildreen
            };
        }

    public async Task<AdminUsersResponse> GetUsersAsync(
        UserFilterRequest request,
        CancellationToken cancellationToken = default)
        {


        var parents = await _userManager
            .GetUsersInRoleAsync("ولي أمر");

        var specialists = await _userManager
            .GetUsersInRoleAsync("أخصائي/ة");

        var teachers = await _userManager
            .GetUsersInRoleAsync("معلم/ة");


        var allowedUserIds = parents
            .Select(x => x.Id)
            .Concat(specialists.Select(x => x.Id))
            .Concat(teachers.Select(x => x.Id))
            .Distinct()
            .ToHashSet();


        var users = await _userManager.Users
            .Where(x => allowedUserIds.Contains(x.Id))
            .ToListAsync(cancellationToken);


        var result = new List<AdminUserResponse>();


        foreach (var user in users)
            {
            var roles = await _userManager
                .GetRolesAsync(user);

            var role = roles.FirstOrDefault() ?? "";


            // Role Filter

            if (!string.IsNullOrWhiteSpace(request.Role))
                {
                if (!roles.Contains(request.Role))
                    continue;
                }


            // Search By Name Only

            if (!string.IsNullOrWhiteSpace(request.Search))
                {
                var search = request.Search.Trim();

                if (string.IsNullOrWhiteSpace(user.FullName) ||
                    !user.FullName.Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase))
                    {
                    continue;
                    }
                }

            // Children Count

            var childrenCount = 0;


            // ولي أمر
            if (roles.Contains("ولي أمر"))
                {
                childrenCount = await _context.Children
                    .CountAsync(
                        x => x.ParentId == user.Id,
                        cancellationToken);
                }


            // أخصائي
            else if (roles.Contains("أخصائي/ة"))
                {
                childrenCount = await _context.Children
                    .CountAsync(
                        x => x.SpecialistId == user.Id,
                        cancellationToken);
                }


            // معلم
            else if (roles.Contains("معلم/ة"))
                {
                childrenCount = await _context.TrainingPlans
                    .Where(x => x.TeacherId == user.Id)
                    .Select(x => x.ChildId)
                    .Distinct()
                    .CountAsync(cancellationToken);
                }


            result.Add(new AdminUserResponse
                {
                Id = user.Id,

                FullName = user.FullName,

                Email = user.Email ?? "",

                Role = role,

                EmailConfirmed = user.EmailConfirmed,

                ChildrenCount = childrenCount
                });
            }



        return new AdminUsersResponse
            {
            TotalUsers = result.Count,

            TotalParents = parents.Count,

            TotalSpecialists = specialists.Count,

            TotalTeachers = teachers.Count,

            Users = result
            };
        }
    public async Task<List<AdminSpecialistResponse>> GetAllSpecialistsAsync(
    CancellationToken cancellationToken = default)
        {
        var specialists = await _userManager
            .GetUsersInRoleAsync("أخصائي/ة");

        return specialists
            .Select(user => new AdminSpecialistResponse
                {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                IsActive = user.IsActive
                })
            .OrderBy(user => user.FullName)
            .ToList();
        }


    public async Task<bool> AssignChildrenToSpecialistAsync(
        AssignChildrenToSpecialistRequest request,
        CancellationToken cancellationToken = default)
        {
        if (request.ChildIds == null ||
            request.ChildIds.Count == 0)
            {
            return false;
            }


        var specialist = await _userManager
            .FindByIdAsync(
                request.SpecialistId.ToString());

        if (specialist == null)
            return false;


        var roles = await _userManager
            .GetRolesAsync(specialist);

        if (!roles.Contains("أخصائي/ة"))
            return false;


        var childIds = request.ChildIds
            .Distinct()
            .ToList();


        var children = await _context.Children
            .Where(x => childIds.Contains(x.Id))
            .ToListAsync(cancellationToken);


        if (children.Count != childIds.Count)
            return false;


        foreach (var child in children)
            {
            child.SpecialistId = request.SpecialistId;
            }


        await _context.SaveChangesAsync(
            cancellationToken);

        return true;
        }

    //هذه الفنكشن هي اللي تجيب الأطفال لما تضغطي على المستخدم.
    public async Task<AdminUserDetailsResponse?> GetUserByIdAsync(
    Guid userId,
    CancellationToken cancellationToken = default)
        {
        var user = await _userManager.Users
            .FirstOrDefaultAsync(
                x => x.Id == userId,
                cancellationToken);


        if (user == null)
            return null;


        var roles = await _userManager
            .GetRolesAsync(user);


        var role = roles.FirstOrDefault() ?? "";


        var children = new List<AdminChildResponse>();



        if (roles.Contains("ولي أمر"))
            {
            children = await _context.Children
                .Include(x => x.Parent)
                .Include(x => x.Specialist)
                .Where(x => x.ParentId == userId)
                .AsNoTracking()
                .Select(x => new AdminChildResponse
                    {
                    Id = x.Id,

                    FullName = x.FullName,

                    Age = x.Age,
                    Gender = x.Gender.ToString(),

                    ParentId = x.ParentId,

                    ParentName = x.Parent != null
                        ? x.Parent.FullName
                        : "",

                    SpecialistId = x.SpecialistId,

                    SpecialistName = x.Specialist != null
                        ? x.Specialist.FullName
                        : null
                    })
                .ToListAsync(cancellationToken);
            }


        else if (roles.Contains("أخصائي/ة"))
            {
            children = await _context.Children
                .Include(x => x.Parent)
                .Include(x => x.Specialist)
                .Where(x => x.SpecialistId == userId)
                .AsNoTracking()
                .Select(x => new AdminChildResponse
                    {
                    Id = x.Id,

                    FullName = x.FullName,

                    Age = x.Age,
                    Gender = x.Gender.ToString(),

                    ParentId = x.ParentId,

                    ParentName = x.Parent != null
                        ? x.Parent.FullName
                        : "",

                    SpecialistId = x.SpecialistId,

                    SpecialistName = x.Specialist != null
                        ? x.Specialist.FullName
                        : null
                    })
                .ToListAsync(cancellationToken);
            }

        else if (roles.Contains("معلم/ة"))
            {
            children = await _context.TrainingPlans
                .Where(x => x.TeacherId == userId)
                .Include(x => x.Child)
                    .ThenInclude(x => x.Parent)
                .Include(x => x.Child)
                    .ThenInclude(x => x.Specialist)
                .Select(x => x.Child)
                .Distinct()
                .AsNoTracking()
                .Select(x => new AdminChildResponse
                    {
                    Id = x.Id,

                    FullName = x.FullName,

                    Age = x.Age,

                    Gender = x.Gender.ToString(),

                    ParentId = x.ParentId,

                    ParentName = x.Parent != null
                        ? x.Parent.FullName
                        : "",

                    SpecialistId = x.SpecialistId,

                    SpecialistName = x.Specialist != null
                        ? x.Specialist.FullName
                        : null
                    })
                .ToListAsync(cancellationToken);
            }


        return new AdminUserDetailsResponse
            {
            Id = user.Id,

            FullName = user.FullName,

            Email = user.Email ?? "",

            Role = role,

            EmailConfirmed = user.EmailConfirmed,

            ChildrenCount = children.Count,

            Children = children
            };
        }

    public async Task<List<CategoryResponse>> GetAllCategoriesAsync(
        CancellationToken cancellationToken = default)
        {
        return await _context.Categories
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.Name)
            .Select(c => new CategoryResponse
                {
                Id = c.Id,
                Name = c.Name,
                IsDeleted = c.IsDeleted,
                IsInInitialAssessment = c.IsInInitialAssessment,
                })
            .ToListAsync(cancellationToken);
        }
    public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
        {
        var category = new Category
            {
            Id = Guid.NewGuid(),
            Name = request.Name
            };

        _context.Categories.Add(category);
        await _context.SaveChangesAsync(cancellationToken);

        return new CategoryResponse
            {
            Id = category.Id,
            Name = category.Name
            };
        }


    public async Task<CategoryResponse?> UpdateCategoryAsync(
    Guid id,
    UpdateCategoryRequest request,
    CancellationToken cancellationToken = default)
        {
        var category = await _context.Categories
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category == null)
            return null;

        category.Name = request.Name;

        await _context.SaveChangesAsync(cancellationToken);

        return new CategoryResponse
            {
            Id = category.Id,
            Name = category.Name,


            };
        }

    public async Task<bool> DeleteCategoryAsync(
      Guid id,
      CancellationToken cancellationToken = default)
        {
        var category = await _context.Categories
            .Include(c => c.Activities)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        if (category == null)
            return false;

        category.IsDeleted = true;

        foreach (var activity in category.Activities)
            {
            activity.IsDeleted = true;
            }

        await _context.SaveChangesAsync(cancellationToken);

        return true;
        }
    }
