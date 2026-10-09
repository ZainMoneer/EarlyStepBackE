

namespace EarlyStep.Contracts.DTOS.Responses.ChildResponses;

public class ReportDetailsResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Weaknesses { get; set; }
    public string Recommendations { get; set; }
    public DateTime CreatedAt { get; set; }
}

