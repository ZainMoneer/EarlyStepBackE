
namespace EarlyStep.Contracts.DTOS.Responses.ActivityResponses;

public class PlayableQuestionResponse
{
    public Guid Id { get; set; }
    public string QuestionText { get; set; }
    public string QuestionImageUrl { get; set; }
    public string Type { get; set; }
    public int Order { get; set; }

    public List<PlayableOptionResponse> Options { get; set; } = [];
}
