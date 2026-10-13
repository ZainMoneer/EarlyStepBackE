
namespace EarlyStep.DAL.Model;

public class TrainingPlanQuestionAnswer
    {
    public Guid Id { get; set; }

    public Guid TrainingPlanActivityId { get; set; }
    public TrainingPlanActivity TrainingPlanActivity { get; set; } = null!;

    public Guid QuestionId { get; set; }

    // Store selected option IDs as JSON
    public string SelectedOptionIdsJson { get; set; } = "[]";

    public bool IsCorrect { get; set; }

    public DateTime AnsweredAt { get; set; }
    }