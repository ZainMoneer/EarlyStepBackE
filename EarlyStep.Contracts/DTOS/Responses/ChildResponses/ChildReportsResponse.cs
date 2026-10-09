namespace EarlyStep.Contracts.DTOS.Responses.ChildResponses;

public class ChildReportsResponse
{
    public int Count { get; set; }

    public List<ChildReportItemResponse> Reports { get; set; } = new();
}
