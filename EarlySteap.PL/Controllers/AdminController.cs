using EarlyStep.BLL.Interfaces;
using EarlyStep.Contracts.DTOS.Requests.ActivityRequests;
using EarlyStep.Contracts.DTOS.Requests.AdminRequests;
using EarlyStep.Contracts.DTOS.Requests.CategoryRequests;
using EarlyStep.DAL.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EarlySteap.PL.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class AdminController : ControllerBase
    {
    private readonly IAdminRepository _adminRepository;
    private readonly IActivityRepository _activityRepository;

    public AdminController(
        IAdminRepository adminRepository,
        IActivityRepository activityRepository)
        {
        _adminRepository = adminRepository;
        _activityRepository = activityRepository;
        }


    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboard(
        CancellationToken cancellationToken)
        {
        var result = await _adminRepository
            .GetDashboardAsync(cancellationToken);

        return Ok(result);
        }



    [HttpGet("users")]
    public async Task<IActionResult> GetUsers(
        [FromQuery] UserFilterRequest request,
        CancellationToken cancellationToken)
        {
        var result = await _adminRepository
            .GetUsersAsync(
                request,
                cancellationToken);

        return Ok(result);
        }

    [HttpGet("users/{userId:guid}")]
    public async Task<IActionResult> GetUserById(
        Guid userId,
        CancellationToken cancellationToken)
        {
        var result = await _adminRepository
            .GetUserByIdAsync(
                userId,
                cancellationToken);

        if (result == null)
            {
            return NotFound(new
                {
                message = "المستخدم غير موجود."
                });
            }

        return Ok(result);
        }

    [HttpGet("specialists")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetAllSpecialists(
        CancellationToken cancellationToken)
        {
        var specialists = await _adminRepository
            .GetAllSpecialistsAsync(cancellationToken);

        return Ok(specialists);
        }



    [HttpPost("assign-children-to-specialist")]
    public async Task<IActionResult> AssignChildrenToSpecialist(
        [FromBody] AssignChildrenToSpecialistRequest request,
        CancellationToken cancellationToken)
        {
        var result = await _adminRepository
            .AssignChildrenToSpecialistAsync(
                request,
                cancellationToken);

        if (!result)
            {
            return BadRequest(new
                {
                message = "تعذر إضافة الطفل/الأطفال إلى الأخصائي."
                });
            }

        return Ok(new
            {
            message = "تمت إضافة الطفل/الأطفال للأخصائي بنجاح."
            });
        }

    // =========================================================
    // Categories
    // =========================================================

    [HttpGet("categories")]
    public async Task<IActionResult> GetAllCategories(
        CancellationToken cancellationToken)
        {
        var categories = await _adminRepository
            .GetAllCategoriesAsync(cancellationToken);

        return Ok(categories);
        }
    [HttpGet("categoriesActivity")]
    public async Task<IActionResult> GetCategories(
      string type,
      CancellationToken cancellationToken)
        {
        ActivityType activityType;
        try { activityType = ParseType(type); }
        catch (ArgumentException ex) { return BadRequest(new { message = ex.Message }); }

        var categories = await _activityRepository.GetActivityCategoriesAsync(
            activityType, cancellationToken);

        return Ok(categories);
        }


    [HttpPost("categories")]
    public async Task<IActionResult> CreateCategory(
        [FromBody] CreateCategoryRequest request,
        CancellationToken cancellationToken)
        {
        if (string.IsNullOrWhiteSpace(request.Name))
            {
            return BadRequest(new
                {
                message = "الاسم مطلوب."
                });
            }

        var category = await _adminRepository
            .CreateCategoryAsync(
                request,
                cancellationToken);

        return Ok(category);
        }

    [HttpPut("categories/{id:guid}")]
    public async Task<IActionResult> UpdateCategory(
        Guid id,
        [FromBody] UpdateCategoryRequest request,
        CancellationToken cancellationToken)
        {
        if (string.IsNullOrWhiteSpace(request.Name))
            {
            return BadRequest(new
                {
                message = "الاسم مطلوب."
                });
            }

        var category = await _adminRepository
            .UpdateCategoryAsync(
                id,
                request,
                cancellationToken);

        if (category == null)
            {
            return NotFound(new
                {
                message = "التصنيف غير موجود."
                });
            }

        return Ok(category);
        }

    [HttpDelete("categories/{id:guid}")]
    public async Task<IActionResult> DeleteCategory(
        Guid id,
        CancellationToken cancellationToken)
        {
        var deleted = await _adminRepository
            .DeleteCategoryAsync(
                id,
                cancellationToken);

        if (!deleted)
            {
            return NotFound(new
                {
                message = "التصنيف غير موجود."
                });
            }

        return Ok(new
            {
            message = "تم حذف التصنيف بنجاح."
            });
        }

    // =========================================================
    // Activities
    // =========================================================
    [HttpGet("activities/{type}")]
    public async Task<IActionResult> GetActivities(
    string type,
    [FromQuery] Guid? categoryId,
    CancellationToken cancellationToken)
        {
        ActivityType activityType;

        try
            {
            activityType = ParseType(type);
            }
        catch (ArgumentException ex)
            {
            return BadRequest(new
                {
                message = ex.Message
                });
            }

        var activities = await _activityRepository
            .GetActivitiesAsync(
                activityType,
                categoryId,
                cancellationToken);

        return Ok(activities);
        }
    [HttpGet("activities/{type}/{id:guid}")]
    public async Task<IActionResult> GetActivityById(
    string type,
    Guid id,
    CancellationToken cancellationToken)
        {
        ActivityType activityType;

        try
            {
            activityType = ParseType(type);
            }
        catch (ArgumentException ex)
            {
            return BadRequest(new
                {
                message = ex.Message
                });
            }

        var activity = await _activityRepository
            .GetActivityByIdAsync(
                id,
                activityType,
                cancellationToken);

        if (activity == null)
            {
            return NotFound(new
                {
                message = "النشاط غير موجود."
                });
            }

        return Ok(activity);
        }

    [HttpPost("activities/{type}")]
    public async Task<IActionResult> CreateActivity(
        string type,
         [FromForm] CreateActivityContentRequest request,
        CancellationToken cancellationToken)
        {
        ActivityType activityType;

        try
            {
            activityType = ParseType(type);
            }
        catch (ArgumentException ex)
            {
            return BadRequest(new
                {
                message = ex.Message
                });
            }

        try
            {
            var created = await _activityRepository
                .CreateActivityAsync(
                    request,
                    activityType,
                    CurrentAdminId,
                    cancellationToken);

            return CreatedAtAction(
                nameof(GetActivityById),
                new
                    {
                    type,
                    id = created.Id
                    },
                created);
            }
        catch (KeyNotFoundException ex)
            {
            return NotFound(new
                {
                message = ex.Message
                });
            }
        catch (ArgumentException ex)
            {
            return BadRequest(new
                {
                message = ex.Message
                });
            }
        catch (UnauthorizedAccessException)
            {
            return Forbid();
            }
        }


    [HttpPut("activities/{type}/{id:guid}")]
    public async Task<IActionResult> UpdateActivity(
        string type,
        Guid id,
       [FromForm] UpdateActivityRequest request,
        CancellationToken cancellationToken)
        {
        ActivityType activityType;

        try
            {
            activityType = ParseType(type);
            }
        catch (ArgumentException ex)
            {
            return BadRequest(new
                {
                message = ex.Message
                });
            }

        try
            {
            var updated = await _activityRepository
                .UpdateActivityAsync(
                    id,
                    activityType,
                    request,
                    cancellationToken);

            if (updated == null)
                {
                return NotFound(new
                    {
                    message = "النشاط غير موجود."
                    });
                }

            return Ok(updated);
            }
        catch (KeyNotFoundException ex)
            {
            return NotFound(new
                {
                message = ex.Message
                });
            }
        }


    [HttpDelete("activities/{type}/{id:guid}")]
    public async Task<IActionResult> DeleteActivity(
        string type,
        Guid id,
        CancellationToken cancellationToken)
        {
        ActivityType activityType;

        try
            {
            activityType = ParseType(type);
            }
        catch (ArgumentException ex)
            {
            return BadRequest(new
                {
                message = ex.Message
                });
            }

        var deleted = await _activityRepository
            .DeleteActivityAsync(
                id,
                activityType,
                cancellationToken);

        if (!deleted)
            {
            return NotFound(new
                {
                message = "النشاط غير موجود."
                });
            }

        return Ok(new
            {
            message = "تم حذف النشاط بنجاح."
            });
        }

    [HttpGet("questions/{questionId:guid}")]
    public async Task<IActionResult> GetQuestionById(
    Guid questionId,
    CancellationToken cancellationToken)
        {
        var question = await _activityRepository
            .GetQuestionByIdAsync(questionId, cancellationToken);

        if (question == null)
            {
            return NotFound(new
                {
                message = "السؤال غير موجود."
                });
            }

        return Ok(question);
        }
    [HttpPost("activities/{activityId:guid}/questions")]
    public async Task<IActionResult> CreateQuestion(
        Guid activityId,
       [FromForm] CreateQuestionRequest request,
        CancellationToken cancellationToken)
        {
        try
            {
            var question = await _activityRepository
                .CreateQuestionAsync(
                    activityId,
                    request,
                    cancellationToken);

            return Ok(question);
            }
        catch (KeyNotFoundException ex)
            {
            return NotFound(new
                {
                message = ex.Message
                });
            }
        catch (ArgumentException ex)
            {
            return BadRequest(new
                {
                message = ex.Message
                });
            }
        }
    [HttpPut("questions/{questionId:guid}/info")]
    public async Task<IActionResult> UpdateQuestionInfo(
        Guid questionId,
        [FromForm] UpdateQuestionRequest request,
        CancellationToken cancellationToken)
        {
        try
            {
            var updated = await _activityRepository
                .UpdateQuestionAsync(
                    questionId,
                    request,
                    cancellationToken);

            if (updated == null)
                {
                return NotFound(new
                    {
                    message = "السؤال غير موجود."
                    });
                }

            return Ok(updated);
            }
        catch (ArgumentException ex)
            {
            return BadRequest(new
                {
                message = ex.Message
                });
            }
        }

    [HttpDelete("questions/{questionId:guid}")]
    public async Task<IActionResult> DeleteQuestion(
        Guid questionId,
        CancellationToken cancellationToken)
        {
        var deleted = await _activityRepository
            .DeleteQuestionAsync(
                questionId,
                cancellationToken);

        if (!deleted)
            {
            return NotFound(new
                {
                message = "السؤال غير موجود."
                });
            }

        return Ok(new
            {
            message = "تم حذف السؤال بنجاح."
            });
        }
    [HttpPut("question-options/{optionId:guid}")]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> UpdateQuestionOption(
        Guid optionId,
        [FromForm] UpdateOptionRequest request,
        CancellationToken cancellationToken)
        {
        try
            {
            var updated = await _activityRepository
                .UpdateQuestionOptionAsync(
                    optionId,
                    request,
                    cancellationToken);

            if (updated == null)
                {
                return NotFound(new
                    {
                    message = "الخيار غير موجود."
                    });
                }

            return Ok(updated);
            }
        catch (ArgumentException ex)
            {
            return BadRequest(new
                {
                message = ex.Message
                });
            }
        }
    [HttpDelete("questions/{questionId}/options")]
    public async Task<IActionResult> DeleteAllQuestionOptions(
    Guid questionId,
    CancellationToken cancellationToken)
        {
        var result =
            await _activityRepository.DeleteAllQuestionOptionsAsync(
                questionId,
                cancellationToken);

        if (!result)
            {
            return NotFound(new
                {
                message = "لا توجد خيارات لهذا السؤال."
                });
            }

        return Ok(new
            {
            message = "تم حذف جميع خيارات السؤال بنجاح."
            });
        }

    [HttpDelete("question-options/{optionId:guid}")]
    public async Task<IActionResult> DeleteQuestionOption(
    Guid optionId,
    CancellationToken cancellationToken)
        {
        var deleted = await _activityRepository
            .DeleteQuestionOptionAsync(
                optionId,
                cancellationToken);

        if (!deleted)
            {
            return NotFound(new
                {
                message = "الخيار غير موجود."
                });
            }

        return Ok(new
            {
            message = "تم حذف الخيار بنجاح."
            });
        }


    private Guid CurrentAdminId
        {
        get
            {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!Guid.TryParse(userId, out var adminId))
                {
                throw new UnauthorizedAccessException(
                    "تعذر تحديد هوية المستخدم الحالي.");
                }

            return adminId;
            }
        }

    private static ActivityType ParseType(string type)
        {
        return type.Trim().ToLower() switch
            {
                "test" => ActivityType.اختبارات,
                "game" => ActivityType.ألعاب,

                _ => throw new ArgumentException(
                    "نوع النشاط غير صحيح. استخدم test أو game.")
                };
        }
    }
