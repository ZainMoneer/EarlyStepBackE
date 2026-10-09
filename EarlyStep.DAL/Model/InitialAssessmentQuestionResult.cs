namespace EarlyStep.DAL.Model;

public class InitialAssessmentQuestionResult
{
    public Guid Id { get; set; }

    public Guid InitialAssessmentId { get; set; }
    public InitialAssessment InitialAssessment { get; set; }

    public Guid QuestionId { get; set; }
    public ActivityQuestion Question { get; set; }

    public Guid? SelectedOptionId { get; set; }

    public bool? IsCorrect { get; set; }

}