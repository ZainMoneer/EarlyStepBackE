namespace EarlyStep.Contracts.DTOS.Responses.ActivityResponses;

public class ActivityOptionResponse
    {
    public Guid Id { get; set; }

    public string? OptionText { get; set; }

    public string? OptionImageUrl { get; set; }

    public int Order { get; set; }

    public bool IsCorrect { get; set; }

    public int? CorrectOrder { get; set; }
    }