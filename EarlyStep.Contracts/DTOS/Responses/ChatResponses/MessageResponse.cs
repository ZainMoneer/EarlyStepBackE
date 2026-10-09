namespace EarlyStep.Contracts.DTOS.Responses.ChatResponses;

public class MessageResponse
{
    public Guid Id { get; set; }
    public Guid ConversationId { get; set; }

    public Guid SenderId { get; set; }
    public string? SenderName { get; set; }

    public Guid ReceiverId { get; set; }
    public string? ReceiverName { get; set; }

    public string? Text { get; set; }

    public DateTime CreatedAt { get; set; }
    public string Status { get; set; } = null!;
}
