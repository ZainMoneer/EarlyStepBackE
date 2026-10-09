using EarlyStep.DAL.Model;
// Categories الأدمن اختارها للتقييم الأولي.
public class InitialAssessmentConfig
{
    public Guid Id { get; set; }

    public Guid CategoryId { get; set; }

    public Category Category { get; set; }
    public bool IsDeleted { get; set; }


    public ICollection<InitialAssessmentSelectedQuestion> SelectedQuestions { get; set; }
        = [];
}