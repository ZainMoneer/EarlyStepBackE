namespace EarlyStep.Contracts.DTOS.Requests.AdminRequests;

public class AssignChildrenToSpecialistRequest
{
    public Guid SpecialistId { get; set; }

    public List<Guid> ChildIds { get; set; } = [];
}