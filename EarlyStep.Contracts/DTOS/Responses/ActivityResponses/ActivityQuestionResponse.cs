namespace EarlyStep.Contracts.DTOS.Responses.ActivityResponses;

public class ActivityQuestionResponse
    {
    public Guid Id { get; set; }

    public string? QuestionText { get; set; }

    public string? QuestionImageUrl { get; set; }

    public string Type { get; set; } = string.Empty;

    public int Order { get; set; }

    public List<ActivityOptionResponse> Options { get; set; } = new();
    }