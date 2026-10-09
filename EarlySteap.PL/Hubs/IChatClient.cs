using EarlyStep.Contracts.DTOS.Responses.ChatResponses;

namespace EarlySteap.PL.Hubs;

public interface IChatClient
{
    Task ReceiveMessage(MessageResponse message);
}
