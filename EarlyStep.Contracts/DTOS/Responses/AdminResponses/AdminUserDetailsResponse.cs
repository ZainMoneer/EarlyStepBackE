namespace EarlyStep.Contracts.DTOS.Responses.AdminResponses;

public class AdminUserDetailsResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = "";

    public string Email { get; set; } = "";

    public string Role { get; set; } = "";

    public bool EmailConfirmed { get; set; }

    public int ChildrenCount { get; set; }

    public List<AdminChildResponse> Children { get; set; } = [];
}