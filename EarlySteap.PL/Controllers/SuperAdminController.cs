using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Requests.SuperAdmimRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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
}