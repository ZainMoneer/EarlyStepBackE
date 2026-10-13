using EarlyStep.Contracts.DTOS.Requests.AdminRequests;
using EarlyStep.Contracts.DTOS.Requests.CategoryRequests;
using EarlyStep.Contracts.DTOS.Responses.AdminResponses;
using EarlyStep.Contracts.DTOS.Responses.CategoryResponses;
namespace EarlyStep.BLL.Interfaces;

public interface IAdminRepository
{


    Task<AdminDashboardResponse> GetDashboardAsync(
        CancellationToken cancellationToken = default);


    Task<AdminUsersResponse> GetUsersAsync(
        UserFilterRequest request,
        CancellationToken cancellationToken = default);

    Task<AdminUserDetailsResponse?> GetUserByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
    Task<List<AdminSpecialistResponse>> GetAllSpecialistsAsync(
CancellationToken cancellationToken = default);


    Task<bool> AssignChildrenToSpecialistAsync(
        AssignChildrenToSpecialistRequest request,
        CancellationToken cancellationToken = default);

    Task<List<CategoryResponse>> GetAllCategoriesAsync(CancellationToken cancellationToken = default);

    Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default);
    Task<CategoryResponse?> UpdateCategoryAsync(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteCategoryAsync(Guid id, CancellationToken cancellationToken = default);
}



