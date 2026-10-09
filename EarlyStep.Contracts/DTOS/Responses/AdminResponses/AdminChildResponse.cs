namespace EarlyStep.Contracts.DTOS.Responses.AdminResponses;

public class AdminChildResponse
{
    public Guid Id { get; set; }

    public string FullName { get; set; } = "";

    public int Age { get; set; }

    public string Gender { get; set; } = "";

    public Guid ParentId { get; set; }

    public string ParentName { get; set; } = "";

    public Guid? SpecialistId { get; set; }

    public string? SpecialistName { get; set; }
}