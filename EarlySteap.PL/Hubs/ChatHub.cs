using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Requests.ChatRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EarlySteap.PL.Hubs;

[Authorize]
public class ChatHub : Hub<IChatClient>
{
    private readonly IChatRepository _chatRepository;
    private readonly ILogger<ChatHub> _logger;

    public ChatHub(IChatRepository chatRepository, ILogger<ChatHub> logger)
    {
        _chatRepository = chatRepository;
        _logger = logger;
    }

    private Guid GetCurrentUserIdOrThrow()
    {
        var idStr = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);
        return !Guid.TryParse(idStr, out var id) ? throw new HubException("User not authenticated.") : id;
    }

    // User A Send Message -> To User B
    // [ChatHub] -> save in DB, [SignalR] -> push to User B, User B -> ReceiveMessage
    public async Task SendMessage(SendMessageRequest request)
    {
        if (request is null)
            throw new HubException("Request body is required.");

        var senderId = GetCurrentUserIdOrThrow();
        var ct = Context.ConnectionAborted;

        var message = await _chatRepository.SendMessageAsync(senderId, request, ct);

        await Clients.User(message.ReceiverId.ToString()).ReceiveMessage(message);
        await Clients.Caller.ReceiveMessage(message);

        _logger.LogInformation("ChatHub: Message {MessageId} sent from {Sender} to {Receiver} in conversation {Conv}",
            message.Id, message.SenderId, message.ReceiverId, message.ConversationId);
    }

    public override Task OnConnectedAsync()
    {
        _logger.LogDebug("ChatHub connected: UserIdentifier={UserId}", Context.UserIdentifier);
        return base.OnConnectedAsync();
    }

    public override Task OnDisconnectedAsync(Exception? exception)
    {
        _logger.LogDebug("ChatHub disconnected: UserIdentifier={UserId}. Error: {Error}",
            Context.UserIdentifier, exception?.Message);
        return base.OnDisconnectedAsync(exception);
    }
}
