namespace EarlyStep.Contracts.DTOS.Responses.SuperAdminResponses;

public class UserManagementResponse
    {
    public string Message { get; set; }

    public Guid Id { get; set; }

    public string FullName { get; set; }

    public string Email { get; set; }

    public string Role { get; set; }

    public bool IsActive { get; set; }
    }