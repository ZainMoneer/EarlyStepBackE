using EarlyStep.Contracts.DTOS.Requests.ChatRequests;
using EarlyStep.Contracts.DTOS.Responses.ChatResponses;

namespace EarlyStep.BLL.Interfaces;

public interface IChatRepository
{
    Task<List<ConversationResponse>> GetUserConversationsAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<MessageResponse> SendMessageAsync(Guid senderId, SendMessageRequest request, CancellationToken cancellationToken = default);

    Task<List<MessageResponse>> GetOneConversationAsync(Guid? conversationId, Guid currentUserId, Guid receiverId, int take = 20, CancellationToken cancellationToken = default);

    Task<List<MessageResponse>> LoadOlderAsync(Guid conversationId, Guid beforeId, int take = 20, CancellationToken cancellationToken = default);

    Task<List<MessageResponse>> LoadNewerAsync(Guid conversationId, Guid afterId, int take = 20, CancellationToken cancellationToken = default);

    Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<bool> MarkConversationAsSeenAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default);

    // كل المستخدمين (عدا المستخدم الحالي) يقدر يبدأ محادثة معهم
    Task<List<ChatUserResponse>> GetChatUsersAsync(Guid currentUserId, CancellationToken cancellationToken = default);
}
