namespace EarlyStep.Contracts.DTOS.Responses.SuperAdminResponses;


public class UserAdminResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }
    public bool EmailConfirmed { get; set; }
}