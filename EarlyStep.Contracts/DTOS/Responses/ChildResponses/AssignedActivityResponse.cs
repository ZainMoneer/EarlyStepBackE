namespace EarlyStep.Contracts.DTOS.Responses.ChildResponses;

public class AssignedActivityResponse
{
    public Guid TrainingPlanActivityId { get; set; }

    public Guid ActivityId { get; set; }

    public string Title { get; set; }

    public string CategoryName { get; set; }

    public DateTime AssignedAt { get; set; }
}
