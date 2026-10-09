namespace EarlyStep.Contracts.DTOS.Responses.AdminResponses;

public class AdminUsersResponse
{
    public int TotalUsers { get; set; }

    public int TotalParents { get; set; }

    public int TotalSpecialists { get; set; }

    public int TotalTeachers { get; set; }

    public List<AdminUserResponse> Users { get; set; } = [];
}