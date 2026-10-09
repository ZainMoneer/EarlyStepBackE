using EarlyStep.Contracts.DTOS.Responses.AccountResponses;
using EarlyStep.DAL.Model;

namespace EarlyStep.BLL.Interfaces;

public interface ITokenService
{
    Task<AuthResponse> CreateTokenAsync(ApplicationUser user);
}