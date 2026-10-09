

namespace EarlyStep.DAL.Model;

public class PasswordResetCode
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; }

    public string Code { get; set; }
    public DateTime ExpirationCode { get; set; }

    /// creation time 
    public DateTime Created { get; set; }

    /// True once consumed successfully.
    public bool IsUsed { get; set; }

    /// When it was consumed 
    public DateTime? UsedUtc { get; set; }
}