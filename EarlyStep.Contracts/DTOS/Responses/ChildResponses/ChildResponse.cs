using EarlyStep.DAL.Model;

namespace EarlyStep.Contracts.DTOS.Responses.ChildResponses;

public class ChildResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; }

    public int Age { get; set; }
    public Gender Gender { get; set; }
    public Guid ParentId { get; set; }

}
