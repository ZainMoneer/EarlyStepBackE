namespace EarlyStep.DAL.Model;

public enum ActivityType
    {
    ألعاب,
    اختبارات
    }
// جدول بمثل جميع الاختبارات والالعاب داخل النظام
public class Activity
    {
    public Guid Id { get; set; }

    public string Title { get; set; }

    public string? Description { get; set; }
    public string Slug { get; set; }


    public ActivityType Type { get; set; }

    public Guid CategoryId { get; set; }
    public Category Category { get; set; }

    public int? QuestionCount { get; set; }
    public string? IconUrl { get; set; }
    public Guid CreatedById { get; set; }
    public ApplicationUser CreatedBy { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool IsInInitialAssessment { get; set; }


    public ICollection<TrainingPlanActivity> TrainingPlanActivities { get; set; } = [];
    public ICollection<ActivityQuestion> Questions { get; set; } = [];
    }
