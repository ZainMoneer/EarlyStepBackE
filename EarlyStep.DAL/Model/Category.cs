namespace EarlyStep.DAL.Model;

// جدول تصنيفات المهارات (رياضيات، قراءة، تركيز، ذاكرة...) بدل ما تكون enum ثابتة بالكود
public class Category
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool IsInInitialAssessment { get; set; } = false;
    public int InitialAssessmentOrder { get; set; }

    public ICollection<Activity> Activities { get; set; } = [];
}
