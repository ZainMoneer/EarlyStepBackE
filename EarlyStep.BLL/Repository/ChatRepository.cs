using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Requests.ChatRequests;
using EarlyStep.Contracts.DTOS.Responses.ChatResponses;
using EarlyStep.DAL.Data;
using EarlyStep.DAL.Model;
using Microsoft.EntityFrameworkCore;

namespace EarlyStep.BLL.Repository;

public class ChatRepository : IChatRepository
    {
    private readonly ApplicationDbContext _context;

    public ChatRepository(ApplicationDbContext context)
        {
        _context = context;
        }


    public async Task<List<MessageResponse>> GetOneConversationAsync(Guid? conversationId, Guid currentUserId, Guid receiverId, int take = 20, CancellationToken cancellationToken = default)
        {
        Conversation? conversation;

        if (conversationId.HasValue)
            {
            conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == conversationId.Value && (c.UserAId == currentUserId || c.UserBId == currentUserId), cancellationToken);

            if (conversation is null)
                throw new KeyNotFoundException("Conversation not found or access denied.");
            }
        else
            {
            conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => (c.UserAId == currentUserId && c.UserBId == receiverId) || (c.UserAId == receiverId && c.UserBId == currentUserId), cancellationToken);

            conversation ??= await CreateConversationAsync(currentUserId, receiverId, cancellationToken);
            }

        var descending = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .AsNoTracking()
            .Where(m => m.ConversationId == conversation.Id)
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Take(take)
            .ToListAsync(cancellationToken);

        return descending
            .OrderBy(m => m.CreatedAt)
            .ThenBy(m => m.Id)
            .Select(ToResponse)
            .ToList();
        }
    //   3AF55EE2-861D-467C-D534-08DEEE58BA4F	FeryalJaber
    //B3EAD7AD-9169-41D3-D535-08DEEE58BA4F	ZainMONEER  Sender
    public async Task<MessageResponse> SendMessageAsync(Guid senderId, SendMessageRequest request, CancellationToken cancellationToken = default)
        {
        Conversation? conversation = null;

        if (request.ConversationId.HasValue)
            {
            conversation = await _context.Conversations
                .FirstOrDefaultAsync(c => c.Id == request.ConversationId.Value, cancellationToken);
            }

        conversation ??= await _context.Conversations
            .FirstOrDefaultAsync(
                c => (c.UserAId == senderId && c.UserBId == request.ReceiverId) ||
                     (c.UserAId == request.ReceiverId && c.UserBId == senderId),
                cancellationToken);

        conversation ??= await CreateConversationAsync(senderId, request.ReceiverId, cancellationToken);

        var receiverId = conversation.UserAId == senderId
            ? conversation.UserBId
            : conversation.UserAId;

        var message = new Message
            {
            Id = Guid.NewGuid(),
            ConversationId = conversation.Id,
            SenderId = senderId,
            ReceiverId = receiverId,
            Text = request.Text,
            Status = MessageStatus.Delivered,
            CreatedAt = DateTime.Now
            };

        _context.Messages.Add(message);
        await _context.SaveChangesAsync(cancellationToken);

        var saved = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .FirstAsync(m => m.Id == message.Id, cancellationToken);

        return ToResponse(saved);
        }

    public async Task<List<MessageResponse>> LoadOlderAsync(Guid conversationId, Guid beforeId, int take = 20, CancellationToken cancellationToken = default)
        {
        var pivot = await _context.Messages.AsNoTracking()
            .Where(m => m.Id == beforeId && m.ConversationId == conversationId)
            .Select(m => new { m.CreatedAt, m.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (pivot is null)
            return [];

        var older = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .AsNoTracking()
            .Where(m =>
                m.ConversationId == conversationId &&
                (m.CreatedAt < pivot.CreatedAt ||
                 (m.CreatedAt == pivot.CreatedAt && m.Id.CompareTo(pivot.Id) < 0)))
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Take(take)
            .ToListAsync(cancellationToken);

        return older
            .OrderBy(m => m.CreatedAt)
            .ThenBy(m => m.Id)
            .Select(ToResponse)
            .ToList();
        }

    public async Task<List<MessageResponse>> LoadNewerAsync(Guid conversationId, Guid afterId, int take = 20, CancellationToken cancellationToken = default)
        {
        var pivot = await _context.Messages.AsNoTracking()
            .Where(m => m.Id == afterId && m.ConversationId == conversationId)
            .Select(m => new { m.CreatedAt, m.Id })
            .FirstOrDefaultAsync(cancellationToken);

        if (pivot is null)
            return [];

        var newer = await _context.Messages
            .Include(m => m.Sender)
            .Include(m => m.Receiver)
            .AsNoTracking()
            .Where(m =>
                m.ConversationId == conversationId &&
                (m.CreatedAt > pivot.CreatedAt ||
                 (m.CreatedAt == pivot.CreatedAt && m.Id.CompareTo(pivot.Id) > 0)))
            .OrderBy(m => m.CreatedAt)
            .ThenBy(m => m.Id)
            .Take(take)
            .ToListAsync(cancellationToken);

        return newer.Select(ToResponse).ToList();
        }

    public async Task<List<ConversationResponse>> GetUserConversationsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
        return await _context.Conversations
            .Include(c => c.UserA)
            .Include(c => c.UserB)
            .Include(c => c.Messages)
            .AsNoTracking()
            .Where(c => c.UserAId == userId || c.UserBId == userId)
            .Select(c => new ConversationResponse
                {
                Id = c.Id,

                ReceiverId = userId == c.UserAId ? c.UserBId : c.UserAId,
                ReceiverName = userId == c.UserAId ? c.UserB.FullName : c.UserA.FullName,

                LastMessage = c.Messages
                    .OrderByDescending(m => m.CreatedAt)
                    .Select(m => new MessageResponse
                        {
                        Id = m.Id,
                        ConversationId = m.ConversationId,
                        Text = m.Text,
                        CreatedAt = m.CreatedAt,
                        Status = m.Status.ToString(),
                        SenderId = m.SenderId,
                        SenderName = m.Sender.FullName,
                        ReceiverId = m.ReceiverId,
                        ReceiverName = m.Receiver.FullName
                        })
                    .FirstOrDefault(),

                UnreadCount = c.Messages.Count(m =>
                    m.ReceiverId == userId &&
                    m.Status == MessageStatus.Delivered)
                })
            .OrderByDescending(c => c.LastMessage!.CreatedAt)
            .ToListAsync(cancellationToken);
        }

    public async Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken = default)
        {
        return await _context.Messages
            .AsNoTracking()
            .Where(m => m.ReceiverId == userId && m.Status == MessageStatus.Delivered)
            .CountAsync(cancellationToken);
        }

    public async Task<bool> MarkConversationAsSeenAsync(Guid conversationId, Guid userId, CancellationToken cancellationToken = default)
        {
        var unreadMessages = await _context.Messages
            .Where(m => m.ConversationId == conversationId &&
                        m.ReceiverId == userId &&
                        m.Status == MessageStatus.Delivered)
            .ToListAsync(cancellationToken);

        if (unreadMessages.Count == 0)
            return false;

        foreach (var message in unreadMessages)
            message.Status = MessageStatus.Seen;

        await _context.SaveChangesAsync(cancellationToken);
        return true;
        }
    public async Task<List<ChatUserResponse>> GetChatUsersAsync(
        Guid currentUserId,
        CancellationToken cancellationToken = default)
        {
        var currentRole = await _context.UserRoles
            .Where(ur => ur.UserId == currentUserId)
            .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
            .FirstOrDefaultAsync(cancellationToken);

        IQueryable<Guid> allowedIds;

        switch (currentRole)
            {
            case Roles.Parent:
                    {
                    var myChildren = _context.Children.Where(c => c.ParentId == currentUserId);

                    var teachers = myChildren
                        .Where(c => c.TeacherId != null)
                        .Select(c => c.TeacherId!.Value);

                    var specialists = myChildren
                        .Where(c => c.SpecialistId != null)
                        .Select(c => c.SpecialistId!.Value);

                    allowedIds = teachers.Union(specialists);
                    break;
                    }

            case Roles.Teacher:
                    {
                    var myChildren = _context.Children.Where(c => c.TeacherId == currentUserId);

                    var parents = myChildren.Select(c => c.ParentId);

                    var specialists = myChildren
                        .Where(c => c.SpecialistId != null)
                        .Select(c => c.SpecialistId!.Value);

                    allowedIds = parents.Union(specialists);
                    break;
                    }

            case Roles.Specialist:
                    {
                    var myChildren = _context.Children.Where(c => c.SpecialistId == currentUserId);

                    var parents = myChildren.Select(c => c.ParentId);

                    var teachers = myChildren
                        .Where(c => c.TeacherId != null)
                        .Select(c => c.TeacherId!.Value);

                    allowedIds = parents.Union(teachers);
                    break;
                    }

            case Roles.Admin:
            case Roles.SuperAdmin:
                // ??????: ?? ???????? (???? ?????? ??????)
                allowedIds = _context.UserRoles
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => new { ur.UserId, r.Name })
                    .Where(x => x.Name != Roles.Parent)
                    .Select(x => x.UserId);
                break;

            default:
                return new List<ChatUserResponse>();
            }

        var users = await _context.Users
            .Where(u => u.Id != currentUserId && allowedIds.Contains(u.Id))
            .Select(u => new
                {
                u.Id,
                u.FullName,
                Role = _context.UserRoles
                    .Where(ur => ur.UserId == u.Id)
                    .Join(_context.Roles, ur => ur.RoleId, r => r.Id, (ur, r) => r.Name)
                    .FirstOrDefault()
                })
            .OrderBy(u => u.FullName)
            .ToListAsync(cancellationToken);

        return users.Select(u => new ChatUserResponse
            {
            Id = u.Id,
            Name = u.FullName,
            Role = u.Role ?? string.Empty
            }).ToList();
        }

    private async Task<Conversation> CreateConversationAsync(Guid userAId, Guid userBId, CancellationToken cancellationToken)
        {

        var conversation = new Conversation
            {
            Id = Guid.NewGuid(),
            UserAId = userAId,
            UserBId = userBId,
            CreatedAt = DateTime.UtcNow
            };

        _context.Conversations.Add(conversation);
        await _context.SaveChangesAsync(cancellationToken);
        return conversation;
        }

    private static MessageResponse ToResponse(Message m) => new()
        {
        Id = m.Id,
        ConversationId = m.ConversationId,
        SenderId = m.SenderId,
        SenderName = m.Sender.FullName,
        ReceiverId = m.ReceiverId,
        ReceiverName = m.Receiver.FullName,
        Text = m.Text,
        CreatedAt = m.CreatedAt,
        Status = m.Status.ToString()
        };
    }