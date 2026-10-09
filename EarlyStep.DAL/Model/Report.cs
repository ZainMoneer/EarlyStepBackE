namespace EarlyStep.DAL.Model;

public class Report
{
    public Guid Id { get; set; }

    public Guid ChildId { get; set; }
    public Child Child { get; set; }

    public Guid SpecialistId { get; set; }
    public ApplicationUser Specialist { get; set; }

    public string Title { get; set; }

    public string Weaknesses { get; set; }

    public string Recommendations { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

}