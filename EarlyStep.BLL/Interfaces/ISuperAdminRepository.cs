
using EarlyStep.Contracts.DTOS.Requests.SuperAdmimRequests;
using EarlyStep.Contracts.DTOS.Responses.SuperAdminResponses;
using Microsoft.AspNetCore.Identity;

namespace EarlyStep.BLL.Interfaces;


public interface ISuperAdminRepository
    {

    Task<SuperAdminDashboardResponse> GetDashboardAsync(
        CancellationToken cancellationToken = default);


    Task<List<UserAdminResponse>> GetUsersAsync(
        UserFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<List<ChildAdminResponse>> GetChildrenAsync(
        ChildFilterRequest request,
        CancellationToken cancellationToken = default);
    Task<UserManagementResponse?> CreateUserAsync(
        CreateUserRequest request,
        CancellationToken cancellationToken = default);

    Task<UserManagementResponse?> UpdateUserAsync(
        Guid userId,
        UpdateUserRequest request,
        CancellationToken cancellationToken = default);
    Task<IdentityResult> DeleteUserAsync(
        Guid userId,
        Guid currentSuperAdminId,
        CancellationToken cancellationToken = default);
    }
