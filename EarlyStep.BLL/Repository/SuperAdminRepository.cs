using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Requests.SuperAdmimRequests;
using EarlyStep.Contracts.DTOS.Responses.SuperAdminResponses;
using EarlyStep.DAL.Data;
using EarlyStep.DAL.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EarlyStep.BLL.Repository;

public class SuperAdminRepository : ISuperAdminRepository
    {
    private readonly ApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public SuperAdminRepository(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
        {
        _context = context;
        _userManager = userManager;
        _roleManager = roleManager;
        }

    public async Task<SuperAdminDashboardResponse> GetDashboardAsync(
        CancellationToken cancellationToken = default)
        {


        var totalUsers = await _userManager.Users
            .CountAsync(cancellationToken);



        var totalReports = await _context.Reports
            .CountAsync(cancellationToken);




        var totalActivities = await _context.Activities
            .CountAsync(cancellationToken);


        var roles = await _roleManager.Roles
            .Select(x => x.Name!)
            .Where(x => x != null)
            .ToListAsync(cancellationToken);


        var usersByRole = new List<RoleCountResponse>();


        foreach (var role in roles)
            {
            var usersInRole = await _userManager
                .GetUsersInRoleAsync(role);

            usersByRole.Add(new RoleCountResponse
                {
                Role = role,
                Count = usersInRole.Count
                });
            }



        return new SuperAdminDashboardResponse
            {
            TotalUsers = totalUsers,

            TotalReports = totalReports,

            TotalActivities = totalActivities,

            UsersByRole = usersByRole,

            };
        }

    public async Task<List<UserAdminResponse>> GetUsersAsync(
        UserFilterRequest request,
        CancellationToken cancellationToken = default)
        {
        var users = _userManager.Users
            .AsQueryable();


        if (!string.IsNullOrWhiteSpace(request.Search))
            {
            var search = request.Search.Trim();

            users = users.Where(x =>
                (x.UserName != null &&
                 x.UserName.Contains(search)) ||

                (x.Email != null &&
                 x.Email.Contains(search)));
            }


        var userList = await users
            .ToListAsync(cancellationToken);


        var result = new List<UserAdminResponse>();



        foreach (var user in userList)
            {
            var roles = await _userManager
                .GetRolesAsync(user);


            // Filter By Role

            if (!string.IsNullOrWhiteSpace(request.Role))
                {
                if (!roles.Contains(request.Role))
                    continue;
                }


            result.Add(new UserAdminResponse
                {
                Id = user.Id,

                FullName = user.FullName ?? "",

                Email = user.Email ?? "",

                Role = roles.FirstOrDefault() ?? "",

                EmailConfirmed = user.EmailConfirmed,
                isActive = user.IsActive
            });
            }


        return result;
        }




    public async Task<List<ChildAdminResponse>> GetChildrenAsync(
        ChildFilterRequest request,
        CancellationToken cancellationToken = default)
        {
        var children = _context.Children
            .Include(x => x.Parent)
            .AsQueryable();


        // Search By Child Name

        if (!string.IsNullOrWhiteSpace(request.Name))
            {
            var name = request.Name.Trim();

            children = children.Where(x =>
                x.FullName.Contains(name));
            }


        var result = await children
            .ToListAsync(cancellationToken);


        return result
            .Select(x => new ChildAdminResponse
                {
                Id = x.Id,

                FullName = x.FullName,

                Age = x.Age,
                Gender = x.Gender.ToString(),

                ParentId = x.ParentId,

                ParentName = x.Parent != null
                    ? x.Parent.UserName!
                    : ""
                })
            .ToList();
        }
    public async Task<UserManagementResponse?> CreateUserAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default)
        {
        cancellationToken.ThrowIfCancellationRequested();

        var fullName = request.FullName?.Trim();
        var email = request.Email?.Trim();
        var role = request.Role?.Trim();

        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(role) ||
            string.IsNullOrWhiteSpace(request.Password))
            {
            throw new ArgumentException(
                "Full name, email, password and role are required.");
            }

        if (!IsAllowedManagedRole(role) ||
            !await _roleManager.RoleExistsAsync(role))
            {
            throw new ArgumentException(
                "Invalid or unauthorized user role.");
            }

        var existingUser = await _userManager.FindByEmailAsync(email);

        if (existingUser is not null)
            {
            throw new InvalidOperationException(
                "Email is already registered.");
            }

        var user = new ApplicationUser
        {
            Id = Guid.NewGuid(),
            FullName = fullName,
            Email = email,
            UserName = email,

            EmailConfirmed = true,

            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var createResult = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!createResult.Succeeded)
            {
            throw new ArgumentException(
                string.Join("; ",
                    createResult.Errors.Select(e => e.Description)));
            }

        var roleResult = await _userManager.AddToRoleAsync(
            user,
            role);

        if (!roleResult.Succeeded)
            {
            var rollbackResult = await _userManager.DeleteAsync(user);

            if (!rollbackResult.Succeeded)
                {
                throw new InvalidOperationException(
                    "Role assignment failed and user rollback failed.");
                }

            throw new InvalidOperationException(
                string.Join("; ",
                    roleResult.Errors.Select(e => e.Description)));
            }

        return new UserManagementResponse
            {
            Message = "User created successfully.",
            Id = user.Id,
            FullName = user.FullName,
            Email = user.Email ?? string.Empty,
            Role = role,
            IsActive = user.IsActive
            };
        }
    public async Task<UserManagementResponse?> UpdateUserAsync(
    Guid userId,
    UpdateUserRequest request,
    CancellationToken cancellationToken = default)
        {
        cancellationToken.ThrowIfCancellationRequested();

        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        if (user is null)
            return null;

        if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
            throw new InvalidOperationException(
                "SuperAdmin accounts cannot be modified.");
            }

        var fullName = request.FullName?.Trim();
        var email = request.Email?.Trim();
        var newRole = request.Role?.Trim();

        if (string.IsNullOrWhiteSpace(fullName) ||
            string.IsNullOrWhiteSpace(email) ||
            string.IsNullOrWhiteSpace(newRole))
            {
            throw new ArgumentException(
                "Full name, email and role are required.");
            }

        if (!IsAllowedManagedRole(newRole) ||
            !await _roleManager.RoleExistsAsync(newRole))
            {
            throw new ArgumentException(
                "Invalid or unauthorized user role.");
            }

        var existingEmailUser =
            await _userManager.FindByEmailAsync(email);

        if (existingEmailUser is not null &&
            existingEmailUser.Id != user.Id)
            {
            throw new InvalidOperationException(
                "Email is already registered.");
            }

        var currentRoles = await _userManager.GetRolesAsync(user);

        await using var transaction =
            await _context.Database.BeginTransactionAsync(
                cancellationToken);

        try
            {
            user.FullName = fullName;
            user.Email = email;
            user.UserName = email;
            user.IsActive = request.IsActive;

            var updateResult = await _userManager.UpdateAsync(user);

            if (!updateResult.Succeeded)
                {
                throw new ArgumentException(
                    string.Join("; ",
                        updateResult.Errors.Select(e => e.Description)));
                }

            if (!currentRoles.Contains(newRole))
                {
                var addResult = await _userManager.AddToRoleAsync(
                    user,
                    newRole);

                if (!addResult.Succeeded)
                    {
                    throw new InvalidOperationException(
                        string.Join("; ",
                            addResult.Errors.Select(e => e.Description)));
                    }
                }

            var oldRoles = currentRoles
                .Where(r => r != newRole)
                .ToArray();

            if (oldRoles.Length > 0)
                {
                var removeResult =
                    await _userManager.RemoveFromRolesAsync(
                        user,
                        oldRoles);

                if (!removeResult.Succeeded)
                    {
                    throw new InvalidOperationException(
                        string.Join("; ",
                            removeResult.Errors.Select(e => e.Description)));
                    }
                }

            await transaction.CommitAsync(cancellationToken);

            return new UserManagementResponse
                {
                Message = "User updated successfully.",
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email ?? string.Empty,
                Role = newRole,
                IsActive = user.IsActive
                };
            }
        catch
            {
            await transaction.RollbackAsync(
                cancellationToken);

            throw;
            }
        }
    public async Task<IdentityResult> DeleteUserAsync(
    Guid userId,
    Guid currentSuperAdminId,
    CancellationToken cancellationToken = default)
        {
        cancellationToken.ThrowIfCancellationRequested();

        if (userId == currentSuperAdminId)
            {
            return IdentityResult.Failed(new IdentityError
                {
                Code = "Forbidden",
                Description = "You cannot deactivate your own account."
                });
            }

        var user = await _userManager.FindByIdAsync(
            userId.ToString());

        if (user is null)
            {
            return IdentityResult.Failed(new IdentityError
                {
                Code = "NotFound",
                Description = "User not found."
                });
            }

        if (await _userManager.IsInRoleAsync(user, "SuperAdmin"))
            {
            return IdentityResult.Failed(new IdentityError
                {
                Code = "Forbidden",
                Description = "SuperAdmin accounts cannot be deactivated here."
                });
            }

        if (!user.IsActive)
            return IdentityResult.Success;

        user.IsActive = false;

        return await _userManager.UpdateAsync(user);
        }

    private static bool IsAllowedManagedRole(string role)
        {
        var allowedRoles = new HashSet<string>(
            StringComparer.Ordinal)
    {
        "ولي أمر",
        "معلم/ة",
        "أخصائي/ة",
        "Admin"
    };

        return allowedRoles.Contains(role);
        }
    }