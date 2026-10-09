namespace EarlyStep.DAL.Model;

public enum TrainingPlanStatus
    {
    Active,
    Completed,
    Cancelled
    }

public class TrainingPlan
    {
    public Guid Id { get; set; }

    public Guid ChildId { get; set; }
    public Child Child { get; set; }

    public Guid SpecialistId { get; set; }
    public ApplicationUser Specialist { get; set; }

    public Guid TeacherId { get; set; }
    public ApplicationUser Teacher { get; set; }

    public TrainingPlanStatus Status { get; set; } = TrainingPlanStatus.Active;

    public ICollection<TrainingPlanActivity> Activities { get; set; } = [];
    }
/*
 * يمثل القرار الكامل الذي أخذه الأخصائي للطفل.
 * هذا الكلاس يخزن:

لأي طفل الخطة.
أي أخصائي أنشأها.
أي معلم سيتابع الطفل.
تعليمات الأخصائي.
حالة الخطة.
 */