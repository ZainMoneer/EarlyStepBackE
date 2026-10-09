using EarlyStep.Contracts.DTOS.Requests.ChildRequests;
using EarlyStep.Contracts.DTOS.Responses.ChildResponses;

namespace EarlyStep.BLL.Interfaces;

public interface IParentRepository
{
    // CRUD  , Create Update Delete Read
    Task<List<ChildResponse>> GetMyChildrenAsync(Guid parentId, CancellationToken cancellationToken = default);
    Task<ChildDetailsResponse?> GetByIdAsync(Guid childId, Guid parentId, CancellationToken cancellationToken = default);

    Task<ChildResponse> CreateChildAsync(Guid parentId, CreateChildRequest request, CancellationToken cancellationToken = default);

    Task<ChildResponse?> UpdateChildInfoAsync(Guid childId, Guid parentId, UpdateChildRequest request, CancellationToken cancellationToken = default);

    Task<bool> DeleteChildAsync(Guid childId, Guid parentId, CancellationToken cancellationToken = default);

}


