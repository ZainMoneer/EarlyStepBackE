using EarlyStep.Contracts.DTOS.Requests.AccountRequests;
using EarlyStep.Contracts.DTOS.Responses.AccountResponses;
using Microsoft.AspNetCore.Identity;

namespace EarlyStep.BLL.Interfaces;

public interface IAuthRepository
{
    Task<RegisterResponse?> RegisterAsync(
        RegisterRequests register,
        CancellationToken cancellationToken = default); Task<AuthResponse?> LoginAsync(LoginRequests login, CancellationToken cancellationToken = default);
    Task<IdentityResult> ConfirmEmailAsync(string userId, string token, CancellationToken cancellationToken = default);

    Task<IdentityResult> ChangePasswordAsync(string userId, ChangePasswordRequest request, CancellationToken cancellationToken = default);
    Task<string?> ForgotPassword(ForgotPasswordRequest forgotPasswordRequest, CancellationToken cancellationToken = default);
    Task<IdentityResult> ResetPasswordAsync(ResetPasswordRequest request, CancellationToken cancellationToken = default);

}