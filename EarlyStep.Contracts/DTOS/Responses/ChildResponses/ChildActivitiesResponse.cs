namespace EarlyStep.Contracts.DTOS.Responses.ChildResponses;

public class ChildActivitiesResponse
{
    public int Count { get; set; }

    public List<ChildActivityResponse> Activities { get; set; } = new();
}