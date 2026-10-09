namespace EarlyStep.Contracts.DTOS.Responses.SuperAdminResponses;

public class SuperAdminDashboardResponse
{
    public int TotalUsers { get; set; }

    public int TotalReports { get; set; }

    public int TotalActivities { get; set; }

    public List<RoleCountResponse> UsersByRole { get; set; } = new();

}