namespace EarlyStep.DAL.Model;

public class Conversation
{
    public Guid Id { get; set; }

    public Guid UserAId { get; set; }
    public ApplicationUser UserA { get; set; }

    public Guid UserBId { get; set; }
    public ApplicationUser UserB { get; set; }

    public ICollection<Message> Messages { get; set; } = new List<Message>();
    public DateTime CreatedAt { get; set; }
}