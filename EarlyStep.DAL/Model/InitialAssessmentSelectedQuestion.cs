//بمعنى مختصر: هذا المودل يخزن الأسئلة التي اختارها الأدمن من داخل الـ Activity لإدخالها في التقييم الأولي.

namespace EarlyStep.DAL.Model;

public class InitialAssessmentSelectedQuestion
{
    public Guid Id { get; set; }

    public Guid InitialAssessmentConfigId { get; set; }

    public InitialAssessmentConfig InitialAssessmentConfig { get; set; }

    public Guid QuestionId { get; set; }
    public ActivityQuestion Question { get; set; }

    public int Order { get; set; }
    public bool IsDeleted { get; set; }

}