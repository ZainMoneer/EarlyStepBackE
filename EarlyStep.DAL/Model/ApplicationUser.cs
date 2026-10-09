using Microsoft.AspNetCore.Identity;

namespace EarlyStep.DAL.Model;

public class ApplicationUser : IdentityUser<Guid>
    {
    public string FullName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;


    public ICollection<Child> ParentChildren { get; set; } = [];

    public ICollection<Child> SpecialistChildren { get; set; } = [];

    public ICollection<Report> ReportsCreated { get; set; } = [];

    public ICollection<TrainingPlanFeedback> TeacherNotes { get; set; } = [];

    public ICollection<Conversation> ConversationsAsSender { get; set; } = [];

    public ICollection<Conversation> ConversationsAsReceiver { get; set; } = [];

    public ICollection<Message> MessagesSent { get; set; } = [];

    public ICollection<Message> MessagesReceived { get; set; } = [];
    public ICollection<TrainingPlan> TrainingPlansAsSpecialist { get; set; } = [];

    public ICollection<TrainingPlan> TrainingPlansAsTeacher { get; set; } = [];
    }