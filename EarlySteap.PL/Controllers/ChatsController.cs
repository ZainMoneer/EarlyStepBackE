using EarlySteap.PL.Hubs;
using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Requests.ChatRequests;
using EarlyStep.Contracts.DTOS.Responses.ChatResponses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;

namespace EarlySteap.PL.Controllers;

[Route("api/[controller]")]
[ApiController]
//[Authorize]
public class ChatsController : ControllerBase
    {
    private readonly IChatRepository _chatRepository;
    private readonly IHubContext<ChatHub, IChatClient> _hub;
    private readonly ILogger<ChatsController> _logger;

    public ChatsController(IChatRepository chatRepository, IHubContext<ChatHub, IChatClient> hub, ILogger<ChatsController> logger)
        {
        _chatRepository = chatRepository;
        _hub = hub;
        _logger = logger;
        }

    // بترجع null لو الـ token ما فيه claim صالح (توكن ناقص/غلط) بدل ما تفرقع
    private Guid? CurrentUserId()
        {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return Guid.TryParse(value, out var id) ? id : null;
        }

    [HttpGet("users")]
    public async Task<ActionResult<List<ChatUserResponse>>> GetChatUsers(CancellationToken ct = default)
        {
        var userId = CurrentUserId();
        if (userId is null)
            return Unauthorized();

        var users = await _chatRepository.GetChatUsersAsync(userId.Value, ct);
        return Ok(users);
        }

    [HttpGet("")]
    public async Task<ActionResult<List<MessageResponse>>> GetInitial([FromQuery] Guid? conversationId, [FromQuery] Guid? receiverId, [FromQuery] int take = 20, CancellationToken ct = default)
        {
        var userId = CurrentUserId();
        if (userId is null)
            return Unauthorized();

        if (!conversationId.HasValue && !receiverId.HasValue)
            return BadRequest("Provide conversationId or receiverId.");

        try
            {
            var list = conversationId.HasValue
                ? await _chatRepository.GetOneConversationAsync(conversationId, userId.Value, Guid.Empty, take, ct)
                : await _chatRepository.GetOneConversationAsync(null, userId.Value, receiverId!.Value, take, ct);

            return Ok(list);
            }
        catch (KeyNotFoundException ex)
            {
            return NotFound(ex.Message);
            }
        }

    [HttpPost("send")]
    public async Task<ActionResult<MessageResponse>> Send([FromBody] SendMessageRequest request, CancellationToken ct)
        {
        var userId = CurrentUserId();
        if (userId is null)
            return Unauthorized();

        var message = await _chatRepository.SendMessageAsync(userId.Value, request, ct);

        await _hub.Clients.User(message.ReceiverId.ToString()).ReceiveMessage(message);

        _logger.LogInformation("HTTP send: {MessageId} {Sender}->{Receiver}",
            message.Id, message.SenderId, message.ReceiverId);

        return Ok(message);
        }

    [HttpGet("messages/old")]
    public async Task<ActionResult<List<MessageResponse>>> GetOlder([FromQuery] Guid conversationId, [FromQuery] Guid beforeId, [FromQuery] int take = 20, CancellationToken ct = default)
        {
        var list = await _chatRepository.LoadOlderAsync(conversationId, beforeId, take, ct);
        return Ok(list);
        }

    [HttpGet("messages/new")]
    public async Task<ActionResult<List<MessageResponse>>> GetNewer([FromQuery] Guid conversationId, [FromQuery] Guid afterId, [FromQuery] int take = 20, CancellationToken ct = default)
        {
        var list = await _chatRepository.LoadNewerAsync(conversationId, afterId, take, ct);
        return Ok(list);
        }

    [HttpGet("conversations")]
    public async Task<ActionResult<List<ConversationResponse>>> GetConversations(CancellationToken ct)
        {
        var userId = CurrentUserId();
        if (userId is null)
            return Unauthorized();

        var list = await _chatRepository.GetUserConversationsAsync(userId.Value, ct);
        return Ok(list);
        }

    [HttpGet("unread-count")]
    public async Task<ActionResult<int>> GetUnreadCount(CancellationToken ct = default)
        {
        var userId = CurrentUserId();
        if (userId is null)
            return Unauthorized();

        var count = await _chatRepository.GetUnreadCountAsync(userId.Value, ct);
        return Ok(count);
        }

    [HttpPost("mark-conversation-seen")]
    public async Task<ActionResult<bool>> MarkConversationAsSeen([FromQuery] Guid conversationId, CancellationToken ct)
        {
        var userId = CurrentUserId();
        if (userId is null)
            return Unauthorized();

        var result = await _chatRepository.MarkConversationAsSeenAsync(conversationId, userId.Value, ct);
        return Ok(result);
        }
    }
