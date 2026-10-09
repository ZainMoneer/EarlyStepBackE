
namespace EarlyStep.DAL.Model;

public enum QuestionType
    {
    SingleChoice,
    MultiSelect,
    Sequence,
    }
public class ActivityQuestion
    {
    public Guid Id { get; set; }

    public Guid ActivityId { get; set; }
    public Activity Activity { get; set; }

    public string? QuestionText { get; set; }
    public string? QuestionImageUrl { get; set; } // اختياري - لو السؤال نفسه صورة (بعض الألعاب)
    public QuestionType Type { get; set; }
    public int Order { get; set; }
    public bool IsDeleted { get; set; }
    public ICollection<QuestionOption> Options { get; set; } = [];
    }