namespace EarlyStep.Contracts.DTOS.Requests.AccountRequests;

public class ResetPasswordRequest
{

    public required string Email { get; set; }
    public required string Code { get; set; }
    public required string NewPassword { get; set; }
    public required string ConfirmPassword { get; set; }
}