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

                EmailConfirmed = user.EmailConfirmed
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
}