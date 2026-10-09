namespace EarlyStep.DAL.Model;

public class QuestionOption
    {
    public Guid Id { get; set; }

    public Guid QuestionId { get; set; }
    public ActivityQuestion Question { get; set; }

    public string? OptionText { get; set; }
    public string? OptionImageUrl { get; set; }
    public bool IsCorrect { get; set; }
    public int Order { get; set; }
    public bool IsDeleted { get; set; }

    public int? CorrectOrder { get; set; }   // ← جديد: يُستخدم بس لو Type = Sequence
    }