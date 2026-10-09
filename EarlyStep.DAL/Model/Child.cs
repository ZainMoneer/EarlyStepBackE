namespace EarlyStep.DAL.Model;

public enum Gender
{
    ولد,
    بنت
}

public class Child
{
    public Guid Id { get; set; }


    public string FullName { get; set; }
    public int Age { get; set; }
    public Gender Gender { get; set; }

    public Guid ParentId { get; set; }
    public ApplicationUser Parent { get; set; }


    public Guid? SpecialistId { get; set; }
    public ApplicationUser? Specialist { get; set; }

    public Guid? TeacherId { get; set; }
    public ApplicationUser? Teacher { get; set; }

    public InitialAssessment? InitialAssessment { get; set; }

    public ICollection<Report> Reports { get; set; } = [];
    public ICollection<TrainingPlan> TrainingPlans { get; set; } = [];
    public ICollection<TrainingPlanFeedback> TrainingPlanFeedbacks { get; set; } = [];
}
