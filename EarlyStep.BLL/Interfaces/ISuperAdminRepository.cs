
using EarlyStep.Contracts.DTOS.Requests.SuperAdmimRequests;
using EarlyStep.Contracts.DTOS.Responses.SuperAdminResponses;

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
}
