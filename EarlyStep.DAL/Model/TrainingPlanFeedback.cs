using EarlyStep.DAL.Model;

public class TrainingPlanFeedback
    {
    public Guid Id { get; set; }

    public Guid TrainingPlanId { get; set; }
    public TrainingPlan TrainingPlan { get; set; }

    public Guid ChildId { get; set; }
    public Child Child { get; set; }

    public Guid TeacherId { get; set; }
    public ApplicationUser Teacher { get; set; }

    public string Content { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }