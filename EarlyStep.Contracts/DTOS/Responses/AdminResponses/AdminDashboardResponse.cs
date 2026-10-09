using System.Text.Json.Serialization;

namespace EarlyStep.Contracts.DTOS.Responses.AdminResponses;

public class AdminDashboardResponse
{
    [JsonPropertyName("إجمالي المستخدمين")]

    public int TotalUsers { get; set; }
    [JsonPropertyName("إجمالي التقارير")]

    public int TotalReports { get; set; }
    [JsonPropertyName("إجمالي الانشطة")]

    public int TotalActivities { get; set; }

    [JsonPropertyName("إجمالي الفئات")]

    public int TotalCategories { get; set; }
    [JsonPropertyName("عدد الأطفال")]

    public int TotalChildreen { get; set; }
}