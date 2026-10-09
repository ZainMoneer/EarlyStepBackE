

namespace EarlyStep.Contracts.DTOS.Responses.AccountResponses;


public class AuthResponse
{
    public Guid Id { get; set; }
    public string? FullName { get; set; }
    public string? AccessToken { get; set; }
    public string Role { get; set; }
}

