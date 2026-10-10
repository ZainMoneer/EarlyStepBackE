using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Requests.SuperAdmimRequests;
using EarlyStep.Contracts.DTOS.Responses.SuperAdminResponses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EarlySteap.PL.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "SuperAdmin")]
public class SuperAdminController : ControllerBase
    {
    private readonly ISuperAdminRepository _superAdminRepository;

    public SuperAdminController(
        ISuperAdminRepository superAdminRepository)
        {
        _superAdminRepository = superAdminRepository;
        }
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(
        CancellationToken cancellationToken)
        {
        var result = await _superAdminRepository
            .GetDashboardAsync(cancellationToken);

        return Ok(result);
        }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] UserFilterRequest request,
        CancellationToken cancellationToken)
        {
        var result = await _superAdminRepository.GetUsersAsync(
            request,
            cancellationToken);

        return Ok(result);
        }

    [HttpGet("children")]
    public async Task<IActionResult> GetChildren(
        [FromQuery] ChildFilterRequest request,
        CancellationToken cancellationToken)
        {
        var result = await _superAdminRepository.GetChildrenAsync(
            request,
            cancellationToken);

        return Ok(result);
        }
    [HttpPost("users")]
    public async Task<ActionResult<UserManagementResponse>> CreateUser(
        [FromBody] CreateUserRequest request,
        CancellationToken cancellationToken)
        {
        var result = await _superAdminRepository.CreateUserAsync(
            request,
            cancellationToken);

        return StatusCode(StatusCodes.Status201Created, result);
        }
    [HttpPut("users/{id:guid}")]
    public async Task<ActionResult<UserManagementResponse>> UpdateUser(
    Guid id,
    [FromBody] UpdateUserRequest request,
    CancellationToken cancellationToken)
        {
        var result = await _superAdminRepository.UpdateUserAsync(
            id,
            request,
            cancellationToken);

        if (result is null)
            return NotFound(new { message = "User not found." });

        return Ok(result);
        }
    [HttpDelete("users/{id:guid}")]
    public async Task<IActionResult> DeleteUser(
    Guid id,
    CancellationToken cancellationToken)
        {
        var currentUserIdValue =
            User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(currentUserIdValue, out var currentUserId))
            return Unauthorized();

        var result = await _superAdminRepository.DeleteUserAsync(
            id,
            currentUserId,
            cancellationToken);

        if (!result.Succeeded)
            return UserManagementError(result);

        return Ok(new
            {
            message = "User deactivated successfully."
            });
        }

    private IActionResult UserManagementError(IdentityResult result)
        {
        var errors = result.Errors.ToArray();

        if (errors.Any(e => e.Code == "NotFound"))
            return NotFound(new { errors });

        if (errors.Any(e => e.Code == "Forbidden"))
            return StatusCode(403, new { errors });

        if (errors.Any(e => e.Code == "DuplicateEmail"))
            return Conflict(new { errors });

        return BadRequest(new { errors });
        }
    }