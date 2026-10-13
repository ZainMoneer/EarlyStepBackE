
namespace EarlyStep.Contracts.DTOS.Responses.AdminResponses;

public class AdminSpecialistResponse
    {
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
    }