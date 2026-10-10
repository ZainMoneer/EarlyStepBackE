
using System.ComponentModel.DataAnnotations;

namespace EarlyStep.Contracts.DTOS.Requests.SuperAdmimRequests;

public class UpdateUserRequest
    {
    [Required]
    public string FullName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Required]
    public string Role { get; set; }

    public bool IsActive { get; set; } = true;
    }

