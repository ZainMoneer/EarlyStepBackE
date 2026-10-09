
namespace EarlyStep.DAL.Model;


public enum MessageStatus { Delivered, Seen }
public class Message
{
    public Guid Id { get; set; }

    public Guid ConversationId { get; set; }
    public Conversation Conversation { get; set; }

    public Guid SenderId { get; set; }
    public ApplicationUser Sender { get; set; }

    public Guid ReceiverId { get; set; }
    public ApplicationUser Receiver { get; set; }

    public string Text { get; set; } = null!;

    public MessageStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
}