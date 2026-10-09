
namespace EarlyStep.DAL.Model;

public class TrainingPlanQuestionAnswer
    {
    public Guid Id { get; set; }

    public Guid TrainingPlanActivityId { get; set; }
    public TrainingPlanActivity TrainingPlanActivity { get; set; }

    public Guid QuestionId { get; set; }
    public ActivityQuestion Question { get; set; }

    // إجابة الطفل حسب نوع السؤال
    public string? Answer { get; set; }

    public bool IsCorrect { get; set; }

    public int? ResponseTimeSeconds { get; set; }

    public DateTime AnsweredAt { get; set; } = DateTime.UtcNow;
    }