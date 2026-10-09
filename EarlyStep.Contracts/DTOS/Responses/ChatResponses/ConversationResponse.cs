namespace EarlyStep.Contracts.DTOS.Responses.ChatResponses;

public class ConversationResponse
{
    public Guid Id { get; set; }

    public Guid ReceiverId { get; set; }
    public string? ReceiverName { get; set; }

    public MessageResponse? LastMessage { get; set; }
    public int UnreadCount { get; set; }
}
