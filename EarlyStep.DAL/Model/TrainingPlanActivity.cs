using EarlyStep.DAL.Model;
//هون بنخزن النشاط الذي اختاره الأخصائي ونتيجته.
public class TrainingPlanActivity
    {
    public Guid Id { get; set; }

    public Guid TrainingPlanId { get; set; }
    public TrainingPlan TrainingPlan { get; set; }

    public Guid ActivityId { get; set; }
    public Activity Activity { get; set; }

    public int Order { get; set; }

    public DateTime AssignedAt { get; set; } = DateTime.UtcNow;

    // Result
    public int? Score { get; set; }

    public int? CorrectAnswers { get; set; }
    public int? WrongAnswers { get; set; }

    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }

    public ICollection<TrainingPlanQuestionAnswer> QuestionAnswers { get; set; } = new List<TrainingPlanQuestionAnswer>();
    }