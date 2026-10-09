namespace EarlyStep.Contracts.DTOS.Requests.ChatRequests;

public class SendMessageRequest
{
    public Guid? ConversationId { get; set; }
    public Guid ReceiverId { get; set; }
    public string Text { get; set; } = null!;
}
