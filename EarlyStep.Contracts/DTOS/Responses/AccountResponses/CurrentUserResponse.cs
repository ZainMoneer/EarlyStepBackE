
namespace EarlyStep.Contracts.DTOS.Responses.AccountResponses;

public class CurrentUserResponse
    {
    public Guid Id { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public IList<string> Roles { get; set; } = [];
    }