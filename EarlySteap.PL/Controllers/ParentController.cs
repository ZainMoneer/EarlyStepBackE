using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Requests.ChildRequests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EarlySteap.PL.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "ولي أمر")]
public class ParentController : ControllerBase
    {
    private readonly IParentRepository _parentRepository;

    public ParentController(IParentRepository childRepository)
        {
        _parentRepository = childRepository;
        }

    [HttpPost]
    public async Task<IActionResult> Create(
        [FromForm] CreateChildRequest request,
        CancellationToken cancellationToken)
        {
        var userId = CurrentUserId();

        if (userId is null)
            return Unauthorized();

        var validationError =
            ValidateChildRequest(request);

        if (validationError is not null)
            return validationError;

        var child = await _parentRepository.CreateChildAsync(
            userId.Value,
            request,
            cancellationToken);

        return Ok(child);
        }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromForm] UpdateChildRequest request,
        CancellationToken cancellationToken)
        {
        var userId = CurrentUserId();

        if (userId is null)
            return Unauthorized();

        if (request.Age < 2 || request.Age > 10)
            return BadRequest(
                "عمر الطفل يجب أن يكون بين 2 و 10 سنوات.");

        var child = await _parentRepository.UpdateChildInfoAsync(
            id,
            userId.Value,
            request,
            cancellationToken);

        if (child is null)
            return NotFound();

        return Ok(child);
        }
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
        {
        var userId = CurrentUserId();

        if (userId is null)
            return Unauthorized();

        var deleted = await _parentRepository.DeleteChildAsync(
            id,
            userId.Value,
            cancellationToken);

        if (!deleted)
            return NotFound();

        return NoContent();
        }

    [HttpGet("GetMyChildren")]
    public async Task<IActionResult> GetMyChildren(
        CancellationToken cancellationToken)
        {
        var userId = CurrentUserId();

        if (userId is null)
            return Unauthorized();

        var children =
            await _parentRepository.GetMyChildrenAsync(
                userId.Value,
                cancellationToken);

        return Ok(children);
        }

    [HttpGet("{childId:guid}")]
    public async Task<IActionResult> GetChild(
        Guid childId,
        CancellationToken cancellationToken)
        {
        var parentId = CurrentUserId();

        if (parentId is null)
            return Unauthorized();

        var child = await _parentRepository.GetByIdAsync(
            childId,
            parentId.Value,
            cancellationToken);

        if (child is null)
            return NotFound();

        return Ok(child);
        }

    private Guid? CurrentUserId()
        {
        var value = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        return Guid.TryParse(value, out var id)
            ? id
            : null;
        }

    private BadRequestObjectResult? ValidateChildRequest(
        CreateChildRequest request)
        {
        if (string.IsNullOrWhiteSpace(request.FullName))
            return BadRequest("اسم الطفل مطلوب.");

        if (!request.Gender.HasValue)
            return BadRequest("الجنس مطلوب.");
        if (request.Age < 2 || request.Age > 10)
            return BadRequest(
                "عمر الطفل يجب أن يكون بين 2 و 10 سنوات.");

        return null;
        }



    }