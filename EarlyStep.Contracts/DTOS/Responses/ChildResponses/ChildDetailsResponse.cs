using EarlyStep.DAL.Model;

namespace EarlyStep.Contracts.DTOS.Responses.ChildResponses;

public class ChildDetailsResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }

    public int GamesCount { get; set; }
    public int TestsCount { get; set; }
    public int ReportsCount { get; set; }
}