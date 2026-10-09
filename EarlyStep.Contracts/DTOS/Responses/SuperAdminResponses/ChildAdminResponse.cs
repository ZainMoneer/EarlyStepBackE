
namespace EarlyStep.Contracts.DTOS.Responses.SuperAdminResponses;

public class ChildAdminResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; }

    public int Age { get; set; }

    public string Gender { get; set; }

    public Guid ParentId { get; set; }

    public string ParentName { get; set; }
}