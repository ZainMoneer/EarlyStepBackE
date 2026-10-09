namespace EarlyStep.Contracts.DTOS.Responses.ActivityResponses;

public class ActivityResponse
    {
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string? Description { get; set; }
    public string Slug { get; set; }
    public string Type { get; set; }
    public string? IconUrl { get; set; }
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; }
    public int QuestionCount { get; set; }
    public Guid CreatedById { get; set; }
    public string CreatedByName { get; set; }

    public List<ActivityQuestionResponse> Questions { get; set; } = [];
    }
