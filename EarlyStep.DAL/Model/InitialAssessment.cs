using EarlyStep.DAL.Model;

public class InitialAssessment
    {
    public Guid Id { get; set; }

    public Guid ChildId { get; set; }

    public Child Child { get; set; }

    public double OverallScore { get; set; }

    public ICollection<InitialAssessmentQuestionResult> QuestionResults { get; set; } = [];
    }