namespace EarlyStep.Contracts.DTOS.Responses.CategoryResponses;

public class CategoryResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = "";
    public int Count { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool IsInInitialAssessment { get; set; } = false;
    public int ActivityCount { get; set; }

    }
