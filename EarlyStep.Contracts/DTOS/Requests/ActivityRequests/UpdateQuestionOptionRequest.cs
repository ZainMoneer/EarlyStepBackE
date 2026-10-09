
namespace EarlyStep.Contracts.DTOS.Requests.ActivityRequests;

public class UpdateQuestionOptionsRequest
    {
    public List<UpdateOptionRequest> Options { get; set; } = new();
    }