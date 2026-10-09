namespace EarlyStep.Contracts.DTOS.Responses.ActivityResponses;

public class PlayableActivityResponse
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }

    public List<PlayableQuestionResponse> Questions { get; set; } = [];
}
